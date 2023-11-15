using System.Collections.Concurrent;
using System.Net;

namespace Qwilight
{
    public sealed class AvatarEdgeSystem
    {
        public static readonly AvatarEdgeSystem Instance = new();

        readonly ConcurrentDictionary<string, HandledDrawingItem> _avatarEdges = new();
        readonly ConcurrentDictionary<string, SemaphoreSlim> _avatarCSXs = new();
        readonly ConcurrentQueue<HandledDrawingItem> _pendingAvatarEdges = new();

        public void WipeAvatarEdge(string avatarID)
        {
            if (_avatarEdges.TryRemove(avatarID, out var avatarEdge))
            {
                _pendingAvatarEdges.Enqueue(avatarEdge);
            }
        }

        public void WipeAvatarEdges()
        {
            foreach (var avatarID in _avatarEdges.Keys)
            {
                WipeAvatarEdge(avatarID);
            }
        }

        public void ClosePendingAvatarEdges()
        {
            while (_pendingAvatarEdges.TryDequeue(out var pendingAvatarEdge))
            {
                pendingAvatarEdge.Dispose();
            }
        }

        SemaphoreSlim GetCSX(string avatarID) => new(1);

        public bool CanCallAPI(string avatarID)
        {
            avatarID ??= string.Empty;
            return _avatarCSXs.GetOrAdd(avatarID, GetCSX).CurrentCount > 0;
        }

        public async Task<HandledDrawingItem> GetAvatarEdge(string avatarID)
        {
            avatarID ??= string.Empty;
            var avatarCSX = _avatarCSXs.GetOrAdd(avatarID, GetCSX);
            try
            {
                await avatarCSX.WaitAsync().ConfigureAwait(false);
                if (!_avatarEdges.TryGetValue(avatarID, out var avatarEdge))
                {
                    if (string.IsNullOrEmpty(avatarID))
                    {
                        avatarEdge = new();
                    }
                    else
                    {
                        try
                        {
                            using var s = await TwilightSystem.Instance.GetWwwParallel($"{QwilightComponent.QwilightAPI}/drawing?avatarID={WebUtility.UrlEncode(avatarID)}&drawingVariety=2").ConfigureAwait(false);
                            avatarEdge = new()
                            {
                                Drawing = DrawingSystem.Instance.Load(s, null),
                                DefaultDrawing = DrawingSystem.Instance.LoadDefault(s, null)
                            };
                        }
                        catch
                        {
                        }
                    }
                    _avatarEdges[avatarID] = avatarEdge;
                }
                return avatarEdge;
            }
            finally
            {
                avatarCSX.Release();
            }
        }

        public HandledDrawingItem? JustGetAvatarEdge(string avatarID) => !_avatarEdges.TryGetValue(avatarID ?? string.Empty, out var avatarEdge) ? null : avatarEdge;
    }
}
