using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Models;
using FitStart_Server.Requests;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly ContextDb _context;
        public WorkoutService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetWorkouts(int UserID, int? typeID, string? category, bool? onlyTrial, bool? onlyFavorite)
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

            var query = _context.Workouts.Include(x => x.Workout_Type).AsQueryable();

            if (typeID.HasValue)
                query = query.Where(x => x.WT_ID == typeID.Value);

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(x => x.Workout_Type.Category == category);

            if (onlyTrial.HasValue && onlyTrial.Value)
                query = query.Where(x => x.isTrial);

            var workouts = await query.ToListAsync();

            var favoriteIds = await _context.UserFavoriteWorkouts
                .Where(x => x.UserID == UserID)
                .Select(x => x.WorkoutID)
                .ToListAsync();

            if (onlyFavorite.HasValue && onlyFavorite.Value)
                workouts = workouts.Where(x => favoriteIds.Contains(x.WorkoutID)).ToList();

            if (workouts == null || workouts.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренировки по заданным параметрам не найдены"
                });
            }

            var result = workouts.Select(w => new ReturnWorkoutModel
            {
                WorkoutID = w.WorkoutID,
                WorkoutName = w.WorkoutName,
                Description = w.Description,
                DurationMinutes = w.DurationMinutes,
                Intensity = w.Intensity,
                PreviewPath = w.PreviewPath,
                VideoPath = w.VideoPath,
                TrainerName = w.TrainerName,
                isTrial = w.isTrial,
                WorkoutType = w.Workout_Type,
                isFavorite = favoriteIds.Contains(w.WorkoutID)
            }).ToArray();

            return new OkObjectResult(new
            {
                status = true,
                message = "Список тренировок получен",
                workoutList = result
            });
        }

        public async Task<IActionResult> GetWorkoutById(int WorkoutID, int UserID)
        {
            var workout = await _context.Workouts
                .Include(x => x.Workout_Type)
                .FirstOrDefaultAsync(x => x.WorkoutID == WorkoutID);

            if (workout == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренировка не найдена"
                });
            }

            bool isFav = await _context.UserFavoriteWorkouts
                .AnyAsync(x => x.UserID == UserID && x.WorkoutID == WorkoutID);

            var info = new ReturnWorkoutModel
            {
                WorkoutID = workout.WorkoutID,
                WorkoutName = workout.WorkoutName,
                Description = workout.Description,
                DurationMinutes = workout.DurationMinutes,
                Intensity = workout.Intensity,
                PreviewPath = workout.PreviewPath,
                VideoPath = workout.VideoPath,
                TrainerName = workout.TrainerName,
                isTrial = workout.isTrial,
                WorkoutType = workout.Workout_Type,
                isFavorite = isFav
            };

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные о тренировке получены",
                workout = info
            });
        }

        public async Task<IActionResult> AddToFavorites(FavoriteWorkoutModel model)
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

            var workout = await _context.Workouts.FindAsync(model.WorkoutID);
            if (workout == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренировка не найдена"
                });
            }

            var existing = await _context.UserFavoriteWorkouts
                .FirstOrDefaultAsync(x => x.UserID == model.UserID && x.WorkoutID == model.WorkoutID);
            if (existing != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Тренировка уже в избранном"
                });
            }

            User_Favorite_Workout fav = new User_Favorite_Workout()
            {
                UserID = model.UserID,
                WorkoutID = model.WorkoutID,
                AddedAt = DateTime.UtcNow
            };

            await _context.AddAsync(fav);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Тренировка добавлена в избранное"
            });
        }

        public async Task<IActionResult> RemoveFromFavorites(FavoriteWorkoutModel model)
        {
            var fav = await _context.UserFavoriteWorkouts
                .FirstOrDefaultAsync(x => x.UserID == model.UserID && x.WorkoutID == model.WorkoutID);
            if (fav == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Тренировка не была в избранном"
                });
            }

            _context.Remove(fav);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Тренировка убрана из избранного"
            });
        }

        public async Task<IActionResult> GetUserFavorites(int UserID)
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

            var favorites = await _context.UserFavoriteWorkouts
                .Include(x => x.Workout).ThenInclude(w => w.Workout_Type)
                .Where(x => x.UserID == UserID)
                .OrderByDescending(x => x.AddedAt)
                .ToListAsync();

            if (favorites == null || favorites.Count == 0)
            {
                return new OkObjectResult(new
                {
                    status = true,
                    message = "В избранном пока ничего нет",
                    workoutList = new object[0]
                });
            }

            var result = favorites.Select(f => new ReturnWorkoutModel
            {
                WorkoutID = f.Workout.WorkoutID,
                WorkoutName = f.Workout.WorkoutName,
                Description = f.Workout.Description,
                DurationMinutes = f.Workout.DurationMinutes,
                Intensity = f.Workout.Intensity,
                PreviewPath = f.Workout.PreviewPath,
                VideoPath = f.Workout.VideoPath,
                TrainerName = f.Workout.TrainerName,
                isTrial = f.Workout.isTrial,
                WorkoutType = f.Workout.Workout_Type,
                isFavorite = true
            }).ToArray();

            return new OkObjectResult(new
            {
                status = true,
                message = "Избранные тренировки получены",
                workoutList = result
            });
        }
    }
}
