using System;
using Controllers;
using UnityEngine;
using Utility;

namespace UI
{
    /// <summary>
    /// Handles the current state of UI to manage which windows are currently in main control
    /// </summary>
    [Serializable]
    public class UIManager : IControllable
    {
        private IUIControl _currentControl;
        /// <summary>
        /// The color palette object to use for the current scheme
        /// </summary>
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
            //Swapping controls
            if (_currentControl != null)
            {
                EndControl();
            }
            _currentControl = control;
            _currentControl.Active = true;
            return true;
        }

        public void EndControl(IUIControl control)
        {
            if (_currentControl != control) return;
            EndControl();
            _soundManager.PlayCancel();
        }

        public void EndControl()
        {
            if (_currentControl == null) return;
            _currentControl.ForceShutdown();
            _currentControl.Active = false;
            _currentControl = null;
        }

        /// <summary>
        /// Returns the current color palette
        /// </summary>
        /// <returns></returns>
        public ColorPaletteObject GetPalette()
        {
            return _palette;
        }
    }
}