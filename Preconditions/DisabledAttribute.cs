using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BirthdayBot.Preconditions
{
    public class DisabledAttribute : PreconditionAttribute
    {
        public DisabledAttribute() { }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(PreconditionResult.FromError("This command is currently disabled."));
        }
    }
}