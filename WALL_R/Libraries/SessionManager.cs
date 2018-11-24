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

        public static void clearSessionsByToken(string token)
        {
            room_management_dbContext context = getContext();

            var sessions = context.Sessions.Where(f => f.Token == token);
            if (sessions.Count() == 0)
            {
                return;
            }
            int? account_id = sessions.First().AccountId;

            var account_sessions = context.Sessions.Where(f => f.AccountId == account_id);

            context.Sessions.RemoveRange(account_sessions);
            context.SaveChanges();
        }

        public static bool checkAuthentication(string flag, string token)
        {
            room_management_dbContext context = getContext();
            var sessions = context.Sessions.Where(f => f.Token == token);
            if (sessions.Count() == 0)
            {
                return false;
            }

            Sessions session = sessions.First();
            if (session == null || session.AccountId == null || session.ExpiringDate == null || session.ExpiringDate < DateTime.Now)
            {
                return false;
            }

            context.Sessions.Where(f => f.Token == token).First().ExpiringDate = DateTime.Now.AddMinutes(20);
            context.SaveChanges();

            List<Accounts> accounts = context.Accounts.Where(f => f.Id == session.AccountId).ToList();
            if (accounts.Count() > 0)
            {
                if (flag == "general")
                {
                    return true;
                }
                if (flag == "owner" && (context.Rooms.Where(f => f.OwnerId == accounts.First().Id).Count() > 0 || accounts.First().Id == 1))
                {
                    return true;
                }
                if (flag == "admin" && accounts.First().Id == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetRightgroupForAccount(int account_id)
        {
            room_management_dbContext context = getContext();
            if (account_id == 1)
            {
                return "admin";
            }
            if (context.Rooms.Where(f => f.OwnerId == account_id).Count() > 0)
            {
                return "owner";
            }
            return "general";
        }
    }
}
