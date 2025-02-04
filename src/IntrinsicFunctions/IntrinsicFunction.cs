using System;

namespace StatesLanguage.IntrinsicFunctions
{
    public class IntrinsicFunction : IntrinsicParam
    {
        public string Name { get; set; }
        public IntrinsicParam[] Parameters { get; set; }

        public static IntrinsicFunction Parse(string intrinsicFunctionDefinition)
        {
            return IntrinsicFunctionParser.Parse(intrinsicFunctionDefinition);
        }

        public static bool TryParse(string expression, out IntrinsicFunction intrinsicFunction)
        {
            try
            {
                intrinsicFunction = IntrinsicFunctionParser.Parse(expression);
                return true;
            }
            catch
            {
                intrinsicFunction = null;
                return false;
            }
        }
    }
}