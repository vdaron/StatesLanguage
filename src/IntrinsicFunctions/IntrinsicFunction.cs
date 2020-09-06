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
    }
}