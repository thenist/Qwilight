﻿using CommunityToolkit.Mvvm.Messaging;
using Qwilight.Compiler;
using Qwilight.Compute;
using Qwilight.MSG;
using Qwilight.NoteFile;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Collections.Concurrent;
#if DEBUG
using System.Diagnostics;
#endif
using System.IO;
using System.Text;

namespace Qwilight
{
    public sealed class DefaultTelnetSystem
    {
        readonly IHandleTelnet _handleTelnet;
        bool _isAvailable = true;

        public DefaultTelnetSystem(IHandleTelnet handleTelnet)
        {
            _handleTelnet = handleTelnet;
        }

        public void HandleSystem() => Utility.HandleParallelly(() =>
        {
            while (_isAvailable)
            {
                try
                {
                    Console.Clear();
#if DEBUG
                    Console.WriteLine($"D: {Debugger.IsAttached}");
#endif
                    Console.WriteLine($"H: {QwilightComponent.TaehuiNetDDNS}");
                    Console.WriteLine($"S: {_handleTelnet.IsAlwaysNewStand}");
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.Escape:
                            _handleTelnet.Toggle();
                            break;
#if DEBUG
                        case ConsoleKey.D:
                            Debugger.Launch();
                            break;
#endif
                        case ConsoleKey.E:
                            Console.Clear();
                            Console.WriteLine("1, 2, 3, 4, 5, 6, 7, 8, 9");
                            switch (Console.ReadKey(true).Key)
                            {
                                case ConsoleKey.D1:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.LevelUp);
                                    break;
                                case ConsoleKey.D2:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.LevelClear);
                                    break;
                                case ConsoleKey.D3:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.LogIn);
                                    break;
                                case ConsoleKey.D4:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.NotLogIn);
                                    break;
                                case ConsoleKey.D5:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.ModifyEntryItem);
                                    break;
                                case ConsoleKey.D6:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.ModifyNoteFile);
                                    break;
                                case ConsoleKey.D7:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.NewTitle);
                                    break;
                                case ConsoleKey.D8:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.AbilityUp);
                                    break;
                                case ConsoleKey.D9:
                                    BaseUI.Instance.HandleEvent(BaseUI.EventItem.AbilityClassUp);
                                    break;
                            }
                            break;
                        case ConsoleKey.H:
                            Console.Clear();
                            Console.WriteLine("1. taehui.ddns.net");
                            Console.WriteLine("2. taehui");
                            Console.WriteLine("3. localhost");
                            switch (Console.ReadKey(true).Key)
                            {
                                case ConsoleKey.D1:
                                    QwilightComponent.SetDDNS("taehui.ddns.net");
                                    break;
                                case ConsoleKey.D2:
                                    QwilightComponent.SetDDNS("taehui");
                                    break;
                                case ConsoleKey.D3:
                                    QwilightComponent.SetDDNS("localhost");
                                    break;
                                default:
                                    continue;
                            }
                            TwilightSystem.Instance.Stop();
                            break;
                        case ConsoleKey.G:
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, $"{Utility.FormatLength(GC.GetTotalMemory(false))} => {Utility.FormatLength(GC.GetTotalMemory(true))}");
                            break;
                        case ConsoleKey.N:
                            Console.Clear();
                            Console.WriteLine("1, 2, 3, 4");
                            switch (Console.ReadKey(true).Key)
                            {
                                case ConsoleKey.D1:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, Guid.NewGuid().ToString());
                                    break;
                                case ConsoleKey.D2:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, Guid.NewGuid().ToString());
                                    break;
                                case ConsoleKey.D3:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, Guid.NewGuid().ToString());
                                    break;
                                case ConsoleKey.D4:
                                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, Guid.NewGuid().ToString());
                                    break;
                            }
                            break;
                        case ConsoleKey.S:
                            _handleTelnet.IsAlwaysNewStand = !_handleTelnet.IsAlwaysNewStand;
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, _handleTelnet.IsAlwaysNewStand.ToString());
                            break;
                        case ConsoleKey.T:
                            var entryPath = StrongReferenceMessenger.Default.Send<ViewEntryWindow>();
                            if (!string.IsNullOrEmpty(entryPath))
                            {
                                ViewModels.Instance.MainValue.ModeComponentValue.Salt = 0;
                                Utility.HandleLowestlyParallelly(new ConcurrentBag<BaseNoteFile>(ViewModels.Instance.MainValue.EntryItems.SelectMany(entryItem => entryItem.WellNoteFiles.Where(noteFile => !noteFile.IsLogical))), Configure.Instance.UIBin, noteFile =>
                                {
                                    Console.WriteLine(noteFile.NoteFilePath);
                                    var targetCompiler = BaseCompiler.GetCompiler(noteFile, null);
                                    var defaultComputer = new DefaultCompute([noteFile], null, null, string.Empty, string.Empty)
                                    {
                                        IsSilent = true
                                    };
                                    targetCompiler.Compile(defaultComputer, false);
                                    var noteID = noteFile.GetNoteID512();
                                    noteID = noteID.Substring(0, noteID.IndexOf(':'));
                                    File.WriteAllBytes(Path.Combine(entryPath, Path.ChangeExtension(noteID, Path.GetExtension(noteFile.NoteFilePath))), noteFile.GetContents());
                                    var builder = new StringBuilder();
                                    builder.AppendLine(defaultComputer.IsAutoLongNote.ToString());
                                    builder.AppendLine(defaultComputer.IsBanned.ToString());
                                    builder.AppendLine(defaultComputer.InputMode.ToString());
                                    builder.AppendLine(defaultComputer.Genre);
                                    builder.AppendLine(defaultComputer.Artist);
                                    builder.AppendLine(defaultComputer.Title);
                                    builder.AppendLine(defaultComputer.LevelText);
                                    builder.AppendLine(defaultComputer.LevelTextValue.ToString());
                                    builder.AppendLine(defaultComputer.LevyingBPM.ToString());
                                    builder.AppendLine(defaultComputer.BPM.ToString());
                                    builder.AppendLine(defaultComputer.Length.ToString());
                                    builder.AppendLine(defaultComputer.TotalNotes.ToString());
                                    builder.AppendLine(defaultComputer.AutoableNotes.ToString());
                                    builder.AppendLine(defaultComputer.TrapNotes.ToString());
                                    builder.AppendLine(defaultComputer.LongNotes.ToString());
                                    builder.AppendLine(defaultComputer.JudgmentStage.ToString());
                                    builder.AppendLine(defaultComputer.HitPointsValue.ToString());
                                    builder.AppendLine(defaultComputer.LevelValue.ToString());
                                    builder.AppendLine(defaultComputer.NoteDrawingName);
                                    builder.AppendLine(defaultComputer.BannerDrawingName);
                                    builder.AppendLine(defaultComputer.TrailerAudioName);
                                    builder.AppendLine(defaultComputer.AudioLevyingPosition.ToString());
                                    builder.AppendLine(defaultComputer.IsSalt.ToString());
                                    builder.AppendLine(defaultComputer.Tag);
                                    builder.AppendLine(defaultComputer.LowestBPM.ToString());
                                    builder.AppendLine(defaultComputer.HighestBPM.ToString());
                                    builder.AppendLine(defaultComputer.HighestInputCount.ToString());
                                    builder.AppendLine(defaultComputer.IsHellBPM.ToString());
                                    File.WriteAllText(Path.Combine(entryPath, Path.ChangeExtension(noteID, ".txt")), builder.ToString(), Encoding.UTF8);
                                });
                                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, "OK");
                            }
                            break;
                    }
                }
                catch
                {
                }
            }
        }, false);

        public void Stop()
        {
            _isAvailable = false;
        }
    }
}