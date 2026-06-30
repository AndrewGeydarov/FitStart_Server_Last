using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly ContextDb _context;
        public EquipmentService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetEquipments(int? clubID, int? typeID, bool? onlyAvailable)
        {
            var query = _context.TrainingEquipments
                .Include(x => x.TrainingEquip_Type)
                .Include(x => x.Club)
                .AsQueryable();

            if (clubID.HasValue)
                query = query.Where(x => x.ClubID == clubID.Value);

            if (typeID.HasValue)
                query = query.Where(x => x.TET_ID == typeID.Value);

            if (onlyAvailable.HasValue && onlyAvailable.Value)
                query = query.Where(x => x.isAvailable);

            var equipments = await query.ToListAsync();
            if (equipments == null || equipments.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренажёры по заданным параметрам не найдены"
                });
            }

            var result = equipments.Select(e => new ReturnEquipmentModel
            {
                EquipmentID = e.EquipmentID,
                EquipmentName = e.EquipmentName,
                Description = e.Description,
                Instruction = e.Instruction,
                PhotoPath = e.PhotoPath,
                VideoPath = e.VideoPath,
                isAvailable = e.isAvailable,
                Location = e.Location,
                Type = e.TrainingEquip_Type,
                Club = e.Club
            }).ToArray();

            return new OkObjectResult(new
            {
                status = true,
                message = "Список тренажёров получен",
                equipmentList = result
            });
        }

        public async Task<IActionResult> GetEquipmentById(int EquipmentID)
        {
            var equip = await _context.TrainingEquipments
                .Include(x => x.TrainingEquip_Type)
                .Include(x => x.Club)
                .FirstOrDefaultAsync(x => x.EquipmentID == EquipmentID);

            if (equip == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренажёр не найден"
                });
            }

            var info = new ReturnEquipmentModel
            {
                EquipmentID = equip.EquipmentID,
                EquipmentName = equip.EquipmentName,
                Description = equip.Description,
                Instruction = equip.Instruction,
                PhotoPath = equip.PhotoPath,
                VideoPath = equip.VideoPath,
                isAvailable = equip.isAvailable,
                Location = equip.Location,
                Type = equip.TrainingEquip_Type,
                Club = equip.Club
            };

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о тренажёре получены",
                equipment = info
            });
        }

        public async Task<IActionResult> ToggleEquipmentAvailability(int EquipmentID)
        {
            var equip = await _context.TrainingEquipments.FindAsync(EquipmentID);
            if (equip == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренажёр не найден"
                });
            }

            equip.isAvailable = !equip.isAvailable;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = equip.isAvailable ? "Тренажёр доступен" : "Тренажёр занят",
                isAvailable = equip.isAvailable
            });
        }
    }
}
