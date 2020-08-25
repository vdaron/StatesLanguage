using System;
using Newtonsoft.Json;
using StatesLanguage.Model.Serialization;

namespace StatesLanguage.Model.States
{
    [JsonConverter(typeof(OptionalStringSerializer))]
    public struct OptionalString
    {
        private string _value;
        private bool _isSet;

        /// <summary>
        /// Standard constructor.
        /// </summary>
        /// <param name="value"></param>
        public OptionalString(string value)
        {
            _isSet = true;
            _value = value;
        }

        /// <summary>
        /// True if a value has been set, even if it is null.
        /// </summary>
        public bool IsSet => _isSet;

        /// <summary>
        /// True if not null or undefined.
        /// </summary>
        public bool HasValue => _isSet && _value != null;

        /// <summary>
        /// Gets the value of the current <see cref="OptionalString"/>. 
        /// </summary>
        public string Value
        {
            get
            {
                if (IsSet)
                    return _value;

                throw new InvalidOperationException("The value is undefined.");
            }
            set {
                _isSet = true;
                _value = value;
            }
        }

        /// <summary>
        /// Conversion from String
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator OptionalString(string value)
        {
            return new OptionalString(value);
        }

        /// <summary>
        /// Conversion to string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator string(OptionalString value)
        {
            return (value.HasValue) ? value._value : null;
        }
        
        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="other"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="other">Another object to compare to. </param>
        public override bool Equals(object other)
        {
            if (!HasValue)
                return other == null;
            return other != null && _value.Equals(other);
        }

        /// <summary>
        /// Implementation of the equals operator.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator ==(OptionalString t1, OptionalString t2)
        {
            // undefined equals undefined
            if (!t1._isSet && !t2.IsSet) return true;

            // undefined != everything else
            if (t1._isSet ^ t2._isSet) return false;

            // null equals null
            if (!t1.HasValue && !t2.HasValue) return true;

            // null != everything else
            if (t1.HasValue ^ t2.HasValue) return false;

            // if both are values, compare them
            return t1._value.Equals(t2._value);
        }


        /// <summary>
        /// Implementation of the inequality operator.
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static bool operator !=(OptionalString t1, OptionalString t2)
        {
            return !(t1 == t2);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            if (!_isSet) return -1;
            if (!HasValue) return 0;
            return _value.GetHashCode();
        }

        /// <summary>
        /// Returns a text representation of
        /// the value, or an empty string if no value
        /// is present.
        /// </summary>
        public override string ToString()
        {
            return _isSet ? HasValue ? _value : "null" : "undefined";
        }
    }
}