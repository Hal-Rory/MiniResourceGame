using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionUI : MonoBehaviour
{
    private ObjectManager _objectManager => GameController.Instance.ObjectManager;
    public Image CurrentObjectDisplay;
    public Text CurrentObjectLabel;
    public GameObject CurrentObjectLabelDisplay;

    private void Start()
    {
        UpdateObjectSelection(Vector2.zero);
        GameController.Instance.InputManager.OnScroll += UpdateObjectSelection;
    }

    private void UpdateObjectSelection(Vector2 delta)
    {
        _objectManager.UpdateObjectSelection((int)delta.y);
        CurrentObjectDisplay.sprite = _objectManager.CurrentObject ? _objectManager.CurrentObject.ObjPreview : null;
        CurrentObjectLabel.text = _objectManager.CurrentObject ? _objectManager.CurrentObject.Name : string.Empty;
        CurrentObjectLabelDisplay.SetActive(_objectManager.CurrentObject);
    }
}