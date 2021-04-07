using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

public class Disabled : PreconditionAttribute
{
    public Disabled() {}

    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        return Task.FromResult(PreconditionResult.FromError("This command is currently disabled."));
    }
}