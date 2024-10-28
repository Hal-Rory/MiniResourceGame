using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class Card : MonoBehaviour
    {
        [SerializeField] protected Text _label;
        [SerializeField] protected Image _icon;
        [SerializeField] private Sprite _emptyImage;

        [field:SerializeField] public string ID { get; private set; }
        public string LabelText => _label != null ? _label.text : string.Empty;
        public Sprite IconSprite => _icon != null ? _icon.sprite : null;
        public void SetLabel(string label)
        {
            if (_label == null)
            {
                return;
            }
            _label.text = label;
        }
        public void SetIcon(Sprite sprite)
        {
            if (_icon == null)
            {
                return;
            }
            if (sprite == null)
            {
                _icon.enabled = false;
            }
            else
            {
                _icon.enabled = true;
                _icon.sprite = sprite;
            }
        }
        public void Set(string id)
        {
            ID = id;
        }
        public void Set(string id, string label = "", Sprite sprite = null)
        {
            ID = id;
            SetLabel(label);
            SetIcon(sprite);
        }
        public virtual void SetEmpty(string label = "")
        {
            ID = string.Empty;
            SetIcon(_emptyImage);
            _label.text = label;
        }

        public void SetIconActive(bool active)
        {
            _icon.gameObject.SetActive(active);
        }
    }
}