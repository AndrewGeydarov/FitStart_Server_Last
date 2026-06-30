using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Models;
using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly ContextDb _context;
        public ScheduleService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetSchedule(int UserID, ScheduleFilterModel filter)
        {
            var user = await _context.Users.FindAsync(UserID);
            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            var query = _context.Schedules
                .Include(x => x.Class)
                .Include(x => x.Trainer).ThenInclude(t => t.TrainerCategory)
                .Include(x => x.Trainer).ThenInclude(t => t.Club)
                .Include(x => x.Club)
                .AsQueryable();

            if (filter.Date.HasValue)
                query = query.Where(x => x.Date == filter.Date.Value);
            else
                query = query.Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today));

            if (filter.ClubID.HasValue)
                query = query.Where(x => x.ClubID == filter.ClubID.Value);

            if (filter.TimeFrom.HasValue)
                query = query.Where(x => x.StartTime >= filter.TimeFrom.Value);

            if (filter.TimeTo.HasValue)
                query = query.Where(x => x.StartTime <= filter.TimeTo.Value);

            if (filter.Intensity.HasValue)
                query = query.Where(x => x.Class.Intensity == filter.Intensity.Value);

            var schedules = await query.OrderBy(x => x.Date).ThenBy(x => x.StartTime).ToListAsync();
            if (schedules == null || schedules.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Занятий по выбранным параметрам не найдено"
                });
            }

            var scheduleIds = schedules.Select(x => x.ScheduleID).ToList();
            var diaries = await _context.TrainingDiaries
                .Where(x => scheduleIds.Contains(x.ScheduleID) && !x.isCancelled)
                .ToListAsync();

            var items = new List<ReturnScheduleItemModel>();
            foreach (var sch in schedules)
            {
                int signedUp = diaries.Count(x => x.ScheduleID == sch.ScheduleID);
                bool userSignedUp = diaries.Any(x => x.ScheduleID == sch.ScheduleID && x.UserID == UserID);
                items.Add(new ReturnScheduleItemModel
                {
                    ScheduleID = sch.ScheduleID,
                    Date = sch.Date,
                    StartTime = sch.StartTime,
                    DurationMinutes = sch.Class.DurationMinutes,
                    ClassName = sch.Class.ClassName,
                    ClassDescription = sch.Class.Description,
                    Intensity = sch.Class.Intensity,
                    Trainer = sch.Trainer,
                    Club = sch.Club,
                    MaxSlots = sch.MaxSlots,
                    FreeSlots = sch.MaxSlots - signedUp,
                    isUserSignedUp = userSignedUp
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Расписание получено",
                scheduleList = items
            });
        }

        public async Task<IActionResult> GetScheduleByDate(int UserID, DateOnly date)
        {
            return await GetSchedule(UserID, new ScheduleFilterModel { Date = date });
        }

        public async Task<IActionResult> GetScheduleItemById(int ScheduleID, int UserID)
        {
            var sch = await _context.Schedules
                .Include(x => x.Class)
                .Include(x => x.Trainer).ThenInclude(t => t.TrainerCategory)
                .Include(x => x.Club)
                .FirstOrDefaultAsync(x => x.ScheduleID == ScheduleID);

            if (sch == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Занятие не найдено"
                });
            }

            var diaries = await _context.TrainingDiaries
                .Where(x => x.ScheduleID == ScheduleID && !x.isCancelled)
                .ToListAsync();

            int signedUp = diaries.Count;
            bool userSignedUp = diaries.Any(x => x.UserID == UserID);

            var item = new ReturnScheduleItemModel
            {
                ScheduleID = sch.ScheduleID,
                Date = sch.Date,
                StartTime = sch.StartTime,
                DurationMinutes = sch.Class.DurationMinutes,
                ClassName = sch.Class.ClassName,
                ClassDescription = sch.Class.Description,
                Intensity = sch.Class.Intensity,
                Trainer = sch.Trainer,
                Club = sch.Club,
                MaxSlots = sch.MaxSlots,
                FreeSlots = sch.MaxSlots - signedUp,
                isUserSignedUp = userSignedUp
            };

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о занятии получены",
                scheduleItem = item
            });
        }

        public async Task<IActionResult> SignUpForClass(SignUpForClassModel model)
        {
            var user = await _context.Users.FindAsync(model.UserID);
            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            var sch = await _context.Schedules
                .Include(x => x.Class)
                .Include(x => x.Club)
                .FirstOrDefaultAsync(x => x.ScheduleID == model.ScheduleID);
            if (sch == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Занятие не найдено"
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (sch.Date < today || (sch.Date == today && sch.StartTime <= TimeOnly.FromDateTime(DateTime.Now)))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Невозможно записаться на прошедшее занятие"
                });
            }

            var activeSub = await _context.UserSubscriptions
                .FirstOrDefaultAsync(x => x.UserID == model.UserID && x.isActive && x.EndDate >= today);
            if (activeSub == null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Для записи на занятие необходимо приобрести подписку"
                });
            }

            var existing = await _context.TrainingDiaries
                .FirstOrDefaultAsync(x => x.UserID == model.UserID && x.ScheduleID == model.ScheduleID && !x.isCancelled);
            if (existing != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Вы уже записаны на это занятие"
                });
            }

            int signedUp = await _context.TrainingDiaries
                .CountAsync(x => x.ScheduleID == model.ScheduleID && !x.isCancelled);
            if (signedUp >= sch.MaxSlots)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Свободных мест на занятии нет"
                });
            }

            Training_Diary diary = new Training_Diary()
            {
                UserID = model.UserID,
                ScheduleID = model.ScheduleID,
                SignUpDate = DateTime.UtcNow,
                isAttended = false,
                isCancelled = false
            };

            await _context.AddAsync(diary);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Вы успешно записаны на занятие"
            });
        }

        public async Task<IActionResult> CancelClass(CancelClassModel model)
        {
            var diary = await _context.TrainingDiaries
                .Include(x => x.Schedule)
                .FirstOrDefaultAsync(x => x.UserID == model.UserID && x.ScheduleID == model.ScheduleID && !x.isCancelled);

            if (diary == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Запись на занятие не найдена"
                });
            }

            var classDateTime = diary.Schedule.Date.ToDateTime(diary.Schedule.StartTime);
            if (classDateTime <= DateTime.Now)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Невозможно отменить запись на уже состоявшееся занятие"
                });
            }

            diary.isCancelled = true;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Запись на занятие отменена"
            });
        }

        public async Task<IActionResult> GetUserUpcomingTrainings(int UserID)
        {
            var user = await _context.Users.FindAsync(UserID);
            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var trainings = await _context.TrainingDiaries
                .Include(x => x.Schedule).ThenInclude(s => s.Class)
                .Include(x => x.Schedule).ThenInclude(s => s.Trainer)
                .Include(x => x.Schedule).ThenInclude(s => s.Club)
                .Where(x => x.UserID == UserID && !x.isCancelled && x.Schedule.Date >= today)
                .OrderBy(x => x.Schedule.Date).ThenBy(x => x.Schedule.StartTime)
                .ToListAsync();

            if (trainings == null || trainings.Count == 0)
            {
                return new OkObjectResult(new
                {
                    status = true,
                    message = "Нет записей на тренировки, давай это исправим!",
                    trainingList = new object[0]
                });
            }

            var items = trainings.Select(t => new ReturnScheduleItemModel
            {
                ScheduleID = t.ScheduleID,
                Date = t.Schedule.Date,
                StartTime = t.Schedule.StartTime,
                DurationMinutes = t.Schedule.Class.DurationMinutes,
                ClassName = t.Schedule.Class.ClassName,
                ClassDescription = t.Schedule.Class.Description,
                Intensity = t.Schedule.Class.Intensity,
                Trainer = t.Schedule.Trainer,
                Club = t.Schedule.Club,
                MaxSlots = t.Schedule.MaxSlots,
                FreeSlots = 0,
                isUserSignedUp = true
            }).ToArray();

            return new OkObjectResult(new
            {
                status = true,
                message = "Предстоящие тренировки получены",
                trainingList = items
            });
        }
    }
}
