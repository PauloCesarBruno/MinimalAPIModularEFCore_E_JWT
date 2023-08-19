using Microsoft.AspNetCore.Authorization;
using MinimalAPICatalogo.Models;
using MinimalAPICatalogo.ServicesTokenJwt;

namespace MinimalAPICatalogo.ModularEndPoints
{
    // Método de extenssão WebApplication
    public static class AutenticacaoEndPoint
    {
        public static void MapAuenticacaoEndPoints(this WebApplication app)
        {
            // EndPoint para efetuar o Login e obter o Token.
            app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
            {
                if (userModel == null)
                {
                    return Results.BadRequest("Login Invalido");
                }
                if (userModel.UserName == "PauloBruno" && userModel.Password == "Paradoxo22")
                {
                    var tokenString = tokenService.GerarToken(app.Configuration["Jwt:Key"],
                        app.Configuration["Jwt:Issuer"],
                        app.Configuration["Jwt:Audience"],
                        userModel);
                    return Results.Ok(new { token = tokenString });
                }
                else
                {
                    return Results.BadRequest("Login Invalido");
                }
            }).Produces(StatusCodes.Status400BadRequest)
                          .Produces(StatusCodes.Status200OK)
                          .WithName("Login")
                          .WithTags("MinimalAPICatalogo -> Autenticacao");

        }
    }
}
