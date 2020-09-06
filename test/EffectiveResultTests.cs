using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace StatesLanguage.Tests
{
    public class EffectiveResultTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public class EffectiveResultTest
        {
            public string Output { get; set; }
            public string Payload { get; set; }
            public string Context { get; set; }
            public string ExpectedResult { get; set; }
        }
        
        private readonly IInputOutputProcessor _inputOutputProcessor = new InputOutputProcessor(new IntrinsicFunctionRegistry());

        public EffectiveResultTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        
        public static IEnumerable<object[]> GetEffectiveInputData()
        {
            var items = new List<EffectiveResultTest>();
            items.Add(new EffectiveResultTest
            {
                Output = "'test'",
                ExpectedResult = "'test'"
            });
            
            items.Add(new EffectiveResultTest
            {
                Output = "'test'",
                Payload = "{}",
                ExpectedResult = "{}"
            });
            
            items.Add(new EffectiveResultTest
            {
                Output = "'test'",
                Payload = "{'a.$':'$'}",
                ExpectedResult = "{'a':'test'}"
            });
            
            items.Add(new EffectiveResultTest
            {
                Output = "'test'",
                Payload = "{'a.$':'$', 'bb.$':'$$.b'}",
                Context = "{'b':33}",
                ExpectedResult = "{'a':'test','bb':33}"
            });
            
            return items.Select(x => new object[]{x});
        }
        
        [Theory]
        [MemberData(nameof(GetEffectiveInputData))]
        public void Test_Effective_Result(EffectiveResultTest test)
        {
            var output = JToken.Parse(test.Output);
            var payload = test.Payload != null ? JObject.Parse(test.Payload) : null;
            var context = test.Context != null ? JObject.Parse(test.Context) : null;
            var expectedResult = JToken.Parse(test.ExpectedResult);

            var effectiveResult = _inputOutputProcessor.GetEffectiveResult(output, payload, context);
            
            _testOutputHelper.WriteLine("Effective Result:"+ effectiveResult);
            _testOutputHelper.WriteLine("Expected Result:" + expectedResult);
            
            Assert.True(JToken.DeepEquals(effectiveResult,JToken.Parse(test.ExpectedResult)));
        }
    }
}