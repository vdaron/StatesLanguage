## States Language

This library contains some helper classes to help creating and modifying workflow using the [Amazon States Language](https://states-language.net/spec.html).
This is the workflow description language used by [AWS StepFunctions](https://aws.amazon.com/step-functions)

This project starts as a port of the java livrary [light-workflow-4j project](https://github.com/networknt/light-workflow-4j).

 
# Sample

```csharp
	StateMachine stateMachine = StepFunctionBuilder.StateMachine()
		.StartAt("InitialState")
		.TimeoutSeconds(30)
		.Comment("My Simple State Machine")
		.State("InitialState", StepFunctionBuilder.SucceedState()
			.Comment("Initial State")
			.InputPath("$.input")
			.OutputPath("$.output"))
		.Build();

    string json = stateMachine.ToJson();

    var builder = StateMachine.FromJson(json);
```

# InputOutputProcessor is also available

```csharp
    public interface IInputOutputProcessor
    {
        JToken GetEffectiveInput(JToken input, OptionalString inputPath, JObject payload, JObject context);
        JToken GetEffectiveResult(JToken output, JObject payload, JObject context);
        JToken GetEffectiveOutput(JToken input, JToken result, OptionalString outputPath, OptionalString resultPath);
    }
```