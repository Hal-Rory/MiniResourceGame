using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
    public class Card : MonoBehaviour
    {
        [SerializeField] protected Text Label;
        [SerializeField] protected Image Icon;
        [SerializeField] private Sprite EmptyImage;

        [field:SerializeField] public string ID { get; private set; }
        public string LabelText => Label != null ? Label.text : string.Empty;
        public Sprite IconSprite => Icon != null ? Icon.sprite : null;
        public void SetLabel(string label)
        {
            if (Label == null)
            {
                return;
            }
            Label.text = label;
        }
        public void SetIcon(Sprite sprite)
        {
            if (Icon == null)
            {
                return;
            }
            if (sprite == null)
            {
                Icon.enabled = false;
            }
            else
            {
                Icon.enabled = true;
                Icon.sprite = sprite;
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
            SetIcon(EmptyImage);
            Label.text = label;
        }
    }
}