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
        public room_managementContext getContext()
        {
            return new room_managementContext();
        }

        [HttpGet("rooms")]
        public IActionResult GetRooms()
        {
            room_managementContext context = getContext();
            var rooms = context.Rooms;

            return Ok(rooms);
        }

        [HttpGet("defects/{room_id}")]
        public IActionResult GetDefectsForRoom(int room_id)
        {
            room_managementContext context = getContext();
            //check if room exists
            if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
            {
                return NotFound();
            }
            var defects = new List<Defects>();

            foreach (Devices device in context.Devices.Where(f => f.RoomId == room_id))
            {
                foreach (Components component in context.Components.Where(f => f.DeviceId == device.Id))
                {
                    defects = context.Defects.Where(f => f.ComponentId == component.Id).ToList();
                    //defects.AddRange(context.Defects.Where(f => f.ComponentId == component.Id).ToList());
                }
            }



            if (defects.Count() == 0)
            {
                return NotFound();
            }
            return Ok(defects);
        }

        [HttpGet("device/{id}")]
        public IActionResult GetDevice(int id)
        {
            room_managementContext context = getContext();
            var device = context.Devices.Where(f => f.Id == id);
            if (device.Count() == 0)
            {
                return NotFound();
            }

            return Ok(device);
        }

        [HttpGet("devices/{room_id}")]
        public IActionResult GetDevicesForRoom(int room_id)
        {
            room_managementContext context = getContext();
            //check if room exists
            if (context.Rooms.Where(f => f.Id == room_id).Count() == 0)
            {
                return NotFound();
            }

            var devices = context.Devices.Where(f => f.RoomId == room_id);
            if (devices.Count() == 0)
            {
                return NotFound();
            }
            return Ok(devices);
        }
    }
}