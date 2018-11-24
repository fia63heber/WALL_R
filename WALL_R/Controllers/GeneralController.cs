﻿using System;
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
                var rooms = context.Rooms;

                // Check if rooms is empty and if so send "404 - Not Found" to the frontend:
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

                // Create empty 
                List<Defects> defects = new List<Defects>();

                foreach (Devices device in context.Devices.Where(f => f.RoomId == room_id))
                {
                    foreach (Components component in context.Components.Where(f => f.DeviceId == device.Id))
                    {
                        defects.AddRange(context.Defects.Where(f => f.ComponentId == component.Id).ToList());
                    }
                }

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
            // Get database context
            room_management_dbContext context = getDatabaseContext();
            // Check if room exists
            if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
            {
                return NotFound();
            }
            // Get all devices where room_id equals received room_id:
            var devices = context.Devices.Where(f => f.RoomId == room_id);
            if (devices.Count() == 0)
            {
                return NotFound();
            }
            // Return a success report including found devices to the frontend:
            return Ok(devices);
        }

        [HttpGet("device/{id}")]
        public IActionResult GetDevice(int id)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            room_management_dbContext context = getDatabaseContext();
            var device = context.Devices.Where(f => f.Id == id);
            if (device.Count() == 0)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpPost("defect")]
        public IActionResult CreateDefect(int component_id, int defect_type_id, int priority_id, string name, string entry_comment)
        {
            if (!checkAuthentication())
            {
                return Unauthorized();
            }
            room_management_dbContext context = getDatabaseContext();
            Accounts user = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]);
            if (user == null)
            {
                return Unauthorized();
            }
            Defects newDefect = new Defects();

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

            if (error)
            {
                return NotFound(error_message);
            }

            newDefect.Name = name;
            newDefect.EntryComment = entry_comment;
            newDefect.StateId = 1;
            context.Add(newDefect);
            context.SaveChanges();

            return Ok();
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
                Accounts user = Libraries.SessionManager.getAccountForSession(HttpContext.Request.Cookies["session"]);

                return Ok(context.Defects);
            }
            catch
            {
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

                return Ok(context.DefectTypes);
            }
            catch
            {
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

            return Ok(context.States);
            }
            catch
            {
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
            
                int general_id = context.Components.Where(f => f.Name == "General").First().Id;

                return Ok(context.Components.Where(f => f.DeviceId == device_id).Where(f => f.ComponentTypeId != general_id));
            }
            catch
            {
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

                
                int general_id = context.ComponentTypes.Where(f => f.Name == "General").First().Id;

                return Ok(context.Components.Where(f => f.DeviceId == device_id).Where(f => f.ComponentTypeId == general_id));
            }
            catch
            {
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
                
                List<ComponentTypes> component_types = context.ComponentTypes.Where(f => f.Name != "General").ToList();
                if (component_types.Count() == 0)
                {
                    return NotFound();
                }
                return Ok(component_types);
            }
            catch
            {
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
                List<DeviceTypes> device_types = context.DeviceTypes.ToList();

                if (device_types.Count() > 0)
                {
                    NotFound();
                }
                return Ok(device_types);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}