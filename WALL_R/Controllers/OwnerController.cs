using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WALL_R.Models;

namespace WALL_R.Controllers
{
    [Route("api/[controller]")]
    public class OwnerController : Controller
    {
        private room_management_dbContext getContext()
        {
            return new room_management_dbContext();
        }

        [HttpGet("owner/{owner_id}/defects")]
        public IActionResult GetDefectsForOwner(int owner_id)
        {
            room_management_dbContext context = getContext();
            var defects = new List<Defects>();

            foreach (Rooms room in context.Rooms.Where(f => f.OwnerId == owner_id))
            {
                foreach (Devices device in context.Devices.Where(f => f.RoomId == room.Id))
                {
                    foreach (Components component in context.Components.Where(f => f.DeviceId == device.Id))
                    {
                        defects.AddRange(context.Defects.Where(f => f.ComponentId == component.Id).ToList());
                    }
                }
            }

            return Ok(defects);
        }

        [HttpPost("defect/changePriority")]
        public IActionResult GetDefectsForOwner(int defect_id, int priority_id)
        {
            room_management_dbContext context = getContext();

            context.Defects.Where(f => f.Id == defect_id).First().PriorityId = priority_id;
            context.SaveChanges();
            
            return Ok();
        }

        [HttpPost("defect/changePriority")]
        public IActionResult changePriorityForDefect(int defect_id, int priority_id)
        {
            room_management_dbContext context = getContext();

            context.Defects.Where(f => f.Id == defect_id).First().PriorityId = priority_id;
            context.SaveChanges();

            return Ok();
        }

        [HttpPost("room/addPlanFile")]
        public IActionResult addPlanfileForRoom(int room_id)
        {
            room_management_dbContext context = getContext();

            if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
            {
                return NotFound();
            }


            int? file_id = context.Rooms.Where(f => f.Id == room_id).First().PictureFileId;
            
            if (file_id == null)
            {
                Files newFile = new Files();
                newFile.FilePath = "";
            }

            context.Files.Where(f => f.Id == file_id).First().FilePath = "";

            return Ok();
        }
    }
}