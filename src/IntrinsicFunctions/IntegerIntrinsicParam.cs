namespace StatesLanguage.IntrinsicFunctions;

public class IntegerIntrinsicParam : IntrinsicParam
{
        public IntegerIntrinsicParam(int number)
        {
            Number = number;
        }

        public int Number { get; }
}