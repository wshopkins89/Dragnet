using System;

namespace DragnetControl.Configuration
{
    public sealed class ConfigurationProgress
    {
        public ConfigurationProgress(string message, int value)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Value = value;
        }

        public string Message { get; }
        public int Value { get; }
    }
}

