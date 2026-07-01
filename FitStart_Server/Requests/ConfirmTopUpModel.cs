namespace FitStart_Server.Requests
{
    /// <summary>
    /// Запрос подтверждения пополнения баланса при возврате пользователя
    /// в приложение после оплаты в ЮKassa. PaymentId — идентификатор платежа
    /// ЮKassa, полученный при его создании (метод TopUpBalance).
    /// </summary>
    public class ConfirmTopUpModel
    {
        public int UserID { get; set; }
        public string PaymentId { get; set; }
    }
}
