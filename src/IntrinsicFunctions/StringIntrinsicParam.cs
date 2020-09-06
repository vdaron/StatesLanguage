namespace StatesLanguage.IntrinsicFunctions
{
    public class StringIntrinsicParam : IntrinsicParam
    {
        public StringIntrinsicParam(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}