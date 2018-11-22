using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WALL_R.Models;

namespace WALL_R.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private bool checkAuthentication()
        {
            return Libraries.SessionManager.checkAuthentication("admin", HttpContext.Request.Cookies["session"]);
        }

        private room_management_dbContext getContext()
        {
            return new room_management_dbContext();
        }

        [HttpPost("room")]
        public IActionResult CreateRoom(string room_number)
        {
            if(!checkAuthentication())
            {
                return Unauthorized();
            }

            if(String.IsNullOrEmpty(room_number))
            {
                return NotFound("Raummnummer nicht gültig.");
            }

            room_management_dbContext context = getContext();
            Rooms room = new Rooms();

            room.RoomNumber = room_number;
            context.Add(room);
            context.SaveChanges();

            return Ok();
        }

        [HttpGet("account")]
        public IActionResult GetAccounts()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }

            // Get database context
            room_management_dbContext context = getContext();
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            // Get all account:
            var accounts = context.Accounts;

            // Return a success report including all accounts to the frontend:
            return Ok(accounts);
        }

        [HttpPost("room/newOwner")]
        public IActionResult SetOwnerForRoom(int room_id, int owner_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            if (room_id == null)
            {
                return NotFound("Keine Raumnummer angegeben.");
            }
            if (owner_id == null)
            {
                return NotFound("Bentzernummer nicht gültig.");
            }

            room_management_dbContext context = getContext();
            if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
            {
                return NotFound();
            }

            if (context.Accounts.Where(f => f.Id == owner_id).Count() == 0)
            {
                return NotFound();
            }

            context.Rooms.Where(f => f.Id == room_id).First().OwnerId = owner_id;
            context.SaveChanges();
            return Ok();
        }
    }
}