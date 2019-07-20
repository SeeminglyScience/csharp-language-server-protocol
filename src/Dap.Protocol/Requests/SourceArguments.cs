using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SourceArguments : IRequest<SourceResponse>
    {
        /// <summary>
        /// Specifies the source content to load.Either source.path or source.sourceReference must be specified.
        /// </summary>
        [Optional] public Source source { get; set; }

        /// <summary>
        /// The reference to the source.This is the same as source.sourceReference.This is provided for backward compatibility since old backends do not understand the 'source' attribute.
        /// </summary>
        public long sourceReference { get; set; }
    }

}
