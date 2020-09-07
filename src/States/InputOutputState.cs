using Newtonsoft.Json;
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    public abstract class InputOutputState : State
    {
        /// <summary>
        ///     JsonPath which is applied to a State’s raw input to select some or all of it;
        ///     If not provided then the whole output from the previous state is used as input to
        ///     this state.
        /// </summary>
        [JsonProperty(PropertyNames.INPUT_PATH)]
        public OptionalString InputPath { get; protected set; }

        /// <summary>
        ///     JsonPath which is applied to the state’s output after the application of
        ///     ResultPath, leading in the generation of the raw input for the next state. If not provided then the whole output is
        ///     used.
        /// </summary>
        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        public OptionalString OutputPath { get; protected set; }
    }

    public abstract class InputOutputStateBuilder<T, B> : StateBuilder<T, B>
        where T : State
        where B : InputOutputStateBuilder<T, B>
    {
        [JsonProperty(PropertyNames.INPUT_PATH)]
        protected OptionalString _inputPath;

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        protected OptionalString _outputPath;

        internal InputOutputStateBuilder()
        {
        }

        /// <summary>
        ///     OPTIONAL. The value of “InputPath” MUST be a Path, which is applied to a State’s raw input to select some or all of
        ///     it;
        ///     that selection is used by the state. If not provided then the whole output from the previous state is used as input
        ///     to
        ///     this state.
        /// </summary>
        /// <param name="inputPath">New path value</param>
        /// <returns>This object for method chaining.</returns>
        public B InputPath(OptionalString inputPath)
        {
            _inputPath = inputPath;
            return (B) this;
        }

        /// <summary>
        ///     OPTIONAL. The value of “OutputPath” MUST be a path, which is applied to the state’s output after the application of
        ///     ResultPath, leading in the generation of the raw input for the next state. If not provided then the whole output is
        ///     used.
        /// </summary>
        /// <param name="outputPath">New path value.</param>
        /// <returns>This object for method chaining.</returns>
        public B OutputPath(OptionalString outputPath)
        {
            _outputPath = outputPath;
            return (B) this;
        }
    }
}