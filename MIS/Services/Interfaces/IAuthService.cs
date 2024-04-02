using MIS.Data.Models;
using MIS.Data.DTO;

namespace MIS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDTO> Register(UserRegisterDTO userRegisterDTO);
        Task<TokenDTO> Login(UserLoginDTO ForSuccessfulLogin);
        Task<UserDTO> GetInfoProfile(Guid userId, string token);
        Task EditProfile(Guid UserId, userEditModel user, string token);
        Task Logout(string token);

    }
}
