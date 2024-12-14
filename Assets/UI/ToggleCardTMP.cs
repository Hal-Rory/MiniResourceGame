using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class ToggleCardTMP : CardTMP
    {
        [SerializeField] private Toggle Selectable;
        private void Awake()
        {
            Interactable = Selectable.interactable;
        }
        public bool Interactable
        {
            get => Selectable.interactable;
            set => Selectable.interactable = value;
        }

        public void Set(string ID, string header = "", string label = "", Sprite icon = null, UnityAction<bool> callback = null)
        {
            base.Set(ID, header, label, icon);
            SetAction(callback);
        }

        public void SetActive(bool active, bool notify = false)
        {
            if (notify)
            {
                Selectable.isOn = active;
            }
            else
            {
                Selectable.SetIsOnWithoutNotify(active);
            }
        }

        public void AddListener(UnityAction<bool> callback)
        {
            Selectable.onValueChanged.AddListener(callback);
        }

        public void RemoveListener(UnityAction<bool> callback)
        {
            Selectable.onValueChanged.RemoveListener(callback);
        }

        public void SetAction(UnityAction<bool> callback)
        {
            Selectable.onValueChanged.RemoveAllListeners();
            if (callback != null) AddListener(callback);
        }

        public override void SetEmpty(string header = "", string label = "")
        {
            base.SetEmpty(header, label);
            Interactable = false;
        }

        public Toggle GetToggle()
        {
            return Selectable;
        }
    }
}