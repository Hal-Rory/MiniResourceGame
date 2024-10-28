public class UIManager : IControllable
{
    private IUIControl _currentControl;

    public void SetUp()
    {
        _currentControl = null;
    }

    public void SetDown()
    {
    }

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