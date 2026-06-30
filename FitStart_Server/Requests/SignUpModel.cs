namespace FitStart_Server.Requests
{
    public class SignUpModel
    {
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public DateOnly BirthDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
