namespace Qwilight
{
    public struct DrawingHandlerItem : IHandlerItem
    {
        public HandledDrawingItem DrawingComputingValue { get; set; }

        public bool WasDefaultMediaHandled { get; set; }

        public IHandledItem Value => DrawingComputingValue;
    }
}