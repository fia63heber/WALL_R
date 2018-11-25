using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WALL_R.Models;

namespace WALL_R.Controllers
{
    [Route("api/[controller]")]
    public class GeneralController : Controller
    {
        private bool checkAuthentication()
        {
            return Libraries.SessionManager.checkAuthentication("general", HttpContext.Request.Cookies["session"]);
        }

        private room_management_dbContext getDatabaseContext()
        {
            return new room_management_dbContext();
        }

        [HttpGet("rooms")]
        public IActionResult GetRooms()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getDatabaseContext();

                // Get all rooms:
                List<Rooms> rooms = context.Rooms.ToList();

                // Check if list of rooms is empty and if so send "404 - Not Found" to the frontend:
                if (rooms.Count() == 0)
                {
                    return NotFound();
                }

                // Return a "200 - OK" success report including all rooms to the frontend:
                return Ok(rooms);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("room/{room_id}/defects")]
        public IActionResult GetDefectsForRoom(int room_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getDatabaseContext();

                // Check if room exists and if not send "404 - Not Found" to the frontend:
                if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
                {
                    return NotFound();
                }

                // Create empty list of defects
                List<Defects> defects = new List<Defects>();

                // Fill list of defects in a loop:
                foreach (Devices device in context.Devices.Where(f => f.RoomId == room_id))
                {
                    foreach (Components component in context.Components.Where(f => f.DeviceId == device.Id))
                    {
                        defects.AddRange(context.Defects.Where(f => f.ComponentId == component.Id).ToList());
                    }
                }

                // Return a "200 - OK" success report including all rooms to the frontend:
                return Ok(defects);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("devices/{room_id}")]
        public IActionResult GetDevicesForRoom(int room_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getDatabaseContext();

                // Check if room exists and if not send "404 - Not Found" to the frontend:
                if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
                {
                    return NotFound();
                }

                // Get all devices where room_id equals received room_id:
                var devices = context.Devices.Where(f => f.RoomId == room_id);

                // Check if list of devices is empty and if so send "404 - Not Found" to the frontend:
                if (devices.Count() == 0)
                {
                    return NotFound();
                }

                // Return a success report including found devices to the frontend:
                return Ok(devices);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpPost("defect")]
        public IActionResult CreateDefect(int component_id, int defect_type_id, int priority_id, string name, string entry_comment)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try {
                room_management_dbContext context = getDatabaseContext();
                Accounts user = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]);
                if (user == null)
                {
                    return Unauthorized();
                }


                // Create model for new defect :
                Defects newDefect = new Defects();
                
                // Check sent parameters:
                bool error = false;
                string error_message = "Fehlerhafte Angaben:\n";
                if (context.Components.Count(f => f.Id == component_id) == 0)
                {
                    error = true;
                    error_message += "- Es wurde kein gültiger Geräte-Komponent angegeben\n";
                }
                else
                {
                    newDefect.ComponentId = component_id;
                }

                if (context.DefectTypes.Count(f => f.Id == defect_type_id) == 0)
                {
                    error = true;
                    error_message += "- Es wurde kein gültiger Fehlertyp angegeben";
                }
                else
                {
                    newDefect.DefectTypeId = defect_type_id;
                }

                if (context.Priorities.Count(f => f.Id == priority_id) == 0)
                {
                    error = true;
                    error_message += "\n- Es wurde keine gültige Priorität angegeben";
                }
                else
                {
                    newDefect.PriorityId = priority_id;
                }
                newDefect.WriterId = user.Id;

                // Check if error occured and if so sent error message to the frontend:
                if (error)
                {
                    return NotFound(error_message);
                }

                // Fill attributes of new defect:
                newDefect.Name = name;
                newDefect.EntryComment = entry_comment;
                newDefect.StateId = 1;

                // Add new defect to database context:
                context.Add(newDefect);

                // Save changes in context to the database:
                context.SaveChanges();

                // Return a success report including found defects to the frontend:
                return Ok();
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("writer/defects")]
        public IActionResult GetDefectsForWriter()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getDatabaseContext();

                // Get user who sent request:
                Accounts user = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]);

                // Get all defects written by the user:
                List<Defects> defects = context.Defects.Where(f => f.WriterId == user.Id).ToList();

                // Check if defects is empty and if so send "404 - Not Found" to the frontend:
                if (defects.Count() == 0)
                {
                    return NotFound();
                }

                // Return a success report including found defects to the frontend:
                return Ok(defects);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }



        [HttpGet("defecttypes")]
        public IActionResult GetAllDefectTypes()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                room_management_dbContext context = getDatabaseContext();

                // Get all defecttypes
                List<DefectTypes> defect_types = context.DefectTypes.ToList();

                // Check if defects is empty and if so send "404 - Not Found" to the frontend:
                if (defect_types.Count() == 0)
                {
                    return NotFound();
                }

                // Return a success report including found devices to the frontend:
                return Ok(defect_types);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("states")]
        public IActionResult GetAllStates()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try {
                room_management_dbContext context = getDatabaseContext();

                // Get all states
                List<States> states = context.States.ToList();

                // Check if list of states is empty and if so send "404 - Not Found" to the frontend:
                if (states.Count() == 0)
                {
                    return NotFound();
                }

                // Return a success report including found defect_types to the frontend:
                return Ok(states);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("device/{device_id}/components")]
        public IActionResult GetComponentsForDevice(int device_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try {
                room_management_dbContext context = getDatabaseContext();

                // Get list of general component types
                List<ComponentTypes> general_component_types = context.ComponentTypes.Where(f => f.Name == "General").ToList();

                // Check if list of general_components is empty and if so send "404 - Not Found" to the frontend:
                if (general_component_types.Count() == 0)
                {
                    return NotFound();
                }

                // Get id of general components:
                int general_id = general_component_types.First().Id;

                // Get list of components including all but the "general" one identified by the general id:
                List<Components> components = context.Components.Where(f => f.DeviceId == device_id).Where(f => f.ComponentTypeId != general_id).ToList();

                // Check if list of components is empty and if so send "404 - Not Found" to the frontend:
                if (components.Count() == 0)
                {
                    return NotFound();
                }

                // Return a success report including found components to the frontend:
                return Ok(components);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("device/{device_id}/general_component")]
        public IActionResult GetGeneralComponentForDevice(int device_id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try {
                room_management_dbContext context = getDatabaseContext();
                
                // Get list of general component types
                List<ComponentTypes> general_component_types = context.ComponentTypes.Where(f => f.Name == "General").ToList();

                // Check if list of general components is empty and if so send "404 - Not Found" to the frontend:
                if (general_component_types.Count() == 0)
                {
                    return NotFound();
                }

                // Get general id:
                int general_id = general_component_types.First().Id;

                // Get list of generalcomponents identified by the general id:
                List<Components> components = context.Components.Where(f => f.DeviceId == device_id).Where(f => f.ComponentTypeId != general_id).ToList();

                // Check if list of components is empty and if so send "404 - Not Found" to the frontend:
                if (components.Count() == 0)
                {
                    return NotFound();
                }

                // Return a success report including the general component to the frontend:
                return Ok(components.First());
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }
        
        [HttpGet("componenttypes")]
        public IActionResult GetAllComponentTypes()
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try {
                room_management_dbContext context = getDatabaseContext();

                // Get list of component types identified by the general id:
                List<ComponentTypes> component_types = context.ComponentTypes.Where(f => f.Name != "General").ToList();

                // Check if list of component types is empty and if so send "404 - Not Found" to the frontend:
                if (component_types.Count() == 0)
                {
                    return NotFound();
                }

                // Return a success report including all component types to the frontend:
                return Ok(component_types);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }

        [HttpGet("devicetypes")]
        public IActionResult GetDeviceTypes()
        {
            room_management_dbContext context = getDatabaseContext();
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            try
            {
                // Get list of device types identified by the general id:
                List<DeviceTypes> device_types = context.DeviceTypes.ToList();

                // Check if list of device types is empty and if so send "404 - Not Found" to the frontend:
                if (device_types.Count() > 0)
                {
                    NotFound();
                }

                // Return a success report including all device types to the frontend:
                return Ok(device_types);
            }
            catch
            {
                // Return a "500 - Internal Server Error" error message to the frontend:
                return StatusCode(500);
            }
        }


    }
}