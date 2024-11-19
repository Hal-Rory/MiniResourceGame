namespace Interfaces
{
    public interface IActionBoxHolder
    {
        /// <summary>
        /// If the actionbox needs to be externally picked up
        /// perform this
        /// </summary>
        public void PickingUp();

        /// <summary>
        /// If the actionbox needs to be externally put down
        /// perform this
        /// </summary>
        public void PuttingDown();
    }
}