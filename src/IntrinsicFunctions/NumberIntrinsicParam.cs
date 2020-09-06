namespace StatesLanguage.IntrinsicFunctions
{
    public class NumberIntrinsicParam : IntrinsicParam
    {
        public NumberIntrinsicParam(decimal number)
        {
            Number = number;
        }

        public decimal Number { get; }
    }
}