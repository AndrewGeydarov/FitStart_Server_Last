using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class FileService : IFileService
    {
        private static readonly Dictionary<string, string> _contentTypes = new()
        {
            { ".jpg", "image/jpg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".webp", "image/webp" },
            { ".mp4", "video/mp4" },
            { ".webm", "video/webm" },
            { ".mov", "video/quicktime" }
        };

        private readonly ContextDb _context;

        private readonly string _avatarsPath;
        private readonly string _trainersPath;
        private readonly string _equipmentPath;
        private readonly string _exercisesPath;
        private readonly string _workoutsPath;
        private readonly string _bannersPath;
        private readonly string _clubsPath;
        private readonly long _maxFileSize;

        public FileService(IConfiguration configuration, ContextDb contextDb)
        {
            _avatarsPath = GetFullPath(configuration, "FileStorage:AvatarsPath", "FileStorage/Avatars");
            _trainersPath = GetFullPath(configuration, "FileStorage:TrainersPath", "FileStorage/Trainers");
            _equipmentPath = GetFullPath(configuration, "FileStorage:EquipmentPath", "FileStorage/Equipment");
            _exercisesPath = GetFullPath(configuration, "FileStorage:ExercisesPath", "FileStorage/Exercises");
            _workoutsPath = GetFullPath(configuration, "FileStorage:WorkoutsPath", "FileStorage/Workouts");
            _bannersPath = GetFullPath(configuration, "FileStorage:BannersPath", "FileStorage/Banners");
            _clubsPath = GetFullPath(configuration, "FileStorage:ClubsPath", "FileStorage/Clubs");

            _maxFileSize = long.Parse(configuration["FileStorage:MaxFileSizeBytes"] ?? "10485760");

            _context = contextDb;
        }

        private string GetFullPath(IConfiguration configuration, string key, string defaultPath)
        {
            var p = configuration[key] ?? defaultPath;
            var fullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), p));
            Directory.CreateDirectory(fullPath);
            return fullPath;
        }

        public async Task<ReturnFileResult> AddAvatar(AddAvatarModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return Fail("Файл пуст");

            if (model.File.Length > _maxFileSize)
                return Fail("Файл слишком большой");

            var ext = Path.GetExtension(model.File.FileName).ToLowerInvariant();

            if (!_contentTypes.ContainsKey(ext))
                return Fail("Недопустимый формат");

            var user = await _context.Users.FindAsync(model.UserID);
            if (user == null)
                return Fail("Пользователь не найден");

            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                var oldPath = Path.Combine(_avatarsPath, user.AvatarPath);
                if (File.Exists(oldPath) && user.AvatarPath != "default-avatar.png")
                    File.Delete(oldPath);
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(_avatarsPath, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            user.AvatarPath = fileName;
            await _context.SaveChangesAsync();

            return new ReturnFileResult
            {
                Success = true,
                Message = "Аватар успешно загружен",
                FilePath = fileName
            };
        }

        public async Task<IActionResult> DownloadAvatar(int UserID)
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

            if (string.IsNullOrEmpty(user.AvatarPath))
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "У пользователя нет аватара"
                });
            }

            var fullPath = Path.Combine(_avatarsPath, user.AvatarPath);

            if (!File.Exists(fullPath))
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Файл не найден"
                });
            }

            var ext = Path.GetExtension(fullPath);
            var contentType = _contentTypes.GetValueOrDefault(ext, "application/octet-stream");

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = Path.GetFileName(fullPath)
            };
        }

        public async Task<ReturnFileResult> DeleteAvatar(int UserID)
        {
            var user = await _context.Users.FindAsync(UserID);
            if (user == null)
                return Fail("Пользователь не найден");

            if (string.IsNullOrEmpty(user.AvatarPath))
                return Fail("Файл отсутствует");

            if (user.AvatarPath == "default-avatar.png")
                return Fail("Нельзя удалить стандартный аватар");

            var fullPath = Path.Combine(_avatarsPath, user.AvatarPath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            user.AvatarPath = "default-avatar.png";
            await _context.SaveChangesAsync();

            return new ReturnFileResult
            {
                Success = true,
                Message = "Аватар удалён"
            };
        }

        public async Task<IActionResult> DownloadTrainerPhoto(int TrainerID)
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

            return DownloadFromFolder(_trainersPath, trainer.PhotoPath);
        }

        public async Task<IActionResult> DownloadEquipmentPhoto(int EquipmentID)
        {
            var equip = await _context.TrainingEquipments.FindAsync(EquipmentID);
            if (equip == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренажёр не найден"
                });
            }

            return DownloadFromFolder(_equipmentPath, equip.PhotoPath);
        }

        public async Task<IActionResult> DownloadExercisePhoto(int ExerciseID)
        {
            var exercise = await _context.Exercises.FindAsync(ExerciseID);
            if (exercise == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Упражнение не найдено"
                });
            }

            return DownloadFromFolder(_exercisesPath, exercise.PhotoPath);
        }

        public async Task<IActionResult> DownloadExerciseVideo(int ExerciseID)
        {
            var exercise = await _context.Exercises.FindAsync(ExerciseID);
            if (exercise == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Упражнение не найдено"
                });
            }

            // Видео/GIF-разбор лежит в той же папке упражнений, имя файла — в VideoPath.
            return DownloadFromFolder(_exercisesPath, exercise.VideoPath);
        }

        public async Task<IActionResult> DownloadEquipmentVideo(int EquipmentID)
        {
            var equip = await _context.TrainingEquipments.FindAsync(EquipmentID);
            if (equip == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренажёр не найден"
                });
            }

            // Видео/GIF-инструкция лежит в той же папке тренажёров, имя файла — в VideoPath.
            return DownloadFromFolder(_equipmentPath, equip.VideoPath);
        }

        public async Task<IActionResult> DownloadWorkoutPreview(int WorkoutID)
        {
            var workout = await _context.Workouts.FindAsync(WorkoutID);
            if (workout == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренировка не найдена"
                });
            }

            return DownloadFromFolder(_workoutsPath, workout.PreviewPath);
        }

        public async Task<IActionResult> DownloadBannerImage(int BannerID)
        {
            var banner = await _context.Banners.FindAsync(BannerID);
            if (banner == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Баннер не найден"
                });
            }

            return DownloadFromFolder(_bannersPath, banner.ImagePath);
        }

        public async Task<IActionResult> DownloadClubPhoto(int ClubID)
        {
            var club = await _context.Clubs.FindAsync(ClubID);
            if (club == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Клуб не найден"
                });
            }

            return DownloadFromFolder(_clubsPath, club.PhotoPath);
        }

        private IActionResult DownloadFromFolder(string folder, string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Файл отсутствует"
                });
            }

            var fullPath = Path.Combine(folder, fileName);
            if (!File.Exists(fullPath))
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Файл не найден на сервере"
                });
            }

            var ext = Path.GetExtension(fullPath);
            var contentType = _contentTypes.GetValueOrDefault(ext, "application/octet-stream");
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            return new FileStreamResult(stream, contentType)
            {
                FileDownloadName = Path.GetFileName(fullPath)
            };
        }

        private ReturnFileResult Fail(string message)
            => new() { Success = false, Message = message };
    }
}
