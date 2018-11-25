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
            try
            {
                room_management_dbContext context = getContext();
                // Get user who sent request
                Accounts account = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]);

                // If user is logged in, log him out first:
                if (account != null)
                {
                    Logout();
                }

                // Check sent data
                if (context.Accounts.Where(f => f.Email == email).Count() == 0 || context.Accounts.Where(f => f.Email == email).First().Password != password)
                {
                    return NotFound("Falsche Accountdaten");
                }

                // Create session for accout and give the session cookie
                account = context.Accounts.Where(f => f.Email == email).First();
                string token = Guid.NewGuid().ToString();
                Sessions session = new Sessions();
                session.Token = token;
                session.AccountId = account.Id;
                session.ExpiringDate = DateTime.Now.AddMinutes(20);
                context.Sessions.Add(session);
                context.SaveChanges();
                Response.Cookies.Append("session", token);

                // Return success message including account data:
                Dictionary<String, String> result = new Dictionary<String, String>();
                result.Add("rightgroup", Libraries.SessionManager.GetRightgroupForAccount(account.Id));
                result.Add("name", account.Prename + " " + account.Surname);
                result.Add("id", account.Id.ToString());
                Json(result);
                return Ok(Json(result));
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try { 
                room_management_dbContext context = getContext();

                // Get session token
                string token = HttpContext.Request.Cookies["session"];

                // Get Account and check if it is logged in
                Accounts account = Libraries.SessionManager.getAccountForSession(token);
                if (account == null)
                {
                    return NotFound();
                }

                // Clear all session we find for the account identified by its token
                Libraries.SessionManager.clearSessionsByToken(token);

                // Return a "200 - OK" success report 
                return Ok();
            }
            catch {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }
    }
}