namespace StatesLanguage.Model.ReferencePathTokens
{
    public class FieldToken : PathToken
    {
        public FieldToken(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}