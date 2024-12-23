public interface IUIControl
{
    /// <summary>
    /// Close the entire UI setup
    /// </summary>
    public void ForceShutdown();

    public bool Active { get; set; }
}