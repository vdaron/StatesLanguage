using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.IntrinsicFunctions;
using StatesLanguage.States;
using Xunit;
using Xunit.Abstractions;

namespace StatesLanguage.Tests
{
    public class FailInputTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public class FailInputTest
        {
            public string Input { get; set; }
            public OptionalString FailPath { get; set; }
            public string Context { get; set; }
            public string Payload { get; set; }
            public string ExpectedResult { get; set; }
        }

        public FailInputTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private readonly IInputOutputProcessor _inputOutputProcessor = new InputOutputProcessor(new IntrinsicFunctionRegistry());

        public static IEnumerable<object[]> GetFailInputData()
        {
            var items = new List<FailInputTest>();

            items.Add(new FailInputTest
            {
                Input = @"{
                              ""fakepath"": ""error1""
                          }",
                FailPath = "$.fakepath",
                ExpectedResult =  @"error1"
            });

            items.Add(new FailInputTest
            {
                Input = @"{
                              ""fakepath"": ""States.Format('This is a custom error message for {}, caused by {}.', $.fake01, $.fake02)"",
                              ""fake01"": ""555"",
                              ""fake02"": ""777""
                            }",
                FailPath = "$.fakepath",
                ExpectedResult =  @"This is a custom error message for 555, caused by 777."
            });

            items.Add(new FailInputTest
            {
                Input = "{\n  \"Error\": \"GFHTD 23\"\n}",
                FailPath = @"$.Error",
                ExpectedResult =  @"GFHTD 23"
            });

            items.Add(new FailInputTest
            {
                Input = "{\n  \"Cause\": \"ABCDF 01\"\n}",
                FailPath = @"$.Cause",
                ExpectedResult =  @"ABCDF 01"
            });

            items.Add(new FailInputTest
            {
                Input = @"{""Error"": ""ABCDF 01""}",
                FailPath = "States.Format('{}', $.Error)",
                ExpectedResult =  @"ABCDF 01"
            });

            items.Add(new FailInputTest
            {
                Input = @"{""Cause"": ""ABCDF 01"",""Error"": ""GFHTD 23""}",
                FailPath = "States.Format('This is a custom error message for {}, caused by {}.', $.Error, $.Cause)",
                ExpectedResult =  @"This is a custom error message for GFHTD 23, caused by ABCDF 01."
            });

            return items.Select(x => new object[]{x});
        }

        [Theory]
        [MemberData(nameof(GetFailInputData))]
        public void Test_Fail_Input(FailInputTest test)
        {
            var input = JToken.Parse(test.Input);
            var context = test.Context != null ? JObject.Parse(test.Context) : null;
            var payload = test.Payload != null ? JObject.Parse(test.Payload) : null;

            var failInput = _inputOutputProcessor.GetFailPathInput(
                input,
                test.FailPath,
                payload, context);

            _testOutputHelper.WriteLine("Fail Output:"+ failInput);
            _testOutputHelper.WriteLine("Expected Result:" + test.ExpectedResult);

            Assert.Equal(test.ExpectedResult, failInput.Value<string>());
        }

        [Fact]
        public void Test_Fail_Input_Errors()
        {
            Assert.Throws<PathMatchFailureException>(() =>
                _inputOutputProcessor.GetFailPathInput("input", "$.notfound", null, new JObject()));
            Assert.Throws<PathMatchFailureException>(() =>
                _inputOutputProcessor.GetFailPathInput(JArray.Parse("[1,2,3,4]"), "$[5]", null, new JObject()));
        }
    }
}