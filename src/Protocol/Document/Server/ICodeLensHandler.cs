using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.CodeLens)]
    public interface ICodeLensHandler : IJsonRpcRequestHandler<CodeLensParams, Container<CodeLens>>, IRegistration<CodeLensRegistrationOptions>, ICapability<CodeLensClientCapabilities> { }

    [Parallel, Method(DocumentNames.CodeLensResolve)]
    public interface ICodeLensResolveHandler : ICanBeResolvedHandler<CodeLens> { }

    public abstract class CodeLensHandler : ICodeLensHandler, ICodeLensResolveHandler
    {
        private readonly CodeLensRegistrationOptions _options;
        private readonly ProgressManager _progressManager;
        public CodeLensHandler(CodeLensRegistrationOptions registrationOptions, ProgressManager progressManager)
        {
            _options = registrationOptions;
            _progressManager = progressManager;
        }

        public CodeLensRegistrationOptions GetRegistrationOptions() => _options;

        public Task<Container<CodeLens>> Handle(CodeLensParams request, CancellationToken cancellationToken)
        {
            var partialResults = _progressManager.For(request, cancellationToken);
            var createReporter = _progressManager.Delegate(request, cancellationToken);
            return Handle(request, partialResults, createReporter, cancellationToken);
        }

        public abstract Task<Container<CodeLens>> Handle(
            CodeLensParams request,
            IObserver<Container<CodeLens>> partialResults,
            Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
            CancellationToken cancellationToken
        );

        public abstract Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken);
        public abstract bool CanResolve(CodeLens value);
        public virtual void SetCapability(CodeLensClientCapabilities capability) => Capability = capability;
        protected CodeLensClientCapabilities Capability { get; private set; }
    }

    public static class CodeLensHandlerExtensions
    {
        public static IDisposable OnCodeLens(
            this ILanguageServerRegistry registry,
            Func<CodeLensParams, IObserver<Container<CodeLens>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<CodeLens>>> handler,
            Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler = null,
            Func<CodeLens, bool> canResolve = null,
            CodeLensRegistrationOptions registrationOptions = null,
            Action<CodeLensClientCapabilities> setCapability = null)
        {
            var codeLensRegistrationOptions = new CodeLensRegistrationOptions();
            codeLensRegistrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            if (registrationOptions != null)
            {
                codeLensRegistrationOptions.DocumentSelector = registrationOptions.DocumentSelector;
            }
            return registry.AddHandlers(new DelegatingHandler(handler, resolveHandler, registry.ProgressManager, canResolve, setCapability, codeLensRegistrationOptions));
        }

        class DelegatingHandler : CodeLensHandler
        {
            private readonly Func<CodeLensParams, IObserver<Container<CodeLens>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<CodeLens>>> _handler;
            private readonly Func<CodeLens, CancellationToken, Task<CodeLens>> _resolveHandler;
            private readonly Func<CodeLens, bool> _canResolve;
            private readonly Action<CodeLensClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<CodeLensParams, IObserver<Container<CodeLens>>, Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>>, CancellationToken, Task<Container<CodeLens>>> handler,
                Func<CodeLens, CancellationToken, Task<CodeLens>> resolveHandler,
                ProgressManager progressManager,
                Func<CodeLens, bool> canResolve,
                Action<CodeLensClientCapabilities> setCapability,
                CodeLensRegistrationOptions registrationOptions) : base(registrationOptions, progressManager)
            {
                _handler = handler;
                _resolveHandler = resolveHandler;
                _canResolve = canResolve;
                _setCapability = setCapability;
            }

            public override Task<Container<CodeLens>> Handle(
                CodeLensParams request,
                IObserver<Container<CodeLens>> partialResults,
                Func<WorkDoneProgressBegin, IObserver<WorkDoneProgressReport>> createReporter,
                CancellationToken cancellationToken
            ) => _handler.Invoke(request, partialResults, createReporter, cancellationToken);
            public override Task<CodeLens> Handle(CodeLens request, CancellationToken cancellationToken) => _resolveHandler.Invoke(request, cancellationToken);
            public override bool CanResolve(CodeLens value) => _canResolve.Invoke(value);
            public override void SetCapability(CodeLensClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
