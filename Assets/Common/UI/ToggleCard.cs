using Common.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleCard : Card
{
    [SerializeField] private Toggle Selectable;
    private Color LabelColorActive;
    public Color LabelColorInactive = Color.white;
    private void Awake()
    {
        if (_label != null)
            LabelColorActive = _label.color;
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
            _label.color = value ? LabelColorActive : LabelColorInactive;
    }

    public void Set(string ID, string label = "", Sprite icon = null, UnityAction<bool> callback = null)
    {
        base.Set(ID, label, icon);
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

    public override void SetEmpty(string label = "")
    {
        base.SetEmpty(label);
        Interactable = false;
    }

    public Toggle GetToggle()
    {
        return Selectable;
    }
}