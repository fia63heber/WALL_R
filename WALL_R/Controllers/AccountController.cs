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
            if (Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]) == null)
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

           
            string token = Guid.NewGuid().ToString();

            Sessions session = new Sessions();
            session.Token = token;
            session.AccountId = context.Accounts.Where(f => f.Email == email).First().Id;
            session.ExpiringDate = DateTime.Now.AddMinutes(20);
            context.Sessions.Add(session);
            context.SaveChanges();

            Response.Cookies.Append("session", token);



            return Ok();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            room_management_dbContext context = getContext();
            string token = HttpContext.Request.Cookies["session"];
            Accounts account = Libraries.SessionManager.getAccountForSession(token);

            if (account == null)
            {
                return NotFound();
            }

            foreach (Sessions session in context.Sessions.Where(f => f.AccountId == account.Id))
            {
                Libraries.SessionManager.clearSessionsByToken(token);
            }
            return Ok();
        }
    }
}