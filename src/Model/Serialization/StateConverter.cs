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
    internal class StateConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var type = obj["Type"].Value<string>();
            State.IBuilder<State> result = null;

            switch (Enum.Parse(typeof(StateType), type))
            {
                case StateType.Choice:
                    result = ChoiceState.GetBuilder();
                    break;
                case StateType.Fail:
                    result = FailState.GetBuilder();
                    break;
                case StateType.Parallel:
                    result = ParallelState.GetBuilder();
                    break;
                case StateType.Pass:
                    result = PassState.GetBuilder();
                    break;
                case StateType.Succeed:
                    result = SucceedState.GetBuilder();
                    break;
                case StateType.Task:
                    result = TaskState.GetBuilder();
                    break;
                case StateType.Wait:
                    result = WaitState.GetBuilder();
                    break;
                case StateType.Map:
                    result = MapState.GetBuilder();
                    break;
            }

            serializer.Populate(obj.CreateReader(), result);

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(State.IBuilder<State>);
        }
    }
}