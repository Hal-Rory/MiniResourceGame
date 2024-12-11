using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardTMP : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _header;
    [SerializeField] protected TextMeshProUGUI _label;
    [SerializeField] protected Image _icon;
    [SerializeField] private Sprite _emptyImage;

    [field: SerializeField] public string ID { get; private set; }
    public string HeaderText => _header != null ? _header.text : string.Empty;
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

    public void SetHeader(string header)
    {
        if (_header == null)
        {
            return;
        }

        _header.text = header;
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

    public void Set(string id, string header = "", string label = "", Sprite sprite = null)
    {
        ID = id;
        SetLabel(label);
        SetHeader(header);
        SetIcon(sprite);
    }

    public virtual void SetEmpty(string header = "", string label = "")
    {
        ID = string.Empty;
        SetIcon(_emptyImage);
        _label.text = label;
        _header.text = header;
    }

    public void SetIconActive(bool active)
    {
        _icon.gameObject.SetActive(active);
    }
}