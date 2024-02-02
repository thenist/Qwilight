using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Qwilight
{
    public sealed class MediaSystem
    {
        public static readonly MediaSystem Instance = new();

        static readonly object _exeCSX = new();
        static readonly string[] _validMedia = new string[]
        {
            "e82d6a96a58c9a01098fa4a53f95c5ad"
        };
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
                        UIHandler.Instance.HandleParallel(() => mediaHandlerItem.HandledMediaItem.DefaultMedia.SpeedRatio = mediaHandler.AudioMultiplier);
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
            var hash = $"{(isCounterWave ? '@' : string.Empty)}{Utility.GetID128(File.ReadAllBytes(mediaFilePath))}";
            if (_mediaMap.TryGetValue(mediaContainer, out var handledMediaItems) && handledMediaItems.TryGetValue(hash, out var handledMediaItem))
            {
                return handledMediaItem;
            }

            var mediaModifierValue = mediaContainer.MediaModifierValue;
            if (mediaModifierValue != null)
            {
                lock (_exeCSX)
                {
                    var hashFilePath = Utility.GetFilePath(Path.Combine(QwilightComponent.MediaEntryPath, hash), Utility.FileFormatFlag.Media);
                    if (File.Exists(hashFilePath))
                    {
                        mediaFilePath = hashFilePath;
                    }
                    else
                    {
                        var isWrongMedia = Array.IndexOf(_wrongMedia, hash) != -1 || Array.IndexOf(_validMedia, hash) == -1;
                        var hasAudio = HasAudio(mediaFilePath);
                        if (isWrongMedia || hasAudio || isCounterWave)
                        {
                            hashFilePath = Path.ChangeExtension(Path.Combine(QwilightComponent.MediaEntryPath, hash), isWrongMedia || isCounterWave ? ".mkv" : Path.GetExtension(mediaFilePath));
                            mediaModifierValue.ModifyMedia(mediaFilePath, hashFilePath, isWrongMedia, isCounterWave);
                            mediaFilePath = hashFilePath;
                        }
                    }
                }
            }
            handledMediaItem = new()
            {
                Media = new()
                {
                    Source = new MediaPlaybackItem(MediaSource.CreateFromUri(new(mediaFilePath))),
                    IsMuted = true,
                    IsVideoFrameServerEnabled = true,
                    IsLoopingEnabled = isLooping
                },
                MediaFilePath = mediaFilePath,
                Length = GetMediaLength(mediaFilePath),
                IsLooping = isLooping
            };
            handledMediaItem.Media.CommandManager.IsEnabled = false;
            handledMediaItem.Media.SystemMediaTransportControls.IsEnabled = false;
            _mediaMap.AddOrUpdate(mediaContainer, (mediaContainer, handledMediaItem) => new([KeyValuePair.Create(hash, handledMediaItem)]), (mediaContainer, handledMediaItems, handledMediaItem) =>
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

        static double GetMediaLength(string fileName)
        {
            var mediaLength = 0.0;
            using (var exe = new Process
            {
                StartInfo = new(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "ffprobe.exe"), $"""
                    -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 "{fileName}"
                """)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            })
            {
                exe.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Utility.ToFloat64(e.Data, out mediaLength);
                    }
                };
                exe.Start();
                exe.PriorityClass = ProcessPriorityClass.Idle;
                exe.BeginOutputReadLine();
                exe.WaitForExit();
            }
            return 1000.0 * mediaLength;
        }

        static bool HasAudio(string fileName)
        {
            var hasAudio = false;
            using (var exe = new Process
            {
                StartInfo = new(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "ffprobe.exe"), $"""
                    -v error -show_entries stream=codec_type -of default=noprint_wrappers=1:nokey=1 "{fileName}"
                """)
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            })
            {
                exe.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        hasAudio |= e.Data.Contains("audio");
                    }
                };
                exe.Start();
                exe.PriorityClass = ProcessPriorityClass.Idle;
                exe.BeginOutputReadLine();
                exe.WaitForExit();
            }
            return hasAudio;
        }
    }
}