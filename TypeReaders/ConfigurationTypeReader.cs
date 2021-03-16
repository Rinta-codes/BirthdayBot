// .NET Base
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
// Discord
using Discord;
using Discord.Commands;

namespace BirthdayBot.TypeReaders
{
    class ConfigurationTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            IConfiguration config;
            try
            {
                config = services.GetRequiredService<IConfiguration>();
            }
            catch (Exception e)
            {
                return Task.FromResult(TypeReaderResult.FromError(e));
            }

            if (config is null) return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Configuration file not found."));
            else return Task.FromResult(TypeReaderResult.FromSuccess(services.GetRequiredService<IConfiguration>()));
        }
    }
}
