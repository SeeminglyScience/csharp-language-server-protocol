using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentFormattingParams : ITextDocumentIdentifierParams, IRequest<Container<TextEdit>>, IWorkDoneProgressParams
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The format options.
        /// </summary>
        public FormattingOptions Options { get; set; }

        /// <inheritdoc />
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
