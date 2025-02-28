﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Data.Sqlite;
using Qwilight.MSG;
using Qwilight.NoteFile;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class EventNoteViewModel : BaseViewModel
    {
        string _eventNoteName;
        BaseNoteFile _noteFile;

        public override double TargetLength => 0.8;

        public override double TargetHeight => 0.6;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public ObservableCollection<BaseNoteFile> NoteFileCollection { get; } = new();

        public BaseNoteFile NoteFile
        {
            get => _noteFile;

            set => SetProperty(ref _noteFile, value, nameof(NoteFile));
        }

        public string EventNoteName
        {
            get => _eventNoteName;

            set => SetProperty(ref _eventNoteName, value, nameof(EventNoteName));
        }

        public void OnNoteFileView(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up when NoteFile != null:
                    var i = NoteFileCollection.IndexOf(NoteFile);
                    if (i > 0)
                    {
                        NoteFileCollection.Move(i, i - 1);
                    }
                    StrongReferenceMessenger.Default.Send(new PointEventNoteView
                    {
                        Target = NoteFile
                    });
                    e.Handled = true;
                    break;
                case Key.Down when NoteFile != null:
                    i = NoteFileCollection.IndexOf(NoteFile);
                    if (i < NoteFileCollection.Count - 1)
                    {
                        NoteFileCollection.Move(i, i + 1);
                    }
                    StrongReferenceMessenger.Default.Send(new PointEventNoteView
                    {
                        Target = NoteFile
                    });
                    e.Handled = true;
                    break;
                case Key.Delete when NoteFile != null:
                    i = NoteFileCollection.IndexOf(NoteFile);
                    NoteFileCollection.RemoveAt(i);
                    if (i < NoteFileCollection.Count)
                    {
                        NoteFile = NoteFileCollection[i];
                    }
                    StrongReferenceMessenger.Default.Send(new PointEventNoteView
                    {
                        Target = NoteFile
                    });
                    break;
            }
        }

        public void OnInputLower(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    var eventNoteID = string.Join('/', NoteFileCollection.Select(noteFile => noteFile.GetNoteID512()));
                    var date = DateTime.Now;
                    DB.Instance.SetEventNote(eventNoteID, EventNoteName, date, DB.EventNoteVariety.Qwilight);
                    Close();
                    NoteFileCollection.Clear();
                    EventNoteName = null;
                    var mainViewModel = ViewModels.Instance.MainValue;
                    mainViewModel.LoadEventNoteEntryItems();
                    mainViewModel.Want();
                }
                catch (SqliteException)
                {
                    NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.BeforeEventNoteContents);
                }
            }
        }
    }
}