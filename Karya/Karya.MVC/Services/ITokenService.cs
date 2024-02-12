using IdentityModel.Client;

namespace Karya.MVC.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
}
