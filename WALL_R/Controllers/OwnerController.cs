﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WALL_R.Models;
using System.Web.Http.Cors;
using WALL_R.Libraries;

namespace WALL_R.Controllers
{
    [Route("api/[controller]")]
    public class OwnerController : Controller
    {
      
        private bool checkAuthentication()
        {
            return SessionManager.checkAuthentication("owner", HttpContext.Request.Cookies["session"]);
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
            try
            {
                room_management_dbContext context = getContext();

                // Get user who sent request
                int owner_id = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]).Id;

                // Create empty list of defects
                var defects = new List<Defects>();

                // Fill list of defects in a loop:
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

                // Check if list of device types is empty and if so send "404 - Not Found" to the frontend:
                if (defects.Count() > 0)
                {
                    NotFound();
                }

                // Return a success report including all defects to the frontend:
                return Ok(defects);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpPost("defect/changePriority")]
        public IActionResult changePriorityForDefect(int defect_id, int priority_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();

                // Create empty list of defects:
                List<Defects> defects = context.Defects.Where(f => f.Id == defect_id).ToList();

                // Check if priority id exists and if not send "404 - Not Found" to the frontend:
                if (context.Priorities.Where(f => f.Id == priority_id).Count() == 0)
                {
                    return NotFound();
                }

                // Check if list of defects is empty and if so send "404 - Not Found" to the frontend:
                if (defects.Count() == 0)
                {
                    return NotFound();
                }

                // Get defect that has to be changed:
                Defects defect = defects.First();

                // Set new priority id for defect:
                defect.PriorityId = priority_id;

                // Change entry in database context:
                context.Update(defect);

                // Save changes in context to database:
                context.SaveChanges();

                // Return a success report to the frontend:
                return Ok();
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpPost("room/planFile")]
        public IActionResult addPlanfileForRoom(int room_id, string file_content)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();

                List <Rooms> rooms = context.Rooms.Where(f => f.Id == room_id).ToList();

                // Check if room id exists and if not send "404 - Not Found" to the frontend:
                if (rooms.Count() == 0)
                {
                    return NotFound();
                }

                // Get id of room plan files:
                int file_id = rooms.First().PictureFileId;

                // Get list of files:
                List<Files> files = context.Files.Where(f => f.Id == file_id).ToList();

                // Create physical file:
                string file_name = "roomplan" + room_id + ".jpeg";
                if (!FileManager.CreateFile(file_name, file_content))
                {
                    // Return a "500 - Internal Server Error" error message to the frontend:
                    return StatusCode(500);
                }
                
                // Check if list of files is empty:
                if (files.Count == 0)
                {
                    // Create model for new file:
                    Files newFile = new Files();


                    newFile.FilePath = "";
                    context.Add(newFile);
                }
                else {
                    Files file = files.First();

                    file.FilePath = "";

                    context.Update(file);
                }

                context.SaveChanges();
                return Ok();
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("room/{room_id}/planFile")]
        public IActionResult getPlanfileForRoom(int room_id)
        {
            string file_name = "roomplan" + room_id + ".jpeg";
            var image = System.IO.File.OpenRead(FileManager.GetFilePath(file_name));
            return File(image, "image/jpeg");
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