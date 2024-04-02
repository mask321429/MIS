using MIS.Data.Models;

namespace MIS.Services.Interfaces
{
    public interface ITokenService
    {
        User? GetUserWithToken(string jwt);
        bool IsValidToken(string jwt);
        void ClearInvalidTokens();
    }
}
