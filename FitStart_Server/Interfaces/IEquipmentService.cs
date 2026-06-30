using Microsoft.AspNetCore.Mvc;

namespace FitStart_Server.Interfaces
{
    public interface IEquipmentService
    {
        Task<IActionResult> GetEquipments(int? clubID, int? typeID, bool? onlyAvailable);
        Task<IActionResult> GetEquipmentById(int EquipmentID);
        Task<IActionResult> ToggleEquipmentAvailability(int EquipmentID);
    }
}
