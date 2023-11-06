using Windows.System;

namespace Qwilight
{
    public interface IHandleInput
    {
        public void HandleInput(VirtualKey rawInput, bool isInput);
    }
}
