﻿using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows;

namespace Igniter.MSG
{
    public sealed class ViewAllowWindow : AsyncRequestMessage<MessageBoxResult>
    {
        public string Text { get; set; }

        public MessageBoxButton Input { get; set; }

        public MessageBoxImage Drawing { get; set; }
    }
}