using FitStart_Server.CustomAttributes;
using FitStart_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Controllers
{
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;
        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        [Route("/fitstart/equipments")]
        public async Task<IActionResult> GetEquipments([FromQuery] int? clubID, [FromQuery] int? typeID, [FromQuery] bool? onlyAvailable)
        {
            return await _equipmentService.GetEquipments(clubID, typeID, onlyAvailable);
        }

        [HttpGet]
        [Route("/fitstart/equipment/{EquipmentID}")]
        public async Task<IActionResult> GetEquipmentById(int EquipmentID)
        {
            return await _equipmentService.GetEquipmentById(EquipmentID);
        }

        [HttpPut]
        [Route("/fitstart/equipment/toggle/{EquipmentID}")]
        [RoleAuthorize([1])]
        public async Task<IActionResult> ToggleEquipmentAvailability(int EquipmentID)
        {
            return await _equipmentService.ToggleEquipmentAvailability(EquipmentID);
        }
    }
}
