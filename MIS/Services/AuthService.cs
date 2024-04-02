﻿using AutoMapper;
using MIS.Data;
using MIS.Data.Models;
using MIS.Services.Interfaces;
using MIS.Data.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MIS.Services
{

    public class AuthService : IAuthService
    {

        private ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public AuthService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //....................................<Регистрация пользователя>..............................................................
        public async Task<TokenDTO> Register(UserRegisterDTO userRegisterDTO)
        {
            if (!IsValidEmail(userRegisterDTO.Email))
            {
                throw new ArgumentException("Неверный формат email.");
            }

            if (!IsValidPhoneNumber(userRegisterDTO.PhoneNumber))
            {
                throw new ArgumentException("Неверный формат телефонного номера. Используйте формат +7xxxxxxxxxx.");
            }
            var r = userRegisterDTO.SpecialityId;
           var existingSpeciality = await _dbContext.Specialiti.FirstOrDefaultAsync(s => s.Id == userRegisterDTO.SpecialityId);
        
            if (existingSpeciality == null)
            {
                throw new ArgumentException($"Специальность с Id '{userRegisterDTO.SpecialityId}' не найдена.");
            }

            userRegisterDTO.Email = NormalizeAttribute(userRegisterDTO.Email);

            await CheckEmailIdentity(userRegisterDTO.Email);

            CheckGender(userRegisterDTO.Gender);

            CheckBirthDate(userRegisterDTO.BirthDate);

            byte[] saltBytes;
            RandomNumberGenerator.Fill(saltBytes = new byte[16]);

            using var deriveBytes = new Rfc2898DeriveBytes(userRegisterDTO.Password, saltBytes, 100000);

            byte[] passwordHashBytes = deriveBytes.GetBytes(20);

            byte[] combinedBytes = new byte[36];
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, 16);
            Buffer.BlockCopy(passwordHashBytes, 0, combinedBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(combinedBytes);

            await _dbContext.Users.AddAsync(new User
            {
                Id = Guid.NewGuid(),
                FullName = userRegisterDTO.FullName,
                BirthDate = userRegisterDTO.BirthDate,
                Email = userRegisterDTO.Email,
                Gender = userRegisterDTO.Gender,
                Password = savedPasswordHash,
                PhoneNumber = userRegisterDTO.PhoneNumber,
                SpecialityId = userRegisterDTO.SpecialityId,
                createTime = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync();


            var ForSuccessfulLogin = new UserLoginDTO
            {
                Email = userRegisterDTO.Email,
                Password = userRegisterDTO.Password
            };

            return await Login(ForSuccessfulLogin);
        }



        //...........................................<Вход в аккаунт>.......................................................
        public async Task<TokenDTO> Login(UserLoginDTO ForSuccessfulLogin)
        {
            ForSuccessfulLogin.Email = NormalizeAttribute(ForSuccessfulLogin.Email);
            var identity = await GetIdentity(ForSuccessfulLogin.Email, ForSuccessfulLogin.Password);
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: TokenConfigurations.Issuer,
                audience: TokenConfigurations.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.AddMinutes(TokenConfigurations.Lifetime),
                signingCredentials: new SigningCredentials(TokenConfigurations.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            var encodeJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

           

            return new TokenDTO()
            {
                Token = encodeJwt
            }; 
        }
        //...........................................<Выход из аккаунт>.......................................................
        public async Task Logout(string token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token), "Token cannot be null.");
            }

            var alreadyExistsToken = await _dbContext.Tokens.FirstOrDefaultAsync(x => x.InvalidToken == token);

            if (alreadyExistsToken == null)
            {
                var handler = new JwtSecurityTokenHandler();
                var expiredDate = handler.ReadJwtToken(token).ValidTo;
                _dbContext.Tokens.Add(new Token { InvalidToken = token, ExpiredDate = expiredDate });
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status401Unauthorized.ToString(), "Token is already invalid");
                throw exception;
            }
        }



        //..............................<Получение данных об аккаунте>............................................
        public async Task<UserDTO> GetInfoProfile(Guid userId, string token)
        {
            var chekValidToken = _dbContext.Tokens
                .Where(x => x.InvalidToken == token)
                .FirstOrDefault();
            if (chekValidToken == null)
            {

                var userEntity = await _dbContext
                  .Users
                  .FirstOrDefaultAsync(x => x.Id == userId);
                if (userEntity == null)
                {
                    throw new KeyNotFoundException("User not exists");
                }
                else
                {
                    if (userEntity != null)
                        return _mapper.Map<UserDTO>(userEntity);

                    var exception = new Exception();
                    exception.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                        "User not exists"
                    );
                    throw exception;
                }
            }
            else
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status401Unauthorized.ToString(), "Token is already invalid");
                throw exception;
            }
        }



        //..............................<Изменение данных профиля>............................................
        public async Task EditProfile(Guid userId, userEditModel user, string token)
        {
            var chekValidToken = _dbContext.Tokens
                .Where(x => x.InvalidToken == token)
                .FirstOrDefault();

            if (chekValidToken == null)
            {
                var userEntity = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

                if (userEntity == null)
                {
                    throw new KeyNotFoundException("User not exists");
                }
                else
                {

                    CheckGender(user.Gender);
                    CheckBirthDate(user.BirthDate);
                    await CheckEmailIdentity(user.Email);

                    userEntity.FullName = user.Name;
                    userEntity.BirthDate = user.BirthDate;
                    userEntity.Gender = user.Gender;
                    userEntity.PhoneNumber = user.PhoneNumber;
                    userEntity.Email = user.Email;

                    await _dbContext.SaveChangesAsync();
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Token is already invalid");
            }
        }

        //..............................<Удаление пробелов и верхнего регистра>............................................

        public static string NormalizeAttribute(string value)
        {
            return value.TrimEnd().ToLower();
        }

        //....................................<Проверка Пола>..............................................................
        private static void CheckGender(string gender)
        { 
            if (gender == Gender.Male.ToString() || gender == Gender.Female.ToString()) return;

            var exception = new Exception();
            exception.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"Possible Gender values: {Gender.Male.ToString()}, {Gender.Female.ToString()}");
            throw exception;
        }


        //......................................<Проверка даты рождения>............................................................

        private static void CheckBirthDate(DateTime? birthDate)
        {
            if (birthDate == null || birthDate <= DateTime.Now) return;

            var exception = new Exception();
            exception.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Birth date can't be later than today");
            throw exception;
        }
        //...........................................<Проверка Email адреса>.......................................................


        private async Task CheckEmailIdentity(string email)
        {
            var existingEmail = await _dbContext
                .Users
                .AnyAsync(x => x.Email == email);

            if (existingEmail)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    $"Аккаунт с данным email - '{email}' уже существует");
                throw exception;
            }
        }

        //..............................<т наличие пользователя с указанным email в базе данных и после проверка пароля на правильность>....................................................................


        private async Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            var userEntity = await _dbContext
                .Users
                .FirstOrDefaultAsync(x => x.Email == email);

            if (userEntity == null)
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                    "User not exists"
                );
                throw exception;
            }

            if (!ValidatePasswordHash(userEntity.Password, password))
            {
                var exception = new Exception();
                exception.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                    "Wrong password"
                );
                throw exception;
            }

            var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, userEntity.Id.ToString())
        };

            var claimsIdentity = new ClaimsIdentity
            (
                claims,
                "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
            );

            return claimsIdentity;
        }


        //..............................<функция сравнивает хэш введенного пользователем пароля с хэшем, сохраненным в базе данных, для проверки правильности предоставленного пароля>....................................................................

        private static bool ValidatePasswordHash(string savedPasswordHash, string userEnteredPassword)
        {
            var hashBytes = Convert.FromBase64String(savedPasswordHash);
            var storedSalt = new byte[16];
            Array.Copy(hashBytes, 0, storedSalt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(userEnteredPassword, storedSalt, 100000);
            var computedHash = pbkdf2.GetBytes(20);

            for (var i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != computedHash[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
            return Regex.IsMatch(email, emailPattern);
        }


        private bool IsValidPhoneNumber(string phoneNumber)
        {
            string phonePattern = @"^\+7\d{10}$";
            return Regex.IsMatch(phoneNumber, phonePattern);
        }

    }
}
