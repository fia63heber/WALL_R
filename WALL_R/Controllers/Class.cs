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
        private room_management_dbContext getContext()
        {
            return new room_management_dbContext();
        }

        [HttpPost("room")]
        public IActionResult CreateRoom(string room_number)
        {
            room_management_dbContext context = getContext();
            Rooms room = new Rooms();

            room.RoomNumber = room_number;
            context.Add(room);
            context.SaveChanges();

            return Ok();
        }



        [HttpPost("room/newOwner")]
        public IActionResult SetOwnerForRoom(int room_id, int owner_id)
        {
            room_management_dbContext context = getContext();
            if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
            {
                return NotFound();
            }

            context.Rooms.Where(f => f.Id == room_id).First().OwnerId = owner_id;
            return Ok();
            context.SaveChanges();
        }
    }
}