using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Threading.Tasks;
using System.Threading;
using System;

// ReSharper disable CheckNamespace


namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.ColorPresentation)]
    public interface IColorPresentationHandler : IJsonRpcRequestHandler<ColorPresentationParams, Container<ColorPresentation>>, IRegistration<DocumentColorRegistrationOptions>, ICapability<ColorProviderClientCapabilities> { }

    public abstract class ColorPresentationHandler : IColorPresentationHandler
    {
        private readonly DocumentColorRegistrationOptions _options;
        public ColorPresentationHandler(DocumentColorRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentColorRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<ColorPresentation>> Handle(ColorPresentationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(ColorProviderClientCapabilities capability) => Capability = capability;
        protected ColorProviderClientCapabilities Capability { get; private set; }
    }

    public static class ColorPresentationHandlerExtensions
    {
        public static IDisposable OnColorPresentation(
            this ILanguageServerRegistry registry,
            Func<ColorPresentationParams, CancellationToken, Task<Container<ColorPresentation>>> handler,
            DocumentColorRegistrationOptions registrationOptions = null,
            Action<ColorProviderClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new DocumentColorRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : ColorPresentationHandler
        {
            private readonly Func<ColorPresentationParams, CancellationToken, Task<Container<ColorPresentation>>> _handler;
            private readonly Action<ColorProviderClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<ColorPresentationParams, CancellationToken, Task<Container<ColorPresentation>>> handler,
                Action<ColorProviderClientCapabilities> setCapability,
                DocumentColorRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Container<ColorPresentation>> Handle(ColorPresentationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(ColorProviderClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
