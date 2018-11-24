using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WALL_R.Models;
using System.Web.Http.Cors;

namespace WALL_R.Controllers
{
    [Route("api/[controller]")]
    public class OwnerController : Controller
    {
      
        private bool checkAuthentication()
        {
            return Libraries.SessionManager.checkAuthentication("owner", HttpContext.Request.Cookies["session"]);
        }

        private room_management_dbContext getContext()
        {
            return new room_management_dbContext();
        }

        [HttpGet("owner/defects")]
        public IActionResult GetDefectsForOwner()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }

            int owner_id = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]).Id;

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
            if (!checkAuthentication())
            {
                return Unauthorized();
            }

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
        public IActionResult addDeviceForRoom(int room_id, int device_type_id, string name, string serial_number)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
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
            device.SerialNumber = serial_number;

            var deviceTracking = context.Devices.Add(device);
            context.SaveChanges();
            
            Components newComponent = new Components();
            newComponent.DeviceId = deviceTracking.Entity.Id;
            newComponent.ComponentTypeId = 1;
            newComponent.Name = "General Dummy";

            context.Add(newComponent);
            context.SaveChanges();
            return Ok();
        }

        [HttpPost("device/addComponent")]
        public IActionResult addComponentForDevice(int device_id, int component_type_id, string name)
        {
            room_management_dbContext context = getContext();
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
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

        [HttpDelete("defecttype")]
        public IActionResult DeleteDefectType(int defect_type_id)
        {
            room_management_dbContext context = getContext();
            if (!checkAuthentication())
            {
                return Unauthorized();
            }

            DefectTypes defectTypeToDelete = context.DefectTypes.Where(f => f.Id == defect_type_id).First();
            context.DefectTypes.Remove(defectTypeToDelete);
            context.SaveChanges();

            return Ok();
        }
        
        [HttpPost("defecttype")]
        public IActionResult UpdateDefectType(string defect_type_name)
        {
            room_management_dbContext context = getContext();
            if (!checkAuthentication())
            {
                return Unauthorized();
            }

            DefectTypes defectType = new DefectTypes();
            defectType.Name = defect_type_name;

            context.DefectTypes.Add(defectType);
            context.SaveChanges();

            return Ok();
        }

        [HttpPost("defect/changeState")]
        public IActionResult ChangeDefectState(int defect_id, int state_id, string owner_comment = "")
        {
            room_management_dbContext context = getContext();
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                List<Defects> defects = context.Defects.Where(f => f.Id == defect_id).ToList();

                if (defects.Count == 0)
                {
                    return NotFound();
                }
                if (context.States.Where(f => f.Id == state_id).Count() == 0)
                {
                    return NotFound();
                }

                Defects defect = defects.First();
                defect.StateId = state_id;
                defect.OwnerComment = owner_comment;
                context.Update(defect);
                context.SaveChanges();

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("account")]
        public IActionResult GetAccounts()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                // Get database context
                room_management_dbContext context = getContext();
                if (!checkAuthentication())
                {
                    return Unauthorized();
                }

                // Get all existing accounts:
                var accounts = context.Accounts;

                // Return a "200 - OK" success report including all accounts to the frontend:
                return Ok(accounts);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }
    }

}