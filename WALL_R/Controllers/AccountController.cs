using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WALL_R.Models;

namespace WALL_R.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private room_management_dbContext getContext()
        {
            return new room_management_dbContext();
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            room_management_dbContext context = getContext();
            Accounts account = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]);
            if (account == null)
            {
                Logout();
            }
            if (context.Accounts.Where(f => f.Email == email).Count() == 0)
            {
                return NotFound("Falsche Accountdaten");
            }

            if (context.Accounts.Where(f => f.Email == email).First().Password != password)
            {
                return NotFound("Falsche Accountdaten");
            }


            account = context.Accounts.Where(f => f.Email == email).First();
            string token = Guid.NewGuid().ToString();
            Sessions session = new Sessions();
            session.Token = token;
            session.AccountId = account.Id;
            session.ExpiringDate = DateTime.Now.AddMinutes(20);
            context.Sessions.Add(session);
            context.SaveChanges();

            Dictionary<String, String> result = new Dictionary<String, String>();
            result.Add("rightgroup", Libraries.SessionManager.GetRightgroupForAccount(account.Id));
            result.Add("name", account.Prename + " " + account.Surname);

            Json(result);

            return Ok(Json(result));
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            room_management_dbContext context = getContext();
            string token = HttpContext.Request.Cookies["session"];
            Libraries.SessionManager.clearSessionsByToken(token);
            Accounts account = Libraries.SessionManager.getAccountForSession(token);

            if (account == null)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}