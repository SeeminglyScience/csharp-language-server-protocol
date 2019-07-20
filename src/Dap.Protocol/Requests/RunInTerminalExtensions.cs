using System.Threading.Tasks;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public static class RunInTerminalExtensions
    {
        public static Task<RunInTerminalResponse> RunInTerminal(this IDebugAdapterClient mediator, RunInTerminalArguments @params)
        {
            return mediator.SendRequest<RunInTerminalArguments, RunInTerminalResponse>(RequestNames.RunInTerminal, @params);
        }
    }

}
