using FitStart_Server.Connection;
using FitStart_Server.Interfaces;
using FitStart_Server.Models;
using FitStart_Server.Requests;
using FitStart_Server.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FitStart_Server.Services
{
    public class LoginService : ILoginService
    {
        private static readonly Regex _onlyCyrillic = new(@"^[\p{IsCyrillic}]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static readonly Regex _emailRegex = new(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly ContextDb _context;
        private readonly JwtGenerator _jwtGenerator;

        public LoginService(ContextDb contextDb, JwtGenerator jwtGenerator)
        {
            _context = contextDb;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (string.IsNullOrWhiteSpace(model.LastName) || string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) || model?.BirthDate == null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Необходимо заполнить все поля"
                });
            }

            if (!_onlyCyrillic.IsMatch(model.LastName) || !_onlyCyrillic.IsMatch(model.Name))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "ФИО пользователя должно быть корректным и быть написанным на кириллице"
                });
            }

            if (!string.IsNullOrWhiteSpace(model.Patronymic) && !_onlyCyrillic.IsMatch(model.Patronymic))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "ФИО пользователя должно быть корректным и быть написанным на кириллице"
                });
            }

            if (model?.BirthDate.AddYears(14) > DateOnly.FromDateTime(DateTime.Today))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Для регистрации в клубе необходимо быть старше 14 лет"
                });
            }

            if (!_emailRegex.IsMatch(model.Email))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Некорректный формат email"
                });
            }

            if (model.Password.Length < 6)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Пароль должен содержать не менее 6 символов"
                });
            }

            var login = await _context.Logins.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (login != null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Пользователь с данным email адресом уже зарегистрирован"
                });
            }

            User user = new User()
            {
                AvatarPath = "default-avatar.png",
                LastName = model.LastName,
                Name = model.Name,
                Patronymic = string.IsNullOrWhiteSpace(model.Patronymic) ? null : model.Patronymic,
                BirthDate = model.BirthDate,
                Balance = 0,
                VibrationOnStart = true,
                Login = new Login()
                {
                    Email = model.Email,
                    Password = model.Password
                },
                RoleID = 2
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Успешная регистрация"
            });
        }

        public async Task<IActionResult> SignIn(SignInModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Необходимо заполнить все поля"
                });
            }

            var login = await _context.Logins.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (login == null)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Неверный email или пароль"
                });
            }

            if (model.Password != login.Password)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Неверный email или пароль"
                });
            }

            var user = await _context.Users.Include(x => x.Role).Include(x => x.Login).FirstOrDefaultAsync(x => x.LoginID == login.LoginID);
            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден"
                });
            }

            Session session = new Session()
            {
                Token = _jwtGenerator.GenerateToken(user.UserID, user.RoleID),
                UserID = user.UserID,
                LogedAt = DateTime.UtcNow
            };

            await _context.AddAsync(session);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Успешная авторизация",
                logedUser = user,
                token = session.Token
            });
        }

        public async Task<IActionResult> GetProfileInfo(int UserID)
        {
            var user = await _context.Users.Include(x => x.Role).Include(x => x.Login).FirstOrDefaultAsync(x => x.UserID == UserID);
            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Данные профиля не найдены"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                message = "Данные профиля получены",
                profile = user
            });
        }

        public async Task<IActionResult> Logout(int UserID, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Токен не передан"
                });
            }

            var session = await _context.Sessions.FirstOrDefaultAsync(x => x.UserID == UserID && x.Token == token);
            if (session == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Сессия не найдена"
                });
            }

            _context.Remove(session);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Успешный выход из аккаунта"
            });
        }
    }
}
