using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Models;
using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using FitStart_Server.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FitStart_Server.Services
{
    public class UserService : IUserService
    {
        private static readonly Regex _onlyCyrillic = new(@"^[\p{IsCyrillic}]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly ContextDb _context;

        public UserService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetHomeScreen(int UserID)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .Include(x => x.Login)
                .FirstOrDefaultAsync(x => x.UserID == UserID);

            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            var us = await _context.UserSubscriptions
                .Include(x => x.Subscription)
                .Include(x => x.Club)
                .Where(x => x.UserID == UserID && x.isActive && x.EndDate >= today)
                .OrderByDescending(x => x.SubDate)
                .FirstOrDefaultAsync();

            ReturnSubscriptionInfoModel? subInfo = null;
            if (us != null)
            {
                subInfo = new ReturnSubscriptionInfoModel()
                {
                    US_ID = us.US_ID,
                    Subscription = us.Subscription,
                    isActive = us.isActive,
                    ActivationDate = us.ActivationDate,
                    EndDate = us.EndDate,
                    NextPaymentDate = us.NextPaymentDate,
                    DaysRemaining = us.EndDate.DayNumber - today.DayNumber,
                    Club = us.Club,
                    PaymentMethod = us.PaymentMethod,
                    PromoAction = us.PromoAction
                };
            }

            var banners = await _context.Banners
                .Where(x => x.isActive && x.StartDate <= today && x.EndDate >= today)
                .ToListAsync();

            var unreadCount = await _context.Notifications
                .Where(x => x.UserID == UserID && !x.isRead)
                .CountAsync();

            var nowUtc = DateTime.UtcNow;
            var todayDate = DateOnly.FromDateTime(DateTime.Today);
            var upcomingTrainings = await _context.TrainingDiaries
                .Include(x => x.Schedule).ThenInclude(s => s.Class)
                .Include(x => x.Schedule).ThenInclude(s => s.Trainer)
                .Include(x => x.Schedule).ThenInclude(s => s.Club)
                .Where(x => x.UserID == UserID && !x.isCancelled && x.Schedule.Date >= todayDate)
                .OrderBy(x => x.Schedule.Date).ThenBy(x => x.Schedule.StartTime)
                .Take(5)
                .ToListAsync();

            var upcoming = upcomingTrainings.Select(t => new ReturnScheduleItemModel
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

            ReturnHomeModel home = new ReturnHomeModel
            {
                User = user,
                ActiveSubscription = subInfo,
                Banners = banners.ToArray(),
                UnreadNotifications = unreadCount,
                UpcomingTrainings = upcoming
            };

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные главного экрана получены",
                home = home
            });
        }

        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            var thisUser = await _context.Users.Include(x => x.Login).FirstOrDefaultAsync(x => x.UserID == model.UserID);
            if (thisUser == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            if (!string.IsNullOrWhiteSpace(model.LastName))
            {
                if (!_onlyCyrillic.IsMatch(model.LastName))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "ФИО пользователя должно быть корректным и быть написанным на кириллице"
                    });
                }
                thisUser.LastName = model.LastName;
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                if (!_onlyCyrillic.IsMatch(model.Name))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "ФИО пользователя должно быть корректным и быть написанным на кириллице"
                    });
                }
                thisUser.Name = model.Name;
            }

            if (!string.IsNullOrWhiteSpace(model.Patronymic))
            {
                if (!_onlyCyrillic.IsMatch(model.Patronymic))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "ФИО пользователя должно быть корректным и быть написанным на кириллице"
                    });
                }
                thisUser.Patronymic = model.Patronymic;
            }

            if (model?.BirthDate != null && model.BirthDate != default)
            {
                if (model.BirthDate.AddYears(14) > DateOnly.FromDateTime(DateTime.Today))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Пользователь должен быть старше 14 лет"
                    });
                }
                thisUser.BirthDate = model.BirthDate;
            }

            if (!string.IsNullOrWhiteSpace(model?.Password))
            {
                if (model.Password.Length < 6)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Пароль должен содержать не менее 6 символов"
                    });
                }
                thisUser.Login.Password = model.Password;
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = $"Данные пользователя успешно обновлены"
            });
        }

        public async Task<IActionResult> DeleteUser(int UserID)
        {
            var thisUser = await _context.Users.Include(x => x.Login).FirstOrDefaultAsync(x => x.UserID == UserID);
            if (thisUser == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            var thisLogin = await _context.Logins.FirstOrDefaultAsync(x => x.LoginID == thisUser.LoginID);

            var sessions = await _context.Sessions.Where(x => x.UserID == UserID).ToListAsync();
            _context.RemoveRange(sessions);

            _context.Remove(thisUser);
            if (thisLogin != null)
                _context.Remove(thisLogin);

            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Аккаунт успешно удалён"
            });
        }

        public async Task<IActionResult> ToggleVibration(ToggleVibrationModel model)
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

            user.VibrationOnStart = model.VibrationOnStart;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Настройка обновлена",
                vibrationOnStart = user.VibrationOnStart
            });
        }

        public async Task<IActionResult> TopUpBalance(TopUpBalanceModel model)
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

            if (model.Amount <= 0)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Сумма пополнения должна быть положительной"
                });
            }

            if (string.IsNullOrWhiteSpace(model.PaymentMethod))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Необходимо выбрать способ оплаты"
                });
            }

            string shopId = "1396331"; 
            string secretKey = "test_tBWd-HlZvzVVSGMIRv9GwTJOs837-aX1obM2CO_oFLQ"; 

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{shopId}:{secretKey}")));
                httpClient.DefaultRequestHeaders.Add("Idempotence-Key", Guid.NewGuid().ToString());

                var payload = new
                {
                    amount = new
                    {
                        value = model.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                        currency = "RUB"
                    },
                    capture = true,
                    confirmation = new
                    {
                        type = "redirect",
                        return_url = "fitstart://return"
                    },
                    description = $"Пополнение баланса (пользователь {model.UserID})",
                    metadata = new
                    {
                        userId = model.UserID.ToString(),
                        paymentMethod = model.PaymentMethod
                    }
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://api.yookassa.ru/v3/payments", content);

                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Ошибка при создании платежа."
                    });
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = System.Text.Json.JsonDocument.Parse(responseString);
                var root = jsonResponse.RootElement;

                user.Balance += model.Amount;
                await _context.SaveChangesAsync();

                string confirmationUrl = root.GetProperty("confirmation").GetProperty("confirmation_url").GetString();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Платеж создан",
                    confirmationUrl = confirmationUrl
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Внутренняя ошибка при создании платежа: " + ex.Message
                });
            }
        }

        public async Task<IActionResult> GetBalance(int UserID)
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

            return new OkObjectResult(new
            {
                status = true,
                message = "Баланс получен",
                balance = user.Balance
            });
        }

        public async Task<IActionResult> GetPaymentHistory(int UserID)
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

            var payments = await _context.Payments
                .Where(x => x.UserID == UserID)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "История платежей получена",
                paymentList = payments
            });
        }

        public async Task<IActionResult> GeneratePass(int UserID)
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
            var activeSub = await _context.UserSubscriptions
                .FirstOrDefaultAsync(x => x.UserID == UserID && x.isActive && x.EndDate >= today);
            if (activeSub == null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "У пользователя нет активной подписки"
                });
            }

            var oldPasses = await _context.Passes
                .Where(x => x.UserID == UserID && !x.isUsed && x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
            _context.RemoveRange(oldPasses);

            Pass pass = new Pass()
            {
                UserID = UserID,
                QRCode = $"FITSTART-{UserID}-{Guid.NewGuid()}",
                GeneratedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                isUsed = false
            };

            await _context.AddAsync(pass);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "QR-код успешно сгенерирован",
                pass = pass
            });
        }

        public async Task<IActionResult> GetNotifications(int UserID)
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

            var notifications = await _context.Notifications
                .Where(x => x.UserID == UserID)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Уведомления получены",
                notificationList = notifications
            });
        }

        public async Task<IActionResult> MarkNotificationAsRead(int notificationID)
        {
            var notification = await _context.Notifications.FindAsync(notificationID);
            if (notification == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Уведомление не найдено"
                });
            }

            notification.isRead = true;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Уведомление прочитано"
            });
        }

        public async Task<IActionResult> AddBodyComposition(AddBodyCompositionModel model)
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

            if (model.Weight <= 0 || model.Height <= 0)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Вес и рост должны быть положительными значениями"
                });
            }

            if (model.BodyFatPercent < 0 || model.BodyFatPercent > 100 ||
                model.WaterPercent < 0 || model.WaterPercent > 100)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Процентные значения должны быть в диапазоне от 0 до 100"
                });
            }

            double heightInMeters = model.Height / 100.0;
            double bmi = model.Weight / (heightInMeters * heightInMeters);

            Body_Composition bc = new Body_Composition()
            {
                UserID = model.UserID,
                MeasureDate = model.MeasureDate,
                Weight = model.Weight,
                Height = model.Height,
                BodyFatPercent = model.BodyFatPercent,
                MuscleMass = model.MuscleMass,
                WaterPercent = model.WaterPercent,
                BMI = bmi
            };

            await _context.AddAsync(bc);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Замер успешно добавлен",
                bodyComposition = bc
            });
        }

        public async Task<IActionResult> GetBodyCompositionHistory(int UserID)
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

            var history = await _context.BodyCompositions
                .Where(x => x.UserID == UserID)
                .OrderByDescending(x => x.MeasureDate)
                .ToListAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "История замеров получена",
                historyList = history
            });
        }

        public async Task<IActionResult> GetTrainingDiary(int UserID)
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

            var diaries = await _context.TrainingDiaries
                .Include(x => x.Schedule).ThenInclude(s => s.Class)
                .Include(x => x.Schedule).ThenInclude(s => s.Trainer)
                .Include(x => x.Schedule).ThenInclude(s => s.Club)
                .Where(x => x.UserID == UserID)
                .OrderByDescending(x => x.Schedule.Date).ThenByDescending(x => x.Schedule.StartTime)
                .ToListAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Дневник тренировок получен",
                diaryList = diaries
            });
        }
    }
}
