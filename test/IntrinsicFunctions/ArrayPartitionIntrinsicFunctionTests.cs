using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class ArrayPartitionIntrinsicFunctionTests
{
        private readonly IntrinsicFunctionRegistry _registry = new();
        private const string FUNCTION_NAME = "States.ArrayPartition";

        [Theory]
        [InlineData("42", "{}", true)]
        [InlineData("null, 0", "{}", true)]
        [InlineData("$.array, 2", "{'array': [1,2,3,4,5]}", false, "[[1,2],[3,4],[5]]")]
        [InlineData("$.array, 10", "{'array': [1,2,3,4,5]}", false, "[[1,2,3,4,5]]")]
        public void TestArrayPartition(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
            IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
                _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}