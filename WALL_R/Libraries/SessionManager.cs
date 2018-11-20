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
            var sessions = context.Sessions.Where(f => f.Token == token);
            if (sessions.Count() == 0)
            {
                return null;
            }

            Sessions session = sessions.First();
            if (session == null || session.AccountId == null || session.ExpiringDate == null || session.ExpiringDate < DateTime.Now)
            {
                return null;
            }
            
            context.Sessions.Where(f => f.Token == token).First().ExpiringDate = DateTime.Now.AddMinutes(20);
            context.SaveChanges();

            return context.Accounts.Where(f => f.Id == session.AccountId).First();
        }
    }
}
