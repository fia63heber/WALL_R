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

        [HttpPost("owner/defects")]
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
                //context.Add()
            }

            context.Files.Where(f => f.Id == file_id).First().FilePath = "";

            return Ok();
        }

        [HttpPost("room/addDevice")]
        public IActionResult addDeviceForRoom(int room_id, int device_id, int device_type_id, string name)
        {
            room_management_dbContext context = getContext();
            bool error = false;
            string error_message = "Fehlerhafte Angaben:\n";

            if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
            {
                error = true;
                error_message += "- Es wurde kein gültiger Raum angegeben\n";
            }
            if (context.DeviceTypes.Where(f => f.Id == device_type_id).Count() == 0)
            {
                error = true;
                error_message += "- Es wurde kein gültiger Geräte-Typ angegeben\n";
            }

            if (error)
            {
                return NotFound(error_message);
            }
            Devices device = new Devices();
            device.RoomId = room_id;
            device.DeviceTypeId = device_type_id;
            device.Name = name;

            context.Devices.Add(device);
            context.SaveChanges();
            return Ok();
        }

        [HttpPost("device/addComponent")]
        public IActionResult addComponentForDevice(int device_id, int component_id, int component_type_id, string name, string serial_number)
        {
            room_management_dbContext context = getContext();
            bool error = false;
            string error_message = "Fehlerhafte Angaben:\n";

            if (context.Devices.Where(f => f.Id == device_id).Count() == 0)
            {
                error = true;
                error_message += "- Es wurde kein gültiges Gerät angegeben\n";
            }
            if (context.ComponentTypes.Where(f => f.Id == component_type_id).Count() == 0)
            {
                error = true;
                error_message += "- Es wurde kein gültiger Komponent-Typ angegeben\n";
            }

            if (error)
            {
                return NotFound(error_message);
            }
            Components component = new Components();
            component.DeviceId = device_id;
            component.ComponentTypeId = component_type_id;
            component.Name = name;

            context.Components.Add(component);
            context.SaveChanges();
            return Ok();
        }

        [HttpGet("defecttypes")]
        public IActionResult GetAllDefectTypes()
        {
            room_management_dbContext context = getContext();

            return Ok(context.DefectTypes);
        }

        [HttpDelete("defecttype")]
        public IActionResult DeleteDefectType(int defect_type_id)
        {
            room_management_dbContext context = getContext();

            DefectTypes defectTypeToDelete = context.DefectTypes.Where(f => f.Id == defect_type_id).First();
            context.DefectTypes.Remove(defectTypeToDelete);
            context.SaveChanges();

            return Ok();
        }


        [HttpPost("defecttype")]
        public IActionResult UpdateDefectType(string defect_type_name)
        {
            room_management_dbContext context = getContext();

            DefectTypes defectType = new DefectTypes();
            defectType.Name = defect_type_name;

            context.DefectTypes.Add(defectType);
            context.SaveChanges();

            return Ok();
        }  
    }
}