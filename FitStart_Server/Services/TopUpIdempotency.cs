using System.Collections.Generic;

namespace FitStart_Server.Services
{
    /// <summary>
    /// Простейшая защита от повторного зачисления одного и того же платежа
    /// ЮKassa в пределах процесса. И вебхук (YooKassaController), и подтверждение
    /// оплаты при возврате в приложение (UserService.ConfirmTopUp) вызывают
    /// TryBeginCredit перед начислением: реально начислит только тот вызов,
    /// который первым «застолбил» этот PaymentId. Так исключается двойное
    /// зачисление, если сработают оба пути сразу.
    ///
    /// ВАЖНО: хранилище — в оперативной памяти, поэтому защита действует только
    /// в рамках одного запущенного процесса. Для продакшена (несколько инстансов
    /// или перезапуски сервера) идентификатор обработанного платежа следует
    /// хранить в БД (например, добавить колонку YooKassaPaymentId в таблицу
    /// Payments и проверять её наличие).
    /// </summary>
    public static class TopUpIdempotency
    {
        private static readonly HashSet<string> _processed = new();
        private static readonly object _lock = new();

        /// <summary>
        /// Возвращает true, если платёж ещё не начислялся (и помечает его как
        /// обрабатываемый). Возвращает false, если платёж уже был начислен.
        /// </summary>
        public static bool TryBeginCredit(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId)) return true;
            lock (_lock)
            {
                return _processed.Add(paymentId);
            }
        }
    }
}
