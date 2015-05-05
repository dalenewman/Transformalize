using System;

namespace Transformalize.Logging {

    /// <summary>
    /// I don't log anything!
    /// </summary>
    public class NullLogger : ILogger {

        public string Name { get; set; }

        public void Info(string message, params object[] args)
        {
        }

        public void Debug(string message, params object[] args)
        {
            
        }

        public void Warn(string message, params object[] args)
        {
            
        }

        public void Error(string message, params object[] args)
        {
            
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            
        }

        public void EntityInfo(string entity, string message, params object[] args)
        {
            
        }

        public void EntityDebug(string entity, string message, params object[] args)
        {
            
        }

        public void EntityWarn(string entity, string message, params object[] args)
        {
            
        }

        public void EntityError(string entity, string message, params object[] args)
        {
            
        }

        public void EntityError(string entity, Exception exception, string message, params object[] args)
        {
            
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}