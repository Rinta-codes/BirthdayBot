using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

public class RequireGuild : PreconditionAttribute
{
    public RequireGuild() { }

    public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
    {
        if (context.Channel is SocketDMChannel)
            return Task.FromResult(PreconditionResult.FromError("This command is not available via DM"));

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}