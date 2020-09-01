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

using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StatesLanguage.Model.States;

namespace StatesLanguage.Model.Serialization
{
    /// <summary>
    /// Do not serialize empty collections nor UnSet OptionalString 
    /// </summary>
    public class StatesContractResolver : DefaultContractResolver
    {
        public static readonly StatesContractResolver Instance = new StatesContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var shouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = obj => (shouldSerialize == null || shouldSerialize(obj)) && !IsEmptyCollection(property, obj) && IsSet(property,obj);
            return property;
        }

        private bool IsSet(JsonProperty property, object o)
        {
            var val = property.ValueProvider.GetValue(o);
            if (val is OptionalString ostring)
                return ostring.IsSet;
            return true;
        }

        private bool IsEmptyCollection(JsonProperty property, object target)
        {
            var value = property.ValueProvider.GetValue(target);
            if (value is ICollection collection && collection.Count == 0)
            {
                return true;
            }

            if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                return false;
            }

            var countProp = property.PropertyType.GetProperty("Count");
            if (countProp == null)
            {
                return false;
            }

            var count = value == null ? 0 :  (int) countProp.GetValue(value, null);
            return count == 0;
        }
    }
}