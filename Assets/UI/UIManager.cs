using System;
using Controllers;
using UnityEngine;
using Utility;

namespace UI
{
    [Serializable]
    public class UIManager : IControllable
    {
        private IUIControl _currentControl;
        [SerializeField] private ColorPaletteObject _palette;
        private SoundManager _soundManager => GameController.Instance.Sound;

        public void SetUp()
        {
            _currentControl = null;
            ColorPaletteUtilities.GetHex(_palette);
        }

        public void SetDown()
        {
        }

        public bool HasControl()
        {
            return _currentControl != null;
        }

        public bool HasControl(IUIControl control)
        {
            return _currentControl == control && control != null;
        }

        public bool TrySetActive(IUIControl control)
        {
            if (_currentControl != null) return false;
            _currentControl = control;
            _currentControl.Active = true;
            return true;
        }

        public void EndControl(IUIControl control)
        {
            if (_currentControl != control) return;
            _currentControl.ForceShutdown();
            _currentControl.Active = false;
            _currentControl = null;
            _soundManager.PlayCancel();
        }

        public void EndControl()
        {
            EndControl(_currentControl);
        }
    }
}