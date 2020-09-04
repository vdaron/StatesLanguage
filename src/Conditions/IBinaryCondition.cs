/*
 * Copyright 2010-2017 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * Copyright 2018- Vincent DARON All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal;

namespace StatesLanguage.Conditions
{
    public enum Operator
    {
        Eq,
        Gt,
        Gte,
        Lt,
        Lte,
        Match
    }

    public interface IBinaryCondition : ICondition
    {
        string Variable { get; }
    }

    public abstract class BinaryCondition<T> : IBinaryCondition where T : IComparable<T>
    {
        private readonly Operator _operator;

        protected BinaryCondition(Operator op)
        {
            _operator = op;
        }

        public abstract T ExpectedValue { get; protected set; }

        /// <inheritdoc />
        [JsonProperty(PropertyNames.VARIABLE)]
        public string Variable { get; protected set; }

        /// <inheritdoc />
        public bool Match(JToken token)
        {
            try
            {
                if (!string.IsNullOrEmpty(Variable) && token.Type != JTokenType.Object)
                {
                    return false;
                }

                var tmp = string.IsNullOrEmpty(Variable) ? token : ((JObject) token).SelectToken(Variable);
                if (tmp == null || tmp.Type != JTokenType.Boolean
                    && tmp.Type != JTokenType.Date
                    && tmp.Type != JTokenType.Integer
                    && tmp.Type != JTokenType.Float
                    && tmp.Type != JTokenType.String
                    && tmp.Type != JTokenType.Guid
                    && tmp.Type != JTokenType.Uri)
                {
                    return false;
                }

                switch (_operator)
                {
                    case Operator.Eq:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) == 0;
                    case Operator.Gt:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) > 0;
                    case Operator.Gte:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) >= 0;
                    case Operator.Lt:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) < 0;
                    case Operator.Lte:
                        return tmp.Value<T>()?.CompareTo(ExpectedValue) <= 0;
                    case Operator.Match:
                        return IsMatch(tmp.Value<string>(), ExpectedValue.ToString());
                }
            }
            catch (FormatException)
            {
            }

            return false;
        }

        private static bool IsMatch(string s, string pattern)
        {
            return new Regex(WildcardToRegex(pattern)).IsMatch(s);
        }

        private static string WildcardToRegex(string wildcard)
        {
            var s = new StringBuilder();
            s.Append('^');
            var skipNext = false;
            for (int i = 0, isa = wildcard.Length; i < isa; i++)
            {
                var c = wildcard[i];

                if (skipNext)
                {
                    s.Append(c);
                    skipNext = false;
                    continue;
                }

                switch (c)
                {
                    case '*':
                        s.Append(".*?");
                        break;
                    case '?':
                        s.Append(".");
                        break;
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                    case '$':
                    case '^':
                    case '.':
                    case '{':
                    case '}':
                    case '|':
                        s.Append("\\");
                        s.Append(c);
                        break;
                    case '\\':
                        s.Append("\\");
                        skipNext = true;
                        break;
                    default:
                        s.Append(c);
                        break;
                }
            }

            s.Append('$');
            return s.ToString();
        }
    }

    public abstract class BinaryConditionPath : IBinaryCondition
    {
        private readonly Operator _operator;
        private readonly JTokenType[] _validTokenTypes;

        protected BinaryConditionPath(Operator op, params JTokenType[] validTokenTypes)
        {
            _operator = op;
            _validTokenTypes = validTokenTypes;
        }

        public abstract string ExpectedValuePath { get; protected set; }

        [JsonProperty(PropertyNames.VARIABLE)]
        public string Variable { get; protected set; }

        public bool Match(JToken token)
        {
            try
            {
                if (!string.IsNullOrEmpty(Variable) && token.Type != JTokenType.Object)
                {
                    return false;
                }

                var val = string.IsNullOrEmpty(ExpectedValuePath)
                    ? token
                    : ((JObject) token).SelectToken(ExpectedValuePath);
                var tmp = string.IsNullOrEmpty(Variable) ? token : ((JObject) token).SelectToken(Variable);

                if (val.Type != tmp.Type)
                {
                    return false;
                }

                if (!_validTokenTypes.Contains(val.Type))
                {
                    return false;
                }

                switch (val.Type)
                {
                    case JTokenType.None:
                    case JTokenType.Object:
                    case JTokenType.Array:
                    case JTokenType.Constructor:
                    case JTokenType.Property:
                    case JTokenType.Comment:
                        return false;
                    case JTokenType.Integer:
                        return Compare(tmp.Value<int>(), val.Value<int>());
                    case JTokenType.Float:
                        return Compare(tmp.Value<float>(), val.Value<float>());
                    case JTokenType.Guid:
                    case JTokenType.Uri:
                    case JTokenType.String:
                        return Compare(tmp.Value<string>(), val.Value<string>());
                    case JTokenType.Boolean:
                        return Compare(tmp.Value<bool>(), val.Value<bool>());
                    case JTokenType.Date:
                        return Compare(tmp.Value<DateTime>(), val.Value<DateTime>());
                    case JTokenType.TimeSpan:
                        return Compare(tmp.Value<TimeSpan>(), val.Value<TimeSpan>());
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (FormatException)
            {
            }

            return false;
        }

        private bool Compare<T>(T val1, T val2) where T : IComparable
        {
            switch (_operator)
            {
                case Operator.Eq:
                    return val1?.CompareTo(val2) == 0;
                case Operator.Gt:
                    return val1?.CompareTo(val2) > 0;
                case Operator.Gte:
                    return val1?.CompareTo(val2) >= 0;
                case Operator.Lt:
                    return val1?.CompareTo(val2) < 0;
                case Operator.Lte:
                    return val1?.CompareTo(val2) <= 0;
                default:
                    return false; // not supported
            }

            ;
        }
    }
}