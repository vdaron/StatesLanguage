namespace StatesLanguage.IntrinsicFunctions;

public class BooleanIntrinsicParam : IntrinsicParam
{
    public BooleanIntrinsicParam(bool value)
    {
        Value = value;
    }

    public bool Value { get; }
}