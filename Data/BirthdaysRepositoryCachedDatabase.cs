using System;
using System.Threading.Tasks;

namespace BirthdayBot.Data
{
    /*
     * This class details data retrieval and data upload process for BirthdayRepositoryCached
     * when birthday data is stored in database
     */
    class BirthdaysRepositoryCachedDatabase : BirthdaysRepositoryCached
    {
        public BirthdaysRepositoryCachedDatabase()
        { }

        public override async Task LoadUserBirthdaysAsync()
        { }

        public override async Task SaveChangesAsync()
        { }
    }
}
