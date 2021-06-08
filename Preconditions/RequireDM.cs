using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace BirthdayBot.Preconditions
{
    public class RequireDM : PreconditionAttribute
    {
        public RequireDM() { }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Channel is not SocketDMChannel)
                return Task.FromResult(PreconditionResult.FromError("This command is only available via DM"));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}