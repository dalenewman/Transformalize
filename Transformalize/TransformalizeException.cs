using System;

namespace Transformalize {
    public class TransformalizeException : Exception {
        private readonly string _message;

        public TransformalizeException(string message) {
            _message = message;
        }

        public override string Message {
            get { return _message; }
        }
    }
}
