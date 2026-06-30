using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Models;
using FitStart_Server.ReturnModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitStart_Server.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ContextDb _context;
        public ExerciseService(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public async Task<IActionResult> GetExercises(int? typeID, int? muscleGroupID)
        {
            var query = _context.Exercises
                .Include(x => x.Exercise_Type)
                .AsQueryable();

            if (typeID.HasValue)
                query = query.Where(x => x.ET_ID == typeID.Value);

            var exercises = await query.ToListAsync();
            if (exercises == null || exercises.Count == 0)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Упражнения не найдены"
                });
            }

            var exerciseIds = exercises.Select(e => e.ExerciseID).ToList();
            var descriptions = await _context.ExerciseDescriptions
                .Include(x => x.Muscle_Group)
                .Where(x => exerciseIds.Contains(x.ExerciseID))
                .ToListAsync();

            if (muscleGroupID.HasValue)
            {
                var matchedExerciseIds = descriptions
                    .Where(x => x.MG_ID == muscleGroupID.Value)
                    .Select(x => x.ExerciseID)
                    .Distinct()
                    .ToList();
                exercises = exercises.Where(x => matchedExerciseIds.Contains(x.ExerciseID)).ToList();

                if (exercises.Count == 0)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Упражнения по выбранным параметрам не найдены"
                    });
                }
            }

            var result = exercises.Select(e =>
            {
                var groups = descriptions
                    .Where(d => d.ExerciseID == e.ExerciseID)
                    .Select(d => d.Muscle_Group)
                    .Distinct()
                    .ToArray();
                return new ReturnExerciseModel
                {
                    ExerciseID = e.ExerciseID,
                    ExerciseName = e.ExerciseName,
                    Difficulty = e.Difficulty,
                    Technique = e.Technique,
                    Recommendations = e.Recommendations,
                    PhotoPath = e.PhotoPath,
                    VideoPath = e.VideoPath,
                    ExerciseType = e.Exercise_Type,
                    MuscleGroups = groups
                };
            }).ToArray();

            return new OkObjectResult(new
            {
                status = true,
                message = "Список упражнений получен",
                exerciseList = result
            });
        }

        public async Task<IActionResult> GetExerciseById(int ExerciseID)
        {
            var exercise = await _context.Exercises
                .Include(x => x.Exercise_Type)
                .FirstOrDefaultAsync(x => x.ExerciseID == ExerciseID);

            if (exercise == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Упражнение не найдено"
                });
            }

            var groups = await _context.ExerciseDescriptions
                .Include(x => x.Muscle_Group)
                .Where(x => x.ExerciseID == ExerciseID)
                .Select(x => x.Muscle_Group)
                .Distinct()
                .ToArrayAsync();

            var info = new ReturnExerciseModel
            {
                ExerciseID = exercise.ExerciseID,
                ExerciseName = exercise.ExerciseName,
                Difficulty = exercise.Difficulty,
                Technique = exercise.Technique,
                Recommendations = exercise.Recommendations,
                PhotoPath = exercise.PhotoPath,
                VideoPath = exercise.VideoPath,
                ExerciseType = exercise.Exercise_Type,
                MuscleGroups = groups
            };

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные об упражнении получены",
                exercise = info
            });
        }
    }
}
