using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MIS.Services
{
    internal class TokenConfigurations
    {
        // Настройки для генерации JWT токенов
        public const string Issuer = "Blog"; // Название приложения или сервиса, генерирующего токен
        public const string Audience = "Bloggers"; // Аудитория, для которой предназначен токен
        private const string Key = "mYGh8lG8d6W7wC1cK2fR3sT4aP5eN9dE0dK8iS6eY3";
        public const int Lifetime = 60; // Продолжительность жизни токена в минутах

        // Метод для получения симметричного ключа (в данном примере - просто строка)
  
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }

    }
}