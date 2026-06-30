namespace FitStart_Server.Requests
{
    public class EditUserModel
    {
        public int UserID { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Password { get; set; }
    }
}
