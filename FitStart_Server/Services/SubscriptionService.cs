using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Models;
using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ContextDb _context;
        public SubscriptionService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetAvailableSubscriptions()
        {
            var subs = await _context.Subscriptions.ToListAsync();
            if (subs == null || subs.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тарифы не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Список доступных тарифов получен",
                subscriptionList = subs
            });
        }

        public async Task<IActionResult> PurchaseSubscription(PurchaseSubscriptionModel model)
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

            var subscription = await _context.Subscriptions.FindAsync(model.SubscriptionID);
            if (subscription == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тариф не найден"
                });
            }

            var club = await _context.Clubs.FindAsync(model.ClubID);
            if (club == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Клуб не найден"
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

            var today = DateOnly.FromDateTime(DateTime.Today);
            var existingActive = await _context.UserSubscriptions
                .FirstOrDefaultAsync(x => x.UserID == model.UserID && x.isActive && x.EndDate >= today);
            if (existingActive != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "У Вас уже есть активная подписка"
                });
            }

            double totalCost = subscription.Cost + subscription.EntryFee;

            if (user.Balance < totalCost)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Недостаточно средств на внутреннем балансе"
                });
            }

            user.Balance -= totalCost;

            User_Subscription us = new User_Subscription()
            {
                UserID = model.UserID,
                SubscriptionID = model.SubscriptionID,
                SubDate = DateTime.UtcNow,
                isActive = true,
                ActivationDate = today,
                EndDate = today.AddDays(subscription.DurationDays),
                NextPaymentDate = today.AddDays(subscription.DurationDays),
                ClubID = model.ClubID,
                PaymentMethod = model.PaymentMethod,
                PromoAction = string.IsNullOrWhiteSpace(model.PromoAction) ? "Базовая" : model.PromoAction
            };

            Payment payment = new Payment()
            {
                UserID = model.UserID,
                Amount = totalCost,
                PaymentDate = DateTime.UtcNow,
                PaymentType = "Subscription",
                PaymentMethod = model.PaymentMethod
            };

            await _context.AddAsync(us);
            await _context.AddAsync(payment);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Подписка успешно оформлена",
                totalCost = totalCost
            });
        }

        public async Task<IActionResult> GetUserActiveSubscription(int UserID)
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
            var us = await _context.UserSubscriptions
                .Include(x => x.Subscription)
                .Include(x => x.Club)
                .Where(x => x.UserID == UserID && x.isActive && x.EndDate >= today)
                .OrderByDescending(x => x.SubDate)
                .FirstOrDefaultAsync();

            if (us == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Активная подписка не найдена"
                });
            }

            var info = new ReturnSubscriptionInfoModel
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

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о подписке получены",
                subscriptionInfo = info
            });
        }

        public async Task<IActionResult> PaySubscription(int US_ID)
        {
            var us = await _context.UserSubscriptions
                .Include(x => x.Subscription)
                .FirstOrDefaultAsync(x => x.US_ID == US_ID);
            if (us == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Подписка не найдена"
                });
            }

            if (!us.isActive)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Подписка не активна"
                });
            }

            var user = await _context.Users.FindAsync(us.UserID);
            if (user == null || user.Balance < us.Subscription.Cost)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Недостаточно средств на внутреннем балансе для оплаты подписки"
                });
            }

            user.Balance -= us.Subscription.Cost;

            us.NextPaymentDate = us.NextPaymentDate.AddDays(us.Subscription.DurationDays);
            us.EndDate = us.EndDate.AddDays(us.Subscription.DurationDays);

            Payment payment = new Payment()
            {
                UserID = us.UserID,
                Amount = us.Subscription.Cost,
                PaymentDate = DateTime.UtcNow,
                PaymentType = "SubscriptionPayment",
                PaymentMethod = us.PaymentMethod
            };

            await _context.AddAsync(payment);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Подписка успешно оплачена",
                nextPaymentDate = us.NextPaymentDate,
                endDate = us.EndDate
            });
        }

        public async Task<IActionResult> FreezeSubscription(FreezeSubscriptionModel model)
        {
            var us = await _context.UserSubscriptions.FindAsync(model.US_ID);
            if (us == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Подписка не найдена"
                });
            }

            if (!us.isActive)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Нельзя заморозить неактивную подписку"
                });
            }

            if (model.FreezeDays < 1 || model.FreezeDays > 30)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Срок заморозки должен быть от 1 до 30 дней"
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (model.FreezeStart < today)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Дата начала заморозки не может быть в прошлом"
                });
            }

            var activeFreeze = await _context.SubscriptionFreezes
                .FirstOrDefaultAsync(x => x.US_ID == model.US_ID && x.isActive);
            if (activeFreeze != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Подписка уже заморожена"
                });
            }

            Subscription_Freeze freeze = new Subscription_Freeze()
            {
                US_ID = model.US_ID,
                FreezeStart = model.FreezeStart,
                FreezeEnd = model.FreezeStart.AddDays(model.FreezeDays),
                FreezeDays = model.FreezeDays,
                isActive = true
            };

            us.EndDate = us.EndDate.AddDays(model.FreezeDays);
            us.NextPaymentDate = us.NextPaymentDate.AddDays(model.FreezeDays);

            await _context.AddAsync(freeze);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = $"Подписка заморожена на {model.FreezeDays} дней",
                freeze = freeze
            });
        }

        public async Task<IActionResult> UnfreezeSubscription(int FreezeID)
        {
            var freeze = await _context.SubscriptionFreezes
                .Include(x => x.User_Subscription)
                .FirstOrDefaultAsync(x => x.FreezeID == FreezeID);

            if (freeze == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Заморозка не найдена"
                });
            }

            if (!freeze.isActive)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Заморозка уже не активна"
                });
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            int actualFreezeDays = today.DayNumber - freeze.FreezeStart.DayNumber;
            if (actualFreezeDays < 0) actualFreezeDays = 0;
            int unusedDays = freeze.FreezeDays - actualFreezeDays;

            freeze.isActive = false;
            freeze.FreezeEnd = today;

            if (unusedDays > 0)
            {
                freeze.User_Subscription.EndDate = freeze.User_Subscription.EndDate.AddDays(-unusedDays);
                freeze.User_Subscription.NextPaymentDate = freeze.User_Subscription.NextPaymentDate.AddDays(-unusedDays);
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Заморозка снята",
                unusedDays = unusedDays
            });
        }

        public async Task<IActionResult> GetFreezeHistory(int US_ID)
        {
            var us = await _context.UserSubscriptions.FindAsync(US_ID);
            if (us == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Подписка не найдена"
                });
            }

            var freezes = await _context.SubscriptionFreezes
                .Where(x => x.US_ID == US_ID)
                .OrderByDescending(x => x.FreezeStart)
                .ToListAsync();

            if (freezes == null || freezes.Count == 0)
            {
                return new OkObjectResult(new
                {
                    status = true,
                    message = "История заморозок пуста",
                    freezeList = new object[0]
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "История заморозок получена",
                freezeList = freezes
            });
        }

        public async Task<IActionResult> ChangeClub(ChangeClubModel model)
        {
            var us = await _context.UserSubscriptions.FindAsync(model.US_ID);
            if (us == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Подписка не найдена"
                });
            }

            var club = await _context.Clubs.FindAsync(model.ClubID);
            if (club == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Клуб не найден"
                });
            }

            us.ClubID = model.ClubID;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Клуб подписки успешно изменен"
            });
        }

        public async Task<IActionResult> ChangePaymentMethod(ChangePaymentMethodModel model)
        {
            var us = await _context.UserSubscriptions.FindAsync(model.US_ID);
            if (us == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Подписка не найдена"
                });
            }

            if (string.IsNullOrWhiteSpace(model.PaymentMethod))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Необходимо указать способ оплаты"
                });
            }

            us.PaymentMethod = model.PaymentMethod;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Способ оплаты успешно изменен"
            });
        }

        public async Task<IActionResult> CancelSubscription(int US_ID)
        {
            var us = await _context.UserSubscriptions.FindAsync(US_ID);
            if (us == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Подписка не найдена"
                });
            }

            if (!us.isActive)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Подписка уже отменена"
                });
            }

            us.isActive = false;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Подписка успешно отменена"
            });
        }
    }
}
