using MinimalAPICatalogo.Models;

namespace MinimalAPICatalogo.ServicesTokenJwt
{
    public interface ITokenService
    {
        string GerarToken(string key, string issuer, string audience, UserModel user);
    }
}
