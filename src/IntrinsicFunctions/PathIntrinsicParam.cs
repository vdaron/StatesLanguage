namespace StatesLanguage.IntrinsicFunctions
{
    public class PathIntrinsicParam : IntrinsicParam
    {
        public PathIntrinsicParam(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}