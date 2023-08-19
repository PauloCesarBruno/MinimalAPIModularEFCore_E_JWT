using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.ServicesTokenJwt;
using System.Text;

namespace MinimalAPICatalogo.ServicesExtensionsApp
{
    public static class ServicesCollectionExtensions
    {   
        // Método de Extenssão de WebApplicationBuilder
        // Cria o Serviço do Swagger e inclui no Container nativo DI
        public static WebApplicationBuilder AddApiSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwagger();
            return builder;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            // Autorização única para todos os EndPoints
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiCatalogo", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = @"JWT Authorization header using the Bearer scheme.
                    Enter 'Bearer'[space].Example: \'Bearer 12345abcdef\'",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                });
            });

            return services;

        }

        // Método de Extenssão de WebApplicationBuilder
        // Define a persistência do contexto do EFCore 
        public static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<AppDbContext>(options =>
                            options
                            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            // Registro do Token
            builder.Services.AddSingleton<ITokenService>(new TokenService());
            return builder;
        }

        // Método de Extenssão de WebApplicationBuilder
        // Define serviço para geração da autenticação e da autorização JWT
        public static WebApplicationBuilder AddAutenticationJwt (this WebApplicationBuilder builder)
        {
            // Validação do Token
            builder.Services.AddAuthentication
                             (JwtBearerDefaults.AuthenticationScheme)
                             .AddJwtBearer(options =>
                             {
                                 options.TokenValidationParameters = new TokenValidationParameters
                                 {
                                     ValidateIssuer = true,
                                     ValidateAudience = true,
                                     ValidateLifetime = true,
                                     ValidateIssuerSigningKey = true,

                                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                     ValidAudience = builder.Configuration["Jwt:Audience"],
                                     IssuerSigningKey = new SymmetricSecurityKey
                                     (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                                 };
                             });

            builder.Services.AddAuthorization();
            return builder;
        }
    }
}
