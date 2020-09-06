using StatesLanguage.IntrinsicFunctions;

namespace StatesLanguage.Interfaces
{
    public interface IIntrinsicFunctionRegistry
    {
        void Register(string name, IntrinsicFunctionFunc func);
        void Unregister(string name);
    }
}