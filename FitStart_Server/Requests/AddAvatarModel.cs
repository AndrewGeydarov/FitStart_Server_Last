namespace FitStart_Server.Requests
{
    public class AddAvatarModel
    {
        public int UserID { get; set; }
        public IFormFile File { get; set; }
    }
}
