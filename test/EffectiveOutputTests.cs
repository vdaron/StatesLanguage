using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Internal.Validation;
using StatesLanguage.IntrinsicFunctions;
using StatesLanguage.States;
using Xunit;
using Xunit.Abstractions;

namespace StatesLanguage.Tests
{
    public class EffectiveOutputTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public class EffectiveOutputTest
        {
            public string Input { get; set; }
            public string Result { get; set; }
            public OptionalString ResultPath { get; set; }
            public OptionalString OutputPath { get; set; }
            public string ExpectedResult { get; set; }
        }
        
        private readonly IInputOutputProcessor _inputOutputProcessor = new InputOutputProcessor(new IntrinsicFunctionRegistry());

        public EffectiveOutputTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public static IEnumerable<object[]> GetEffectiveInputData()
        {
            var items = new List<EffectiveOutputTest>();
            
            items.Add(new EffectiveOutputTest
            {
                Input = @"{'firstname':'john', 'lastname':'doe'}",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                ResultPath = null,
                OutputPath = "$.firstname",
                ExpectedResult =  @"'john'"
            });
            items.Add(new EffectiveOutputTest
            {
                Input = @"{'firstname':'john', 'lastname':'doe'}",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                OutputPath = "$.a[0,1]",
                ExpectedResult =  @"[ 1, 2 ]"
            });
            items.Add(new EffectiveOutputTest
            {
                Input = @"{'firstname':'john', 'lastname':'doe'}",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                ResultPath = @"$.result",
                ExpectedResult =  @"{'firstname':'john', 'lastname':'doe', 'result':{ 'a': [1, 2, 3, 4] }}"
            });
            items.Add(new EffectiveOutputTest
            {
                Input = @"{'firstname':'john', 'lastname':'doe'}",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                ResultPath = @"$.a.b.c",
                ExpectedResult =  @"{'firstname':'john', 'lastname':'doe', 'a':{'b':{'c':{ 'a': [1, 2, 3, 4]}}}}"
            });
            items.Add(new EffectiveOutputTest
            {
                Input = @"{'firstname':'john', 'lastname':'doe'}",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                ResultPath = @"$.firstname",
                ExpectedResult =  @"{'firstname':{ 'a': [1, 2, 3, 4] }, 'lastname':'doe'}"
            });
            items.Add(new EffectiveOutputTest
            {
                Input = @"{'firstname':'john', 'lastname':'doe'}",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                OutputPath = "$.lastname",
                ResultPath = @"$.firstname",
                ExpectedResult =  @"'doe'",
            });
            items.Add(new EffectiveOutputTest
            {
                Input = @"[{'firstname':'john', 'lastname':'doe'}]",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                ResultPath = "$[1]",
                ExpectedResult =  @"[{'firstname':'john', 'lastname':'doe'},{'a': [1, 2, 3, 4] }]"
            });
            items.Add(new EffectiveOutputTest
            {
                Input = @"{'firstname':'john', 'lastname':'doe'}",
                Result = @"{ 'a': [1, 2, 3, 4] }",
                ResultPath = "$['test']",
                ExpectedResult =  @"{'firstname':'john', 'lastname':'doe', 'test':{'a': [1, 2, 3, 4] }}"
            });
            
            return items.Select(x => new object[]{x});
        }
        
                
        [Theory]
        [MemberData(nameof(GetEffectiveInputData))]
        public void Test_Effective_Output(EffectiveOutputTest test)
        {
            var result = JToken.Parse(test.Result);
            var input = JToken.Parse(test.Input);
            
            var effectiveOutput = _inputOutputProcessor.GetEffectiveOutput(
                input,
                result,
                test.OutputPath, 
                test.ResultPath);
            
            _testOutputHelper.WriteLine("Effective Output:"+ effectiveOutput);
            _testOutputHelper.WriteLine("Expected Result:" + test.ExpectedResult);
            
            Assert.True(JToken.DeepEquals(effectiveOutput,JToken.Parse(test.ExpectedResult)));
        }

        [Fact]
        public void Test_Effective_Output_Errors()
        {
            Assert.Throws<InvalidReferencePathException>(() => _inputOutputProcessor.GetEffectiveOutput(
                "input",
                null,
                new OptionalString(), 
                "$.a[0,1]"));
        }
        
        [Fact]
        public void Test_Effective_Output_Invalid_Result_Path()
        {
            Assert.Throws<ResultPathMatchFailureException>(() => _inputOutputProcessor.GetEffectiveOutput(
                "input",
                "{'b':123}",
                new OptionalString(), 
                "$.a"));
            Assert.Throws<ResultPathMatchFailureException>(() => _inputOutputProcessor.GetEffectiveOutput(
                "{'a':456}",
                "{'b':123}",
                new OptionalString(), 
                "$[2]"));
            Assert.Throws<ResultPathMatchFailureException>(() => _inputOutputProcessor.GetEffectiveOutput(
                "['a']",
                "{'b':123}",
                new OptionalString(), 
                "$.a"));
        }
    }
}