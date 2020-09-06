using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.States;
using Xunit;
using Xunit.Abstractions;

namespace StatesLanguage.Tests
{
    public class EffectiveInputTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        
        public class EffectiveInputTest
        {
            public string Input { get; set; }
            public OptionalString InputPath { get; set; }
            public string Context { get; set; }
            public string Payload { get; set; }
            public string ExpectedResult { get; set; }
        }
        public EffectiveInputTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        
        private readonly IInputOutputProcessor _inputOutputProcessor = new InputOutputProcessor(new IntrinsicFunctionRegistry());
        
        public static IEnumerable<object[]> GetEffectiveInputData()
        {
            var items = new List<EffectiveInputTest>();
            
            items.Add(new EffectiveInputTest
            {
                Input = @"{ 'a': [1, 2, 3, 4] }",
                InputPath = "$.a[0,1]",
                ExpectedResult =  @"[ 1, 2 ]"
            });
            items.Add(new EffectiveInputTest
            {
                Input = @"{'first':{ 'fn':'bob','ln':'doe','pc':444719},'second':'2'}",
                Context = @"{ 'day':'monday' }",
                Payload = @"{ 'name.$':'$.fn', 'ctx.$':'$$.day' }",
                InputPath =  "$.first",
                ExpectedResult =  @"{ 'name':'bob', 'ctx':'monday' }"
            });
            items.Add(new EffectiveInputTest{
                Input = "[23,34,45]",
                Context = @"{ 'day':'monday' }",
                Payload = @"{ 'name.$':'$[1]', 'ctx.$':'$$.day' }",
                ExpectedResult =  @"{ 'name':34, 'ctx':'monday' }"
            });
            items.Add(new EffectiveInputTest
            {
                Input = @"{'obj':{'firstname':'bob','lastname':'doe','address':{'postcode':44719}},'array':['1','2',3]}",
                Payload = @"{'fixed':true,'sub': {'name.$':'$.obj.lastname','subsub':[{'i.$':'$.array[2]'}]}}",
                ExpectedResult = @"{'fixed':true,'sub': {'subsub':[{'i':3}],'name':'doe'}}"
            });
            items.Add(new EffectiveInputTest{
                Input = "[23,34,45]",
                Payload = @"{ 'name.$':'$[1]' }",
                ExpectedResult = @"{ 'name':34 }"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "{ 'firstname':'bob', 'lastname':'doe' }",
                Payload =@"{ 'name.$':'$.firstname' }",
                ExpectedResult = @"{ 'name':'bob' }"
            });
            items.Add(new EffectiveInputTest
            {
                Payload = "{ 'dsds.$':'$' }",
                Input = "'input'",
                ExpectedResult = @"{ 'dsds':'input' }"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "33",
                ExpectedResult = "33"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "33",
                ExpectedResult = "33"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "33",
                InputPath = null,
                ExpectedResult = "{}"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "[1,2,3,4]",
                InputPath = "$[2]",
                ExpectedResult = "3"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "{ 'firstname':'bob', 'lastname':'doe' }",
                InputPath = "$.firstname",
                ExpectedResult = "'bob'"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "{ user:{'firstname':'bob', 'lastname':'doe' }}",
                InputPath = "$.user",
                ExpectedResult = "{'firstname':'bob', 'lastname':'doe' }"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "{ user:{'firstname':'bob', 'lastname':'doe' }}",
                InputPath = null,
                Payload = "{'book':true}",
                ExpectedResult = "{'book':true}"
            });
            items.Add(new EffectiveInputTest
            {
                Input = "{ user:{'firstname':'bob', 'lastname':'doe' }}",
                Payload = "{'book':true, 'u.$':'$.user.firstname'}",
                ExpectedResult = "{'book':true, 'u':'bob'}"
            });

            return items.Select(x => new object[]{x});
        }
        
                
        [Theory]
        [MemberData(nameof(GetEffectiveInputData))]
        public void Test_Effective_Input(EffectiveInputTest test)
        {
            var input = JToken.Parse(test.Input);
            var context = test.Context != null ? JObject.Parse(test.Context) : null;
            var payload = test.Payload != null ? JObject.Parse(test.Payload) : null;
            
            var effectiveInput = _inputOutputProcessor.GetEffectiveInput(
                input,
                test.InputPath,
                payload, context);
            
            _testOutputHelper.WriteLine("Effective Output:"+ effectiveInput);
            _testOutputHelper.WriteLine("Expected Result:" + test.ExpectedResult);
            
            Assert.True(JToken.DeepEquals(JToken.Parse(test.ExpectedResult),effectiveInput));
        }

        [Fact]
        public void Test_Effective_Input_Errors()
        {
            Assert.Throws<PathMatchFailureException>(() => _inputOutputProcessor.GetEffectiveInput("input", "$.notfound", null, new JObject()));
            Assert.Throws<PathMatchFailureException>(()=> _inputOutputProcessor.GetEffectiveInput(JArray.Parse("[1,2,3,4]"), "$[5]", null, new JObject()));
            //Invalid Payload
            Assert.Throws<ParameterPathFailureException>(() => _inputOutputProcessor.GetEffectiveInput(
                "input",
                null,
                JObject.Parse("{'test.$':'$.value'}"), null));
            //Invalid Context 
            Assert.Throws<ParameterPathFailureException>(() => _inputOutputProcessor.GetEffectiveInput(
                "input", 
                null,
                JObject.Parse("{'test.$':'$$.value'}"), JObject.Parse("{'a':'b'}")));
        }
    }
}