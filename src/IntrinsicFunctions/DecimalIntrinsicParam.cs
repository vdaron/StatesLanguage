namespace StatesLanguage.IntrinsicFunctions
{
    public class DecimalIntrinsicParam : IntrinsicParam
    {
        public DecimalIntrinsicParam(decimal number)
        {
            Number = number;
        }

        public decimal Number { get; }
    }
}