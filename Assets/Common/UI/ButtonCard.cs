using Common.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonCard : Card
{
    [SerializeField] private Button Selectable;
    private Color _labelColorActive;
    public Color LabelColorInactive = Color.white;
    private void Awake()
    {
        if (_label != null)
            _labelColorActive = _label.color;
        Interactable = Selectable.interactable;
    }
    public bool Interactable
    {
        get => Selectable.interactable; set
        {
            Selectable.interactable = value;
            SetLabelInteractable(value);
        }
    }

    public void SetLabelInteractable(bool value)
    {
        if (_label != null)
            _label.color = value ? _labelColorActive : LabelColorInactive;
    }

    public void Set(string id, string label = "", Sprite icon = null, UnityAction callback = null)
    {
        base.Set(id, label, icon);
        SetAction(callback);
    }

    public void AddListener(UnityAction callback)
    {
        Selectable.onClick.AddListener(callback);
    }

    public void RemoveListener(UnityAction callback)
    {
        Selectable.onClick.RemoveListener(callback);
    }

    public void SetAction(UnityAction callback)
    {
        Selectable.onClick.RemoveAllListeners();
        if (callback != null) AddListener(callback);
    }

    public void Select()
    {
        Selectable.onClick.Invoke();
    }

    public override void SetEmpty(string label = "")
    {
        base.SetEmpty(label);
        Interactable = false;
    }
}