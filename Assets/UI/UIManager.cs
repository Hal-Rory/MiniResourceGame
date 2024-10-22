using UnityEngine;

public class UIManager : MonoBehaviour
{
    private IUIControl _currentControl;

    public bool HasControl(IUIControl control)
    {
        return _currentControl == control && control != null;
    }

    public bool TrySetActive(IUIControl control)
    {
        if (_currentControl != null) return false;
        _currentControl = control;
        return true;
    }

    public void EndControl(IUIControl control)
    {
        if (_currentControl != control) return;
        _currentControl = null;
    }
}