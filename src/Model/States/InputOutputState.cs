using System.Collections.Generic;
using Newtonsoft.Json;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    public abstract class InputOutputState : State
    {
        [JsonProperty(PropertyNames.INPUT_PATH)]
        public OptionalString InputPath { get; protected set; }

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        public OptionalString OutputPath { get; protected set; }
    }

    public abstract class InputOutputStateBuilder<T, B> : State.IBuilder<T> 
        where T : State
        where B : InputOutputStateBuilder<T, B>
    {
        [JsonProperty(PropertyNames.INPUT_PATH)]
        private OptionalString _inputPath;

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        private OptionalString _outputPath;
        
        internal InputOutputStateBuilder()
        {
        }
        /*
        * OPTIONAL. The value of “InputPath” MUST be a Path, which is applied to a State’s raw input to select some or all of
            * it;
            * that selection is used by the state. If not provided then the whole output from the previous state is used as input to
        * this state.
        *
        * @param inputPath New path value.
        * @return This object for method chaining.
        */
        public B InputPath(string inputPath)
        {
            _inputPath = inputPath;
            return (B) this;
        }
        
        /**
             * OPTIONAL. The value of “OutputPath” MUST be a path, which is applied to the state’s output after the application of
             * ResultPath, leading in the generation of the raw input for the next state. If not provided then the whole output is
             * used.
             *
             * @param outputPath New path value.
             * @return This object for method chaining.
             */
        public B OutputPath(string outputPath)
        {
            _outputPath = outputPath;
            return (B)this;
        }


        public abstract T Build();
    }
}
