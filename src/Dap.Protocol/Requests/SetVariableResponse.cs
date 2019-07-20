using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetVariableResponse
    {
        /// <summary>
        /// The new value of the variable.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// The type of the new value.Typically shown in the UI when hovering over the value.
        /// </summary>
        [Optional] public string type { get; set; }

        /// <summary>
        /// If variablesReference is > 0, the new value is structured and its children can be retrieved by passing variablesReference to the VariablesRequest.
        /// </summary>
        [Optional] public long? variablesReference { get; set; }

        /// <summary>
        /// The number of named child variables.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [Optional] public long? namedVariables { get; set; }

        /// <summary>
        /// The number of indexed child variables.
        /// The client can use this optional information to present the variables in a paged UI and fetch them in chunks.
        /// </summary>
        [Optional] public long? indexedVariables { get; set; }
    }

}
