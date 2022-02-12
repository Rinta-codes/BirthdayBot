using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    public interface IBirthdaysRepositoryCached : IBirthdaysRepository
    {
        public Task LoadFromSourceAsync();

        public Task SaveToSourceAsync();
    }
}
