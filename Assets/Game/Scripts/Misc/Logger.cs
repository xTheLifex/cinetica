using UnityEngine;

namespace Cinetica.Utility
{
    public class Logger
    {
        public string prefix;
        public Logger(string prefix) => this.prefix = prefix;
        private string FormattedMessage(string message) => $"[{prefix}]: {message}";
        
        public void Log(string message) { Debug.Log(FormattedMessage(message)); }
        public void LogWarning(string message) { Debug.LogWarning(FormattedMessage(message)); }
        public void LogError(string message) { Debug.LogError(FormattedMessage(message)); }

        public void Log(string message, Object context) { Debug.Log(FormattedMessage(message), context); }
        public void LogWarning(string message, Object context) { Debug.LogWarning(FormattedMessage(message), context); }
        public void LogError(string message, Object context) { Debug.LogError(FormattedMessage(message), context); }
    }
}