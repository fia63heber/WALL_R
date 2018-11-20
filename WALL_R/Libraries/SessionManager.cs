using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WALL_R.Models;

namespace WALL_R.Libraries
{
    public static class SessionManager
    {
        private static room_management_dbContext getContext()
        {
            return new room_management_dbContext();
        }

        public static Accounts getAccountForSession(string token)
        {
            room_management_dbContext context = getContext();
            Sessions session = context.Sessions.Where(f => f.Token == token).First();

            if (session == null || session.AccountId == null || session.ExpiringDate == null || session.ExpiringDate < DateTime.Now)
            {
                return null;
            }

            return context.Accounts.Where(f => f.Id == session.AccountId).First();
        }
    }
}
