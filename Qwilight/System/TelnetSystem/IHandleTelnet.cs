namespace Qwilight
{
    public interface IHandleTelnet
    {
        public bool IsAvailable { get; set; }

        public bool IsAlwaysNewStand { get; set; }

        public void Toggle();
    }
}
