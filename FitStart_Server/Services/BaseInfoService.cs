using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class BaseInfoService : IBaseInfoService
    {
        private readonly ContextDb _context;
        public BaseInfoService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetClubs()
        {
            var clubs = await _context.Clubs.ToListAsync();
            if (clubs == null || clubs.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Клубы не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о клубах получены",
                clubList = clubs
            });
        }

        public async Task<IActionResult> GetClubById(int ClubID)
        {
            var club = await _context.Clubs.FirstOrDefaultAsync(x => x.ClubID == ClubID);
            if (club == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Клуб не найден"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о клубе получены",
                club = club
            });
        }

        public async Task<IActionResult> GetClubLoad(int ClubID, int dayOfWeek)
        {
            var club = await _context.Clubs.FindAsync(ClubID);
            if (club == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Клуб не найден"
                });
            }

            if (dayOfWeek < 0 || dayOfWeek > 6)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Некорректный день недели"
                });
            }

            var loads = await _context.ClubLoads
                .Where(x => x.ClubID == ClubID && x.DayOfWeek == dayOfWeek)
                .OrderBy(x => x.Hour)
                .ToListAsync();

            if (loads == null || loads.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Данные о загруженности клуба не найдены"
                });
            }

            int currentHour = DateTime.Now.Hour;
            var currentLoad = loads.FirstOrDefault(x => x.Hour == currentHour);

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о загруженности клуба получены",
                loads = loads,
                currentLoad = currentLoad
            });
        }

        public async Task<IActionResult> GetActiveBanners()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var banners = await _context.Banners
                .Where(x => x.isActive && x.StartDate <= today && x.EndDate >= today)
                .ToListAsync();

            if (banners == null || banners.Count == 0)
            {
                return new OkObjectResult(new
                {
                    status = true,
                    message = "Активные баннеры отсутствуют",
                    bannerList = new object[0]
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Активные баннеры получены",
                bannerList = banners
            });
        }

        public async Task<IActionResult> GetTrainerCategories()
        {
            var cats = await _context.TrainerCategories.ToListAsync();
            if (cats == null || cats.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Категории тренеров не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Категории тренеров получены",
                categoryList = cats
            });
        }

        public async Task<IActionResult> GetExerciseTypes()
        {
            var types = await _context.ExerciseTypes.ToListAsync();
            if (types == null || types.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Типы упражнений не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Типы упражнений получены",
                typeList = types
            });
        }

        public async Task<IActionResult> GetMuscleGroups()
        {
            var groups = await _context.MuscleGroups.ToListAsync();
            if (groups == null || groups.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Группы мышц не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Группы мышц получены",
                muscleGroupList = groups
            });
        }

        public async Task<IActionResult> GetEquipmentTypes()
        {
            var types = await _context.TrainingEquipTypes.ToListAsync();
            if (types == null || types.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Типы тренажёров не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Типы тренажёров получены",
                typeList = types
            });
        }

        public async Task<IActionResult> GetWorkoutTypes()
        {
            var types = await _context.WorkoutTypes.ToListAsync();
            if (types == null || types.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Категории тренировок не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Категории тренировок получены",
                typeList = types
            });
        }

        public async Task<IActionResult> GetClasses()
        {
            var classes = await _context.Classes.ToListAsync();
            if (classes == null || classes.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Виды групповых занятий не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Виды групповых занятий получены",
                classList = classes
            });
        }
    }
}
