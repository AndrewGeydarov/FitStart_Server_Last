using FitStart_Server.Connection;
using FitStart_Server.Models;
using FitStart_Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FitStart_Server.Controllers
{
    [ApiController]
    [Route("yookassa")]
    public class YooKassaController : ControllerBase
    {
        private readonly ContextDb _context;
        public YooKassaController(ContextDb contextDb)
        {
            _context = contextDb;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement payload)
        {
            try
            {
                if (payload.TryGetProperty("event", out var eventProperty))
                {
                    string eventName = eventProperty.GetString();
                    if (eventName == "payment.succeeded")
                    {
                        var paymentObject = payload.GetProperty("object");
                        bool isPaid = paymentObject.GetProperty("paid").GetBoolean();
                        
                        if (isPaid)
                        {
                            var metadata = paymentObject.GetProperty("metadata");
                            if (metadata.TryGetProperty("userId", out var userIdProperty))
                            {
                                int userId = int.Parse(userIdProperty.GetString());
                                double amount = double.Parse(paymentObject.GetProperty("amount").GetProperty("value").GetString(), System.Globalization.CultureInfo.InvariantCulture);
                                string paymentMethod = metadata.TryGetProperty("paymentMethod", out var pmProperty) ? pmProperty.GetString() : "ЮKassa";

                                string paymentId = paymentObject.TryGetProperty("id", out var idProperty)
                                    ? idProperty.GetString()
                                    : null;
                                if (!TopUpIdempotency.TryBeginCredit(paymentId))
                                {
                                    return Ok();
                                }

                                var user = await _context.Users.FindAsync(userId);
                                if (user != null)
                                {
                                    user.Balance += amount;

                                    Payment payment = new Payment()
                                    {
                                        UserID = userId,
                                        Amount = amount,
                                        PaymentDate = DateTime.UtcNow,
                                        PaymentType = "TopUp",
                                        PaymentMethod = paymentMethod
                                    };
                                    
                                    await _context.Payments.AddAsync(payment);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
