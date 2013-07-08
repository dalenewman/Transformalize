using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace Transformalize.Rhino.Etl.Core {
    /// <summary>
    /// Represent a virtual row
    /// </summary>
    [DebuggerDisplay("Count = {items.Count}")]
    [Serializable]
    public class Row : QuackingDictionary, IEquatable<Row> {
        static readonly ConcurrentDictionary<Type, List<PropertyInfo>> PropertiesCache = new ConcurrentDictionary<Type, List<PropertyInfo>>();
        static readonly ConcurrentDictionary<Type, List<FieldInfo>> FieldsCache = new ConcurrentDictionary<Type, List<FieldInfo>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        public Row()
            : base(new ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <param name="itemsToClone">The items to clone.</param>
        protected Row(IDictionary<string, object> itemsToClone) : base(new ConcurrentDictionary<string, object>(itemsToClone, StringComparer.InvariantCultureIgnoreCase)) {
        }


        /// <summary>
        /// Creates a copy of the given source, erasing whatever is in the row currently.
        /// </summary>
        /// <param name="source">The source row.</param>
        public void Copy(IDictionary<string, object> source) {
            Items = new ConcurrentDictionary<string, object>(source, StringComparer.InvariantCultureIgnoreCase);
        }

        public IEnumerable<string> Columns {
            get {
                return new List<string>(Items.Keys);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Row Clone() {
            return new Row(this);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Row" /> is equal to another <see cref="Row" />.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Row other) {
            if (Columns.SequenceEqual(other.Columns, StringComparer.InvariantCultureIgnoreCase) == false)
                return false;

            foreach (var key in Items.Keys) {
                var item = Items[key];
                var otherItem = other.Items[key];

                if (item == null | otherItem == null)
                    return item == null & otherItem == null;

                var equalityComparer = CreateComparer(item.GetType(), otherItem.GetType());

                if (equalityComparer(item, otherItem) == false)
                    return false;
            }

            return true;
        }

        private static Func<object, object, bool> CreateComparer(Type firstType, Type secondType) {
            if (firstType == secondType)
                return Equals;

            var firstParameter = Expression.Parameter(typeof(object), "first");
            var secondParameter = Expression.Parameter(typeof(object), "second");

            var equalExpression = Expression.Equal(Expression.Convert(firstParameter, firstType),
                Expression.Convert(Expression.Convert(secondParameter, secondType), firstType));

            return Expression.Lambda<Func<object, object, bool>>(equalExpression, firstParameter, secondParameter).Compile();
        }

        /// <summary>
        /// Creates a key from the current row, suitable for use in hashtables
        /// </summary>
        public ObjectArrayKeys CreateKey() {
            return CreateKey(Columns.ToArray());
        }

        /// <summary>
        /// Creates a key that allow to do full or partial indexing on a row
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public ObjectArrayKeys CreateKey(string[] columns) {
            var array = new object[columns.Length];
            for (var i = 0; i < columns.Length; i++) {
                array[i] = Items[columns[i]];
            }
            return new ObjectArrayKeys(array);
        }

        /// <summary>
        /// Copy all the public properties and fields of an object to the row
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static Row FromObject(object obj) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            var row = new Row();
            foreach (var property in GetProperties(obj)) {
                row[property.Name] = property.GetValue(obj, new object[0]);
            }
            foreach (var field in GetFields(obj)) {
                row[field.Name] = field.GetValue(obj);
            }
            return row;
        }

        private static IEnumerable<PropertyInfo> GetProperties(object obj) {
            List<PropertyInfo> properties;
            if (PropertiesCache.TryGetValue(obj.GetType(), out properties))
                return properties;

            properties = new List<PropertyInfo>();
            foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)) {
                if (property.CanRead == false || property.GetIndexParameters().Length > 0)
                    continue;
                properties.Add(property);
            }
            PropertiesCache[obj.GetType()] = properties;
            return properties;
        }

        private static IEnumerable<FieldInfo> GetFields(object obj) {
            List<FieldInfo> fields;
            if (FieldsCache.TryGetValue(obj.GetType(), out fields))
                return fields;

            fields = new List<FieldInfo>();
            foreach (var fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)) {
                if (Attribute.IsDefined(fieldInfo, typeof(CompilerGeneratedAttribute)) == false) {
                    fields.Add(fieldInfo);
                }
            }
            FieldsCache[obj.GetType()] = fields;
            return fields;
        }

        /// <summary>
        /// Generate a row from the reader
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static Row FromReader(IDataReader reader) {
            var row = new Row();
            var count = reader.FieldCount;
            for (var i = 0; i < count; i++) {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            return row;
        }

        /// <summary>
        /// Create a new object of <typeparamref name="T"/> and set all
        /// the matching fields/properties on it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToObject<T>() {
            return (T)ToObject(typeof(T));
        }

        /// <summary>
        /// Create a new object of <param name="type"/> and set all
        /// the matching fields/properties on it.
        /// </summary>
        public object ToObject(Type type) {
            var instance = Activator.CreateInstance(type);
            foreach (var info in GetProperties(instance)) {
                if (Items.ContainsKey(info.Name) && info.CanWrite)
                    info.SetValue(instance, Items[info.Name], null);
            }
            foreach (var info in GetFields(instance)) {
                if (Items.ContainsKey(info.Name))
                    info.SetValue(instance, Items[info.Name]);
            }
            return instance;
        }
    }
}
