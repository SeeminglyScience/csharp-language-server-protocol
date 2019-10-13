using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(WorkspaceNames.DidChangeConfiguration)]
    public interface IDidChangeConfigurationHandler : IJsonRpcNotificationHandler<DidChangeConfigurationParams>, IRegistration<object>, ICapability<DidChangeConfigurationClientCapabilities> { }

    public abstract class DidChangeConfigurationHandler : IDidChangeConfigurationHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeConfigurationClientCapabilities capability) => Capability = capability;
        protected DidChangeConfigurationClientCapabilities Capability { get; private set; }
    }

    public static class DidChangeConfigurationHandlerExtensions
    {
        public static IDisposable OnDidChangeConfiguration(
            this ILanguageServerRegistry registry,
            Func<DidChangeConfigurationParams, CancellationToken, Task<Unit>> handler,
            Action<DidChangeConfigurationClientCapabilities> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability));
        }

        class DelegatingHandler : DidChangeConfigurationHandler
        {
            private readonly Func<DidChangeConfigurationParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<DidChangeConfigurationClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DidChangeConfigurationParams, CancellationToken, Task<Unit>> handler,
                Action<DidChangeConfigurationClientCapabilities> setCapability) : base()
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DidChangeConfigurationClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
