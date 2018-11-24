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
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();

                // Create model for new room:
                Rooms room = new Rooms();

                // Set roomnumber for the new room:
                room.RoomNumber = room_number;

                // Add new room to database context
                context.Add(room);

                // Save changes in context permanently to the database
                context.SaveChanges();

                // Return a "200 - OK" success report to the frontend:
                return Ok();
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

       

        [HttpPost("room/newOwner")]
        public IActionResult SetOwnerForRoom(int room_id, int owner_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();
                
                // Get all rooms where room_id equals received room_id:
                List<Rooms> rooms = context.Rooms.Where(f => f.Id == room_id).ToList();

                // Check if list of rooms is empty and if so send "404 - Not Found" to the frontend:
                if (rooms.Count() == 0)
                {
                    return NotFound();
                }

                // Check if no accounts for given owner_id exist and if so send "404 - Not Found" to the frontend:
                if (context.Accounts.Where(f => f.Id == owner_id).Count() == 0)
                {
                    return NotFound();
                }

                // Get room that has to be changed:
                Rooms room = rooms.First();

                // Set new owner_id for room
                room.OwnerId = owner_id;

                // Change entry in database context
                context.Update(room);

                // Save changes in context to database
                context.SaveChanges();

                // Return a "200 - OK" success report 
                return Ok();
            }
            catch
            {

                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }
    }
}