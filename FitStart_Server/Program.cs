using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Services;
using FitStart_Server.UniversalMethods;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContextDb>(option => option.UseNpgsql(builder.Configuration.GetConnectionString("FitStartConnection")), ServiceLifetime.Scoped);

builder.Services.AddSingleton<JwtGenerator>();

builder.Services.AddScoped<IBaseInfoService, BaseInfoService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFitStart", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFitStart");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
