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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model.States;

namespace StatesLanguage.Model.Serialization
{
    internal class WaitStateDeserializer : JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var state = JObject.FromObject(value, new JsonSerializer
                                                  {
                                                      Formatting = serializer.Formatting,
                                                      NullValueHandling = NullValueHandling.Ignore,
                                                      DefaultValueHandling = DefaultValueHandling.Ignore,
                                                      ContractResolver = StatesContractResolver.Instance
                                                  });

            var wiatFor = ((WaitState) value).WaitFor;

            var json = JObject.FromObject(wiatFor);

            foreach (var prop in json.Properties())
            {
                state.Add(prop);
            }

            var transition = ((WaitState) value).Transition;

            json = JObject.FromObject(transition);

            foreach (var prop in json.Properties())
            {
                state.Add(prop);
            }

            state.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(WaitState) == objectType;
        }
    }
}