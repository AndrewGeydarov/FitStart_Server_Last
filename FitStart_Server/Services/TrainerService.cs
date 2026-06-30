using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Models;
using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class TrainerService : ITrainerService
    {
        private readonly ContextDb _context;
        public TrainerService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetTrainers(int? clubID, int? categoryID)
        {
            var query = _context.Trainers
                .Include(x => x.TrainerCategory)
                .Include(x => x.Club)
                .AsQueryable();

            if (clubID.HasValue)
                query = query.Where(x => x.ClubID == clubID.Value);

            if (categoryID.HasValue)
                query = query.Where(x => x.TrainerCategoryID == categoryID.Value);

            var trainers = await query.ToListAsync();
            if (trainers == null || trainers.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренеры не найдены"
                });
            }

            var trainerIds = trainers.Select(t => t.TrainerID).ToList();
            var responses = await _context.Responses
                .Where(x => trainerIds.Contains(x.TrainerID))
                .ToListAsync();

            var result = trainers.Select(t =>
            {
                var trRes = responses.Where(r => r.TrainerID == t.TrainerID).ToList();
                double avg = trRes.Count > 0 ? trRes.Average(r => r.ResponseRate) : 0;
                return new ReturnTrainerModel
                {
                    TrainerID = t.TrainerID,
                    LastName = t.LastName,
                    Name = t.Name,
                    Patronymic = t.Patronymic,
                    HourCost = t.HourCost,
                    WorkExperience = t.WorkExperience,
                    AboutTrainer = t.AboutTrainer,
                    PhotoPath = t.PhotoPath,
                    TrainerCategory = t.TrainerCategory,
                    Club = t.Club,
                    AverageRating = Math.Round(avg, 1),
                    ResponseCount = trRes.Count
                };
            }).ToArray();

            return new OkObjectResult(new
            {
                status = true,
                message = "Список тренеров получен",
                trainerList = result
            });
        }

        public async Task<IActionResult> GetTrainerById(int TrainerID)
        {
            var trainer = await _context.Trainers
                .Include(x => x.TrainerCategory)
                .Include(x => x.Club)
                .FirstOrDefaultAsync(x => x.TrainerID == TrainerID);

            if (trainer == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренер не найден"
                });
            }

            var responses = await _context.Responses
                .Where(x => x.TrainerID == TrainerID)
                .ToListAsync();
            double avg = responses.Count > 0 ? responses.Average(r => r.ResponseRate) : 0;

            var info = new ReturnTrainerModel
            {
                TrainerID = trainer.TrainerID,
                LastName = trainer.LastName,
                Name = trainer.Name,
                Patronymic = trainer.Patronymic,
                HourCost = trainer.HourCost,
                WorkExperience = trainer.WorkExperience,
                AboutTrainer = trainer.AboutTrainer,
                PhotoPath = trainer.PhotoPath,
                TrainerCategory = trainer.TrainerCategory,
                Club = trainer.Club,
                AverageRating = Math.Round(avg, 1),
                ResponseCount = responses.Count
            };

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о тренере получены",
                trainer = info
            });
        }

        public async Task<IActionResult> GetTrainerResponses(int TrainerID)
        {
            var trainer = await _context.Trainers.FindAsync(TrainerID);
            if (trainer == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренер не найден"
                });
            }

            var responses = await _context.Responses
                .Include(x => x.User)
                .Where(x => x.TrainerID == TrainerID)
                .OrderByDescending(x => x.ResponseDate)
                .ToListAsync();

            if (responses == null || responses.Count == 0)
            {
                return new OkObjectResult(new
                {
                    status = true,
                    message = "У тренера пока нет отзывов",
                    responseList = new object[0]
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Отзывы о тренере получены",
                responseList = responses
            });
        }

        public async Task<IActionResult> AddResponse(AddResponseModel model)
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

            var trainer = await _context.Trainers.FindAsync(model.TrainerID);
            if (trainer == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренер не найден"
                });
            }

            if (string.IsNullOrWhiteSpace(model.ResponseContent))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Напишите текст отзыва"
                });
            }

            if (model.ResponseRate < 1 || model.ResponseRate > 5)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Оценка должна быть в диапазоне от 1 до 5"
                });
            }

            var hadTraining = await _context.PersonalTrainings
                .AnyAsync(x => x.UserID == model.UserID && x.TrainerID == model.TrainerID && !x.isCancelled
                    && (x.Date < DateOnly.FromDateTime(DateTime.Today)
                        || (x.Date == DateOnly.FromDateTime(DateTime.Today) && x.StartTime <= TimeOnly.FromDateTime(DateTime.Now))));
            if (!hadTraining)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Оставить отзыв можно только после состоявшейся тренировки с этим тренером"
                });
            }

            var existing = await _context.Responses
                .FirstOrDefaultAsync(x => x.UserID == model.UserID && x.TrainerID == model.TrainerID);
            if (existing != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Вы уже оставляли отзыв о данном тренере"
                });
            }

            Response response = new Response()
            {
                UserID = model.UserID,
                TrainerID = model.TrainerID,
                ResponseRate = model.ResponseRate,
                ResponseContent = model.ResponseContent,
                ResponseDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _context.AddAsync(response);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Отзыв успешно добавлен"
            });
        }

        public async Task<IActionResult> BookPersonalTraining(PersonalTrainingModel model)
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

            var trainer = await _context.Trainers.FindAsync(model.TrainerID);
            if (trainer == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренер не найден"
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (model.Date < today || (model.Date == today && model.StartTime <= TimeOnly.FromDateTime(DateTime.Now)))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Невозможно забронировать тренировку на прошедшее время"
                });
            }

            if (model.DurationMinutes <= 0)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Длительность тренировки должна быть положительной"
                });
            }

            var endTime = model.StartTime.AddMinutes(model.DurationMinutes);
            var conflict = await _context.PersonalTrainings
                .Where(x => x.TrainerID == model.TrainerID && x.Date == model.Date && !x.isCancelled)
                .ToListAsync();

            foreach (var pt in conflict)
            {
                var existingEnd = pt.StartTime.AddMinutes(pt.DurationMinutes);
                if (model.StartTime < existingEnd && pt.StartTime < endTime)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "В выбранное время тренер уже занят"
                    });
                }
            }

            double cost = trainer.HourCost * (model.DurationMinutes / 60.0);

            if (user.Balance < cost)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Недостаточно средств на балансе. Пополните баланс."
                });
            }

            user.Balance -= cost;

            Personal_Training personalTraining = new Personal_Training()
            {
                UserID = model.UserID,
                TrainerID = model.TrainerID,
                Date = model.Date,
                StartTime = model.StartTime,
                DurationMinutes = model.DurationMinutes,
                Cost = cost,
                isCancelled = false
            };

            Payment payment = new Payment()
            {
                UserID = model.UserID,
                Amount = cost,
                PaymentDate = DateTime.UtcNow,
                PaymentType = "PersonalTraining",
                PaymentMethod = "Balance"
            };

            await _context.AddAsync(personalTraining);
            await _context.AddAsync(payment);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Тренировка успешно забронирована",
                cost = cost,
                newBalance = user.Balance
            });
        }

        public async Task<IActionResult> CancelPersonalTraining(int personalTrainingID, int UserID)
        {
            var pt = await _context.PersonalTrainings
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.PersonalTrainingID == personalTrainingID);
            if (pt == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Бронирование не найдено"
                });
            }

            if (pt.UserID != UserID)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Нельзя отменить чужую тренировку"
                });
            }

            if (pt.isCancelled)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Бронирование уже отменено"
                });
            }

            var trainingDateTime = pt.Date.ToDateTime(pt.StartTime);
            if (trainingDateTime <= DateTime.Now.AddHours(2))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Отмена возможна не позднее, чем за 2 часа до начала тренировки"
                });
            }

            pt.isCancelled = true;
            pt.User.Balance += pt.Cost;

            Payment refund = new Payment()
            {
                UserID = UserID,
                Amount = pt.Cost,
                PaymentDate = DateTime.UtcNow,
                PaymentType = "Refund",
                PaymentMethod = "Balance"
            };

            await _context.AddAsync(refund);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Бронирование отменено, средства возвращены на баланс",
                newBalance = pt.User.Balance
            });
        }

        public async Task<IActionResult> GetUserPersonalTrainings(int UserID)
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

            var trainings = await _context.PersonalTrainings
                .Include(x => x.Trainer).ThenInclude(t => t.Club)
                .Where(x => x.UserID == UserID)
                .OrderByDescending(x => x.Date).ThenByDescending(x => x.StartTime)
                .ToListAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Персональные тренировки пользователя получены",
                trainingList = trainings
            });
        }
    }
}
