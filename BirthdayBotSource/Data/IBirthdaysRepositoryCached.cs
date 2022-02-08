using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    interface IBirthdaysRepositoryCached : IBirthdaysRepository
    {
        public Task SaveChangesAsync();
    }
}
