namespace StatesLanguage.Model.ReferencePathTokens
{
    public class ArrayIndexToken : PathToken
    {
        public ArrayIndexToken(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }
}