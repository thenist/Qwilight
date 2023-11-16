﻿using FFmpegInteropX;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Collections.Concurrent;
using System.IO;

namespace Qwilight
{
    public sealed class MediaSystem
    {
        public static readonly MediaSystem Instance = new();

        static readonly object _exeCSX = new();
        static readonly string[] _wrongMedia = new string[]
        {
            "ed7f217838d78942898e53d5dbee64ec" // Celestial Axes
        };

        readonly ConcurrentDictionary<IMediaContainer, ConcurrentDictionary<string, HandledMediaItem>> _mediaMap = new();
        readonly ConcurrentDictionary<IMediaHandler, ConcurrentBag<MediaHandlerItem>> _mediaHandlerMap = new();

        public int MediaItemCount => _mediaMap.Values.Sum(mediaItems => mediaItems.Count);

        public int MediaHandlerItemCount => _mediaHandlerMap.Values.Sum(mediaHandlerItems => mediaHandlerItems.Count);

        public void Pause(IMediaHandler mediaHandler, bool isPaused)
        {
            if (Configure.Instance.Media && _mediaHandlerMap.TryGetValue(mediaHandler, out var mediaHandlerItems))
            {
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    if (ViewModels.Instance.MainValue.IsWPFViewVisible)
                    {
                        mediaHandlerItem.PauseDefault(isPaused);
                    }
                    else
                    {
                        mediaHandlerItem.Pause(isPaused);
                    }
                }
            }
        }

        public void SetAudioMultiplier(IMediaHandler mediaHandler)
        {
            if (Configure.Instance.Media && _mediaHandlerMap.TryGetValue(mediaHandler, out var mediaHandlerItems))
            {
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    if (ViewModels.Instance.MainValue.IsWPFViewVisible)
                    {
                        HandlingUISystem.Instance.HandleParallel(() => mediaHandlerItem.HandledMediaItem.DefaultMedia.SpeedRatio = mediaHandler.AudioMultiplier);
                    }
                    else
                    {
                        mediaHandlerItem.HandledMediaItem.Media.PlaybackSession.PlaybackRate = mediaHandler.AudioMultiplier;
                    }
                }
            }
        }

        public void SetMediaPosition(IMediaHandler mediaHandler)
        {
            if (Configure.Instance.Media && _mediaHandlerMap.TryGetValue(mediaHandler, out var mediaHandlerItems))
            {
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    if (mediaHandlerItem.IsVisible && !mediaHandlerItem.HandledMediaItem.IsLooping)
                    {
                        if (ViewModels.Instance.MainValue.IsWPFViewVisible)
                        {
                            if (mediaHandlerItem.IsDefaultHandling)
                            {
                                mediaHandlerItem.SetDefaultMediaPosition(mediaHandlerItem.GetMediaPosition(mediaHandler));
                            }
                        }
                        else
                        {
                            if (mediaHandlerItem.IsHandling)
                            {
                                mediaHandlerItem.SetMediaPosition(mediaHandlerItem.GetMediaPosition(mediaHandler));
                            }
                        }
                    }
                }
            }
        }

        public HandledMediaItem Load(string mediaFilePath, IMediaContainer mediaContainer, bool isLooping)
        {
            var isCounterWave = mediaContainer.IsCounterWave;
            var hash = $"{(isCounterWave ? '@' : string.Empty)}{Utility.GetID128s(File.ReadAllBytes(mediaFilePath))}";
            if (_mediaMap.TryGetValue(mediaContainer, out var handledMediaItems) && handledMediaItems.TryGetValue(hash, out var handledMediaItem))
            {
                return handledMediaItem;
            }

            var mediaSrc = FFmpegMediaSource.CreateFromFileAsync(mediaFilePath).Await();
            var mediaLength = mediaSrc.Duration;
            var mediaModifierValue = mediaContainer.MediaModifierValue;
            if (mediaModifierValue != null)
            {
                lock (_exeCSX)
                {
                    var hashFilePath = Utility.GetAvailable(Path.Combine(QwilightComponent.MediaEntryPath, hash), Utility.AvailableFlag.Media);
                    if (File.Exists(hashFilePath))
                    {
                        mediaFilePath = hashFilePath;
                    }
                    else
                    {
                        var isWrongMedia = Array.IndexOf(_wrongMedia, hash) != -1 || mediaLength < TimeSpan.FromMinutes(1.0);
                        var hasAudio = mediaSrc.AudioStreams.Count > 0;
                        if (isWrongMedia || hasAudio || isCounterWave)
                        {
                            mediaSrc.Dispose();
                            mediaSrc = null;
                            hashFilePath = Path.ChangeExtension(Path.Combine(QwilightComponent.MediaEntryPath, hash), isWrongMedia || isCounterWave ? ".mkv" : Path.GetExtension(mediaFilePath));
                            mediaModifierValue.ModifyMedia(mediaFilePath, hashFilePath, isWrongMedia, isCounterWave);
                            mediaFilePath = hashFilePath;
                        }
                    }
                }
            }
            mediaSrc ??= FFmpegMediaSource.CreateFromFileAsync(mediaFilePath).Await();
            mediaLength = mediaSrc.Duration;
            handledMediaItem = new()
            {
                MediaSrc = mediaSrc,
                Media = new()
                {
                    Source = mediaSrc.CreateMediaPlaybackItem(),
                    IsMuted = true,
                    IsVideoFrameServerEnabled = true,
                    IsLoopingEnabled = isLooping
                },
                MediaFilePath = mediaFilePath,
                Length = mediaLength.TotalMilliseconds,
                IsLooping = isLooping
            };
            handledMediaItem.Media.CommandManager.IsEnabled = false;
            handledMediaItem.Media.SystemMediaTransportControls.IsEnabled = false;
            _mediaMap.AddOrUpdate(mediaContainer, (mediaContainer, handledMediaItem) => new(new[] { KeyValuePair.Create(hash, handledMediaItem) }), (mediaContainer, handledMediaItems, handledMediaItem) =>
            {
                handledMediaItems[hash] = handledMediaItem;
                return handledMediaItems;
            }, handledMediaItem);
            return handledMediaItem;
        }

        public void Stop(IMediaHandler mediaHandler)
        {
            if (_mediaHandlerMap.TryRemove(mediaHandler, out var mediaHandlerItems))
            {
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    mediaHandlerItem.Stop();
                    mediaHandlerItem.StopDefault();
                }
            }
        }

        public MediaHandlerItem Handle(HandledMediaItem handledMediaItem, IMediaHandler mediaHandler, TimeSpan levyingWait, MediaNote.Mode mode)
        {
            var mediaHandlerItem = new MediaHandlerItem
            {
                HandledMediaItem = handledMediaItem,
                LevyingPosition = levyingWait,
                Mode = mode,
                IsHandling = false,
                IsDefaultHandling = false,
                IsVisible = false
            };
            _mediaHandlerMap.AddOrUpdate(mediaHandler, (mediaHandler, t) => new()
            {
                t.mediaHandlerItem
            }, (mediaHandler, mediaHandlerItems, t) =>
            {
                var mode = t.mode;
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    if (mediaHandlerItem.IsVisible)
                    {
                        if (mediaHandlerItem.Mode == mode)
                        {
                            if (mediaHandlerItem.IsDefaultHandling)
                            {
                                mediaHandlerItem.StopDefault();
                            }
                            if (mediaHandlerItem.IsHandling)
                            {
                                mediaHandlerItem.Stop();
                            }
                        }
                        t.mediaHandlerItem.IsVisible = false;
                    }
                }
                mediaHandlerItems.Add(mediaHandlerItem);
                return mediaHandlerItems;
            }, (mediaHandlerItem, mode));

            if (Configure.Instance.Media)
            {
                var mediaPosition = mediaHandlerItem.GetMediaPosition(mediaHandler);
                if (ViewModels.Instance.MainValue.IsNoteFileMode)
                {
                    mediaHandlerItem.HandleDefault(mediaHandler, mediaPosition);
                }
                else
                {
                    mediaHandlerItem.Handle(mediaHandler, mediaPosition);
                }
            }

            mediaHandlerItem.IsVisible = true;
            return mediaHandlerItem;
        }

        public MediaHandlerItem Handle(HandledMediaItem handledMediaItem, IMediaHandler mediaHandler, bool isAvailable, bool isDefaultAvailable)
        {
            var mediaHandlerItem = new MediaHandlerItem
            {
                HandledMediaItem = handledMediaItem,
                LevyingPosition = TimeSpan.Zero,
                Mode = MediaNote.Mode.Default,
                IsAvailable = isAvailable,
                IsDefaultAvailable = isDefaultAvailable
            };
            _mediaHandlerMap.AddOrUpdate(mediaHandler, (mediaHandler, mediaHandlerItem) => new()
            {
                mediaHandlerItem
            }, (mediaHandler, mediaHandlerItems, mediaHandlerItem) =>
            {
                mediaHandlerItems.Add(mediaHandlerItem);
                return mediaHandlerItems;
            }, mediaHandlerItem);

            var mainViewModel = ViewModels.Instance.MainValue;
            if (mainViewModel.IsNoteFileMode && isDefaultAvailable)
            {
                mediaHandlerItem.HandleDefault(mediaHandler, 0.0);
            }
            if (mainViewModel.IsQuitMode && isAvailable)
            {
                mediaHandlerItem.Handle(mediaHandler, 0.0);
            }

            mediaHandlerItem.IsVisible = true;
            return mediaHandlerItem;
        }

        public void HandleIfAvailable(IMediaHandler mediaHandler)
        {
            if (!mediaHandler.IsPausing && _mediaHandlerMap.TryGetValue(mediaHandler, out var mediaHandlerItems))
            {
                var mainViewModel = ViewModels.Instance.MainValue;
                var isNoteFileMode = mainViewModel.IsNoteFileMode;
                var isQuitMode = mainViewModel.IsQuitMode;
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    if (mediaHandlerItem.IsVisible)
                    {
                        if (!isNoteFileMode && (mediaHandler == BaseUI.Instance ? isQuitMode && mediaHandlerItem.IsAvailable : Configure.Instance.Media))
                        {
                            if (!mediaHandlerItem.IsHandling)
                            {
                                mediaHandlerItem.Handle(mediaHandler, mediaHandlerItem.GetMediaPosition(mediaHandler));
                            }
                        }
                        else
                        {
                            if (mediaHandlerItem.IsHandling)
                            {
                                mediaHandlerItem.Stop();
                            }
                        }
                    }
                }
            }
        }

        public void HandleDefaultIfAvailable(IMediaHandler mediaHandler)
        {
            if (!mediaHandler.IsPausing && _mediaHandlerMap.TryGetValue(mediaHandler, out var mediaHandlerItems))
            {
                var isNoteFileMode = ViewModels.Instance.MainValue.IsNoteFileMode;
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    if (mediaHandlerItem.IsVisible)
                    {
                        if (isNoteFileMode && (mediaHandler == BaseUI.Instance ? mediaHandlerItem.IsDefaultAvailable : Configure.Instance.Media))
                        {
                            if (!mediaHandlerItem.IsDefaultHandling)
                            {
                                mediaHandlerItem.HandleDefault(mediaHandler, mediaHandlerItem.GetMediaPosition(mediaHandler));
                            }
                        }
                        else
                        {
                            if (mediaHandlerItem.IsDefaultHandling)
                            {
                                mediaHandlerItem.StopDefault();
                            }
                        }
                    }
                }
            }
        }

        public void Close(IMediaContainer mediaContainer, IMediaHandler mediaHandler)
        {
            if (_mediaMap.TryRemove(mediaContainer, out var mediaItems))
            {
                foreach (var mediaItem in mediaItems)
                {
                    mediaItem.Value.Dispose();
                }
            }
            if (_mediaHandlerMap.TryRemove(mediaHandler, out var mediaHandlerItems))
            {
                foreach (var mediaHandlerItem in mediaHandlerItems)
                {
                    mediaHandlerItem.Dispose();
                }
            }
        }

        public void Migrate(IMediaContainer src, IMediaContainer target)
        {
            if (_mediaMap.TryRemove(src, out var audioItems))
            {
                _mediaMap[target] = audioItems;
            }
        }
    }
}