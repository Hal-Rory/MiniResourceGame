public interface IUIControl
{
    public void ForceShutdown();
    public bool Active { get; set; }
}