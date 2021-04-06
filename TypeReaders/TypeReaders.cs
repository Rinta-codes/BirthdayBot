using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace BirthdayBot.TypeReaders
{
    // Override of TypeReaders for classes of Discord.NET, such as GuildUser,
    // is currently not working due to bug https://github.com/discord-net/Discord.Net/issues/1485
    /*
    class GuildUserTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            return Task.FromResult(TypeReaderResult.FromError(CommandError.Exception, "Just checking if this actually works..."));
            SocketGuildUser guildUser;
            try
            {
                guildUser = services.GetRequiredService<DiscordRestClient>().GetGuildUserAsync(context.Guild.Id, context.User.Id) as IGuildUser as SocketGuildUser;
                return Task.FromResult(TypeReaderResult.FromSuccess(guildUser));
            }
            catch (Exception e)
            {
                return Task.FromResult(TypeReaderResult.FromError(e));
            }
        }
    }
    */

    class TestTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            return Task.FromResult(TypeReaderResult.FromSuccess(5));
        }
    }
}

