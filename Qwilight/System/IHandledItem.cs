namespace Qwilight
{
    public interface IHandledItem
    {
        double Length { get; }

        bool IsLooping { get; }

        IHandlerItem Handle(IMediaHandler mediaHandler, TimeSpan levyingWait, MediaNote.Mode mode);
    }
}