using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.Models;
using MinimalAPICatalogo.ServicesTokenJwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

// Autorização única para todos os EndPoints
builder.Services.AddSwaggerGen(c =>
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


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
                options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registro do Token
builder.Services.AddSingleton<ITokenService>(new TokenService());

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

var app = builder.Build();

app.MapGet("/", () => "Catalogo de Produtos - 2023").ExcludeFromDescription();

//--------------------------End Points para Categoria--------------------------------------

app.MapPost("/Categoria", [Authorize] async
    (Categoria categoria, AppDbContext context) =>
{
    context.Categorias.Add(categoria);
    await context.SaveChangesAsync();
    return Results.Created($"/categorias/{categoria.CategoriaId}", categoria);
});

//await context.Categorias.ToListAsync()).RequireAuthorization(); -> OUTRA FORMA DE IMPEDIR SEM USAR [Authorize].
app.MapGet("/Categorias", [Authorize] async (AppDbContext context) =>
await context.Categorias.ToListAsync()); 

app.MapGet("/Categorias/{id:int}", [Authorize] async (int id, AppDbContext context) =>
{
    return await context.Categorias.FindAsync(id)
    is Categoria categoria
            ? Results.Ok(categoria)
            : Results.NotFound();

});


app.MapPut("/Categorias/{id:int}", [Authorize] async (int id, Categoria categoria, AppDbContext context) =>
{
    if (categoria.CategoriaId != id)
    {
        return Results.BadRequest("Categoria não encontrada !");
    }

    var categoriaDB = await context.Categorias.FindAsync(id);

    if (categoriaDB is null) return Results.NotFound();

    categoriaDB.Nome = categoria.Nome;
    categoriaDB.Descricao = categoria.Descricao;

    await context.SaveChangesAsync();

    return Results.Ok(categoriaDB);

});

app.MapDelete("/Categorias/{id:int}", [Authorize] async (int id, AppDbContext context) =>
{
    var categoria = await context.Categorias.FindAsync(id);

    if (categoria is null)
    {
        return Results.NotFound();
    }

    context.Categorias.Remove(categoria);

    await context.SaveChangesAsync();

    return Results.NoContent();

});

//--------------------------End Points para Produto--------------------------------------

app.MapPost("/Produto", [Authorize] async
    (Produto produto, AppDbContext context) =>
{
    context.Produtos.Add(produto);
    await context.SaveChangesAsync();
    return Results.Created($"/Produtos/{produto.ProdutoId}", produto);
});

app.MapGet("Produtos", [Authorize] async (AppDbContext context) =>
await context.Produtos.ToListAsync());

app.MapGet("/Produtos/{id:int}", [Authorize] async (int id, AppDbContext context) =>
{
    return await context.Produtos.FindAsync(id)
    is Produto produto
            ? Results.Ok(produto)
            : Results.NotFound();

});

app.MapPut("/Produtos/{id:int}",  async (int id, Produto produto, AppDbContext context) =>
{
    if (produto.ProdutoId != id)
    {
        return Results.BadRequest("Produto não encontrado !");
    }

    var produtoDB = await context.Produtos.FindAsync(id);

    if (produtoDB is null) return Results.NotFound();

    produtoDB.Nome = produto.Nome;
    produtoDB.Descricao = produto.Descricao;
    produtoDB.preco = produto.preco;
    produtoDB.Imagem = produto.Imagem;
    produtoDB.DataCompra = produto.DataCompra;
    produtoDB.Estoque = produto.Estoque;
    produtoDB.CategoriaId = produto.CategoriaId;


    await context.SaveChangesAsync();

    return Results.Ok(produtoDB);

});

app.MapDelete("/Produtos/{id:int}", [Authorize] async (int id, AppDbContext context) =>
{
    var produto = await context.Produtos.FindAsync(id);

    if (produto is null)
    {
        return Results.NotFound();
    }

    context.Produtos.Remove(produto);

    await context.SaveChangesAsync();

    return Results.NoContent();

});

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

// =========================================================================================================


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();
