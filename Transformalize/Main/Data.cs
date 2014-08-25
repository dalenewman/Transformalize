using System;

namespace Transformalize.Main {

    public class Data {

        private string _string;
        private short _short;
        private int _int;
        private long _long;
        private UInt64 _uInt64;
        private double _double;
        private decimal _decimal;
        private char _char;
        private DateTime _dateTime;
        private bool _bool;
        private Single _single;
        private Guid _guid;
        private byte _byte;
        private byte[] _binary;
        private object _object;

        public string String {
            get { return _string; }
            set { _object = _string = value; }
        }

        public short Short {
            get { return _short; }
            set { _object = _short = value; }
        }

        public int Int {
            get { return _int; }
            set { _object = _int = value; }
        }

        public long Long {
            get { return _long; }
            set { _object = _long = value; }
        }

        public ulong UInt64 {
            get { return _uInt64; }
            set { _object = _uInt64 = value; }
        }

        public double Double {
            get { return _double; }
            set { _object = _double = value; }
        }

        public decimal Decimal {
            get { return _decimal; }
            set { _object = _decimal = value; }
        }

        public char Char {
            get { return _char; }
            set { _object = _char = value; }
        }

        public DateTime DateTime {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        public bool Bool {
            get { return _bool; }
            set { _object = _bool = value; }
        }

        public float Single {
            get { return _single; }
            set { _object = _single = value; }
        }

        public Guid Guid {
            get { return _guid; }
            set { _object = _guid = value; }
        }

        public byte Byte {
            get { return _byte; }
            set { _object = _byte = value; }
        }

        public byte[] Binary {
            get { return _binary; }
            set { _object = _binary = value; }
        }

        /// <summary>
        /// Only use this if you don't know the type.
        /// Prefer the typed properties (String, Bool, Byte, etc.).
        /// </summary>
        public object Object {
            get { return _object; }
            set { _object = value; }
        }

    }
}