﻿using Qwilight.Utilities;
using Qwilight.ViewModel;

namespace Qwilight
{
    public sealed class NotifySystem
    {
        public const int ModeComponentID = -1;
        public const int SetFavoritesID = -2;
        public const int LoadUIID = -3;
        public const int SaveLevelID = -4;
        public const int SaveQwilightID = -5;
        public const int SaveBundleID = -6;
        public const int SaveAsBundleID = -7;
        public const int SaveFileID = -8;
        public const int SaveUIID = -9;

        public enum NotifyVariety
        {
            OK, Fault, Warning, Info, Levying, Stopped, Quit
        }

        public enum NotifyConfigure
        {
            Default, Save, NotSave
        }

        public static readonly NotifySystem Instance = new();

        int _toNotifyID;

        public void NotifyPending() => UIHandler.Instance.HandleParallel(() =>
        {
            var toNotifyCount = ViewModels.Instance.NotifyValue.NotifyItemCollection.Count(toNotifyItem =>
            {
                var isNew = toNotifyItem.IsNew;
                toNotifyItem.IsNew = false;
                return isNew;
            });
            if (toNotifyCount > 0)
            {
                Notify(NotifyVariety.Info, NotifyConfigure.NotSave, string.Format(LanguageSystem.Instance.WaitingNotifyContents, toNotifyCount));
            }
        });

        public void Notify(NotifyVariety toNotifyVariety, NotifyConfigure toNotifyConfigure, string toNotify, bool allowComputingMode = true, string audioAlt = null, Action onHandle = null, int? toNotifyID = null)
        {
            UIHandler.Instance.HandleParallel(() =>
            {
                switch (toNotifyConfigure)
                {
                    case NotifyConfigure.Default:
                        Save(toNotifyVariety, toNotify, !Notify());
                        break;
                    case NotifyConfigure.Save:
                        Save(toNotifyVariety, toNotify, true);
                        break;
                    case NotifyConfigure.NotSave:
                        Notify();
                        break;
                }
            });

            bool Notify()
            {
                if (allowComputingMode || !ViewModels.Instance.MainValue.IsComputingMode)
                {
                    if (audioAlt == null || audioAlt.Length > 0)
                    {
                        switch (toNotifyVariety)
                        {
                            case NotifyVariety.OK:
                                Utility.HandleUIAudio(audioAlt ?? "Notify OK", "Notify");
                                break;
                            case NotifyVariety.Info:
                                Utility.HandleUIAudio(audioAlt ?? "Notify", "Notify");
                                break;
                            case NotifyVariety.Fault:
                                Utility.HandleUIAudio(audioAlt ?? "Notify Fault", "Notify");
                                break;
                            case NotifyVariety.Warning:
                                Utility.HandleUIAudio(audioAlt ?? "Notify Warning", "Notify");
                                break;
                        }
                    }
                    ViewModels.Instance.NotifyXamlValue.NewNotify(new()
                    {
                        Variety = toNotifyVariety,
                        Paint = toNotifyVariety switch
                        {
                            NotifyVariety.OK => Paints.PaintOK,
                            NotifyVariety.Info => Paints.PaintInfo,
                            NotifyVariety.Fault => Paints.PaintFault,
                            NotifyVariety.Warning => Paints.PaintWarning,
                            _ => default
                        },
                        Color = toNotifyVariety switch
                        {
                            NotifyVariety.OK => Paints.ColorOK,
                            NotifyVariety.Info => Paints.ColorInfo,
                            NotifyVariety.Fault => Paints.ColorFault,
                            NotifyVariety.Warning => Paints.ColorWarning,
                            _ => default
                        },
                        Contents = toNotify,
                        OnHandle = onHandle,
                        ID = toNotifyID ?? ++_toNotifyID
                    });
                    return true;
                }
                return false;
            }

            void Save(NotifyVariety toNotifyVariety, string toNotify, bool isNew)
            {
                var toNotifyCollection = ViewModels.Instance.NotifyValue.NotifyItemCollection;
                if (toNotifyCollection.Count == 100)
                {
                    toNotifyCollection.RemoveAt(99);
                }
                toNotifyCollection.Insert(0, new()
                {
                    Variety = toNotifyVariety,
                    Text = toNotify,
                    IsNew = isNew,
                    OnHandle = onHandle
                });
            }
        }
    }
}