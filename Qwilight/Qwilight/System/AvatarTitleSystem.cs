using Microsoft.UI;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.Concurrent;
using System.Net;

namespace Qwilight
{
    public sealed class AvatarTitleSystem
    {
        public static readonly AvatarTitleSystem Instance = new();

        static readonly AvatarTitle NotAvatarTitle = new(LanguageSystem.Instance.NotAvatarTitle, Paints.Paint4, Colors.White);

        readonly ConcurrentDictionary<string, AvatarTitle?> _avatarTitles = new();
        readonly ConcurrentDictionary<string, SemaphoreSlim> _avatarCSXs = new();

        public void WipeAvatarTitle(string avatarID) => _avatarTitles.TryRemove(avatarID, out _);

        public void WipeAvatarTitles() => _avatarTitles.Clear();

        SemaphoreSlim GetCSX(string avatarID) => new(1);

        public bool CanCallAPI(string avatarID)
        {
            avatarID ??= string.Empty;
            return _avatarCSXs.GetOrAdd(avatarID, GetCSX).CurrentCount > 0;
        }

        public async ValueTask<AvatarTitle?> GetAvatarTitle(string avatarID, bool allowNotAvatarTitle = false)
        {
            avatarID ??= string.Empty;
            var avatarCSX = _avatarCSXs.GetOrAdd(avatarID, GetCSX);
            try
            {
                await avatarCSX.WaitAsync();
                if (!_avatarTitles.TryGetValue(avatarID, out var avatarTitle))
                {
                    if (string.IsNullOrEmpty(avatarID))
                    {
                        avatarTitle = new(string.Empty, default, default);
                    }
                    else
                    {
                        try
                        {
                            var twilightWwwTitle = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwTitle?>($"{QwilightComponent.QwilightAPI}/title?avatarID={WebUtility.UrlEncode(avatarID)}&language={Configure.Instance.Language}");
                            avatarTitle = twilightWwwTitle.HasValue ? new AvatarTitle(twilightWwwTitle.Value.title, Utility.GetTitlePaint(twilightWwwTitle.Value.titleColor), Utility.GetTitleColor(twilightWwwTitle.Value.titleColor)) : new AvatarTitle(string.Empty, default, default);
                        }
                        catch
                        {
                        }
                    }
                    _avatarTitles[avatarID] = avatarTitle;
                }
                return allowNotAvatarTitle && string.IsNullOrEmpty(avatarTitle?.Title) ? NotAvatarTitle : avatarTitle;
            }
            finally
            {
                avatarCSX.Release();
            }
        }

        public AvatarTitle? JustGetAvatarTitle(string avatarID, bool allowNotAvatarTitle = false) => !_avatarTitles.TryGetValue(avatarID ?? string.Empty, out var avatarTitle) ? null as AvatarTitle? : allowNotAvatarTitle && string.IsNullOrEmpty(avatarTitle?.Title) ? NotAvatarTitle : avatarTitle;
    }
}
