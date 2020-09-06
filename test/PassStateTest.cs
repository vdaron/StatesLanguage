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

using Newtonsoft.Json.Linq;
using StatesLanguage.States;
using Xunit;

namespace StatesLanguage.Tests
{
    public class PassStateTest
    {
        private static void AssertJsonEquals(string expected, JToken actual)
        {
            Assert.Equal(JObject.Parse(expected), actual);
        }

        [Fact]
        public void GetResult_ResultCreatedFromString_ReturnsJsonResult()
        {
            var strResult = "{\"Foo\": \"Bar\"}";
            AssertJsonEquals(strResult, NewPassState().Result(strResult).Build().Result);
        }

        [Fact]
        public void GetResult_ResultCreatedFromObject_ReturnsJsonResult()
        {
            AssertJsonEquals("{\"foo\": \"value\"}", NewPassState().Result(new SimplePojo("value")).Build().Result);
        }

        [Fact]
        public void GetResult_NullResult_ReturnsNull()
        {
            Assert.Null(NewPassState().Build().Result);
        }

        [Fact]
        public void SetResult_MalformedJson_ThrowsException()
        {
            Assert.Throws<StatesLanguageException>(() => NewPassState().Result("{").Build().Result);
        }

        [Fact]
        public void SetResult_PojoWithJacksonAnnotations_IgnoresAnnotations()
        {
            var pojo = new PojoWithJacksonAnnotations
            {
                foo = "FooValue",
                bar = "BarValue",
                baz = "BazValue"
            };
            AssertJsonEquals("{\"foo\": \"FooValue\", \"bar\": \"BarValue\"}",
                NewPassState().Result(pojo).Build().Result);
        }

        private PassState.Builder NewPassState()
        {
            return StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.End());
        }
    }
}