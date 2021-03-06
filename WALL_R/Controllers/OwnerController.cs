﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WALL_R.Models;
using System.Web.Http.Cors;
using WALL_R.Libraries;
using System.Text.Encodings;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using System.Text;

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
                int owner_id = SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]).Id;

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

                // Check if list of defects is empty and if so send "404 - Not Found" to the frontend:
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
        public IActionResult addPlanfileForRoom(int room_id)
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
                int? file_id = rooms.First().PictureFileId;

                // Get list of files:
                List<Files> files = context.Files.Where(f => f.Id == file_id).ToList();

                // Get file_content from request body:
                string bodyStr = "";
                var req = HttpContext.Request;
                
                req.EnableRewind();

                using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = reader.ReadToEnd();
                }
                req.Body.Position = 0;

                var headers = bodyStr.Split(',');
                string file_content = "";

                foreach (string header in headers)
                {
                    var key_value_pair = header.Replace("{\"", "").Replace("\"}", "").Replace("\"","").Split(':');
                    if (key_value_pair[0] == "file_content")
                    {
                        string test = "test";
                        file_content = key_value_pair[1];
                        break;
                    }
                }

                // Try to create physical file and return a "500 - Internal Server Error" error message to the frontend if it fails:
                string file_name = "roomplan" + room_id;


                if (!FileManager.CreateFile(file_name, file_content))
                {
                    return StatusCode(500);
                }
                
                // Check if list of files is empty:
                if (files.Count == 0)
                {
                    // Create model for new file:
                    Files newFile = new Files();

                    // Set path for new file:
                    newFile.FilePath = "Files/roomplan" + room_id;

                    // Add new file to the database context, track it and save:
                    var fileTracking = context.Add(newFile);
                    context.SaveChanges();

                    // Change file id for room to new file
                    context.Rooms.Where(f => f.Id == room_id).First().PictureFileId = fileTracking.Entity.Id;
                }
                else {
                    // Get file that has to be changed:
                    Files file = files.First();

                    // Set new path for file:
                    file.FilePath = "Files/roomplan" + room_id;

                    // Change file in database context:
                    context.Update(file);
                }

                // Save changes in context to the database:
                context.SaveChanges();

                // Return a success report to the frontend:
                return Ok();
            }
            catch(Exception ex)
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("room/{room_id}/planFile")]
        public IActionResult getPlanfileForRoom(int room_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();

                // Get list of rooms by given room id
                List<Rooms> rooms = context.Rooms.Where(f => f.Id == room_id).ToList();

                // Check if list of rooms is empty and if so send "404 - Not Found" to the frontend:
                if (rooms.Count() == 0)
                {
                    return NotFound();
                }

                // Get file id
                int? file_id = rooms.First().PictureFileId;

                // Get list of rooms by file id
                List<Files> files = context.Files.Where(f => f.Id == file_id).ToList();

                // Check if list of files is empty and if so send "404 - Not Found" to the frontend:
                if (files.Count() == 0)
                {
                    return NotFound();
                }

                // Get file path from database:
                string file_path = files.First().FilePath;


                // Return file
                return Ok(FileManager.GetFileBody(file_path));
            }
            catch(Exception ex)
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpPost("room/addDevice")]
        public IActionResult addDeviceForRoom(int room_id, int device_type_id, string name, string serial_number)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();

                // Check sent parameters:
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

                // Check if error occured and if so sent error message to the frontend:
                if (error)
                {
                    return NotFound(error_message);
                }
                
                // Create model for new device:
                Devices device = new Devices();

                // Set attributes for new device:
                device.RoomId = room_id;
                device.DeviceTypeId = device_type_id;
                device.Name = name;
                device.SerialNumber = serial_number;

                // Add device to database context and save tracking data to get device id later:
                var deviceTracking = context.Devices.Add(device);

                // Save changes in context to the database:
                context.SaveChanges();

                // Create model for general component:
                Components newComponent = new Components();
                
                // Set attributes for new device:
                newComponent.DeviceId = deviceTracking.Entity.Id;
                newComponent.ComponentTypeId = 1;
                newComponent.Name = "General Dummy";

                // Add device to new component to database context:
                context.Add(newComponent);
                
                // Save changes in context to the database:
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

        [HttpPost("device/addComponent")]
        public IActionResult addComponentForDevice(int device_id, int component_type_id, string name)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();

                // Check sent parameters:
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

                // Check if error occured and if so sent error message to the frontend:
                if (error)
                {
                    return NotFound(error_message);
                }

                // Create model for new component:
                Components component = new Components();
                
                // Set attributes for new component:
                component.DeviceId = device_id;
                component.ComponentTypeId = component_type_id;
                component.Name = name;

                // Add device to new component to database context:
                context.Components.Add(component);

                // Save changes in context to the database:
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

        [HttpDelete("defecttype")]
        public IActionResult DeleteDefectType(int defect_type_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();

                // Get defect type that we want to delete
                DefectTypes defectTypeToDelete = context.DefectTypes.Where(f => f.Id == defect_type_id).First();
                context.DefectTypes.Remove(defectTypeToDelete);

                // Save changes in context to the database:
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
        
        [HttpPost("defecttype")]
        public IActionResult UpdateDefectType(string defect_type_name)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getContext();
                
                // Create model for new defect type
                DefectTypes defectType = new DefectTypes();

                // Set name for new defect type
                defectType.Name = defect_type_name;

                // Add new defect type to database context
                context.DefectTypes.Add(defectType);

                // Save changes in context to the database
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

                // Get list of defects by defect id:
                List<Defects> defects = context.Defects.Where(f => f.Id == defect_id).ToList();

                // Check if list of defects is empty or state id is invalid. If one is true send error message to the frontend:
                if (defects.Count == 0 || context.States.Where(f => f.Id == state_id).Count() == 0)
                {
                    return NotFound();
                }

                // Get specific defect:
                Defects defect = defects.First();

                // Send mail to workshop if state has been changed to "waiting for workshop" DISABLED FOR DEVELOPEMENT
                if (state_id == 2 && defect.Id != 2 && false)
                {
                    // Get additional information for defect and send error message to the frontend if data is missing:
                    List<Components> components = context.Components.Where(f => f.Id == defect.ComponentId).ToList();
                    if (components.Count() == 0)
                    {
                        return NotFound();
                    }
                    List<Devices> devices = context.Devices.Where(f => f.Id == components.First().DeviceId).ToList();
                    if (devices.Count() == 0)
                    {
                        return NotFound();
                    }
                    Devices device = devices.First();
                    List<Rooms> rooms = context.Rooms.Where(f => f.Id == device.RoomId).ToList();
                    if (rooms.Count() == 0)
                    {
                        return NotFound();
                    }
                    Rooms room = rooms.First();
                    List<Accounts> accounts = context.Accounts.Where(f => f.Id == room.OwnerId).ToList();
                    if (accounts.Count() == 0)
                    {
                        return NotFound();
                    }

                    // Send mail with additional information 
                    MailManager.SendRepairMail(room.RoomNumber, device.Name, accounts.First().Email);
                }

                // Set state of defect to given state
                defect.StateId = state_id;

                // Add comment by the room owner
                defect.OwnerComment = owner_comment;

                // Change defect in database context:
                context.Update(defect);

                // Save context to the database:
                context.SaveChanges();

                // Return a success report to the frontend:
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