namespace Qwilight
{
    public interface IHandledItem
    {
        double Length { get; }

        IHandlerItem Handle(IMediaHandler mediaHandler, TimeSpan levyingWait, MediaNote.Mode mode, bool isLooping = false);
    }
}