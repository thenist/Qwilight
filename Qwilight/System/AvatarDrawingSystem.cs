using System.Collections.Concurrent;
using System.Net;

namespace Qwilight
{
    public sealed class AvatarDrawingSystem
    {
        public static readonly AvatarDrawingSystem Instance = new();

        readonly ConcurrentDictionary<string, HandledDrawingItem> _avatarDrawings = new();
        readonly ConcurrentDictionary<string, SemaphoreSlim> _avatarCSXs = new();
        readonly ConcurrentQueue<IDisposable> _pendingClosables = new();

        public void WipeAvatarDrawing(string avatarID)
        {
            if (_avatarDrawings.TryRemove(avatarID, out var avatarDrawing))
            {
                _pendingClosables.Enqueue(avatarDrawing);
            }
        }

        public void WipeAvatarDrawings()
        {
            foreach (var avatarID in _avatarDrawings.Keys)
            {
                WipeAvatarDrawing(avatarID);
            }
        }

        public void ClosePendingClosables()
        {
            while (_pendingClosables.TryDequeue(out var pendingAvatarDrawing))
            {
                pendingAvatarDrawing.Dispose();
            }
        }

        SemaphoreSlim GetCSX(string avatarID) => new(1);

        public bool CanCallAPI(string avatarID)
        {
            avatarID ??= string.Empty;
            return _avatarCSXs.GetOrAdd(avatarID, GetCSX).CurrentCount > 0;
        }

        public async Task<HandledDrawingItem> GetAvatarDrawing(string avatarID)
        {
            avatarID ??= string.Empty;
            var avatarCSX = _avatarCSXs.GetOrAdd(avatarID, GetCSX);
            try
            {
                await avatarCSX.WaitAsync().ConfigureAwait(false);
                if (!_avatarDrawings.TryGetValue(avatarID, out var avatarDrawing))
                {
                    if (string.IsNullOrEmpty(avatarID))
                    {
                        avatarDrawing = new();
                    }
                    else
                    {
                        try
                        {
                            using var s = await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?avatarID={WebUtility.UrlEncode(avatarID)}&drawingVariety=0").ConfigureAwait(false);
                            avatarDrawing = new()
                            {
                                Drawing = DrawingSystem.Instance.Load(s, null),
                                DefaultDrawing = DrawingSystem.Instance.LoadDefault(s, null)
                            };
                        }
                        catch
                        {
                        }
                    }
                }
                _avatarDrawings[avatarID] = avatarDrawing;
                return avatarDrawing;
            }
            finally
            {
                avatarCSX.Release();
            }
        }

        public HandledDrawingItem? JustGetAvatarDrawing(string avatarID) => !_avatarDrawings.TryGetValue(avatarID ?? string.Empty, out var avatarDrawing) ? null : avatarDrawing;
    }
}
