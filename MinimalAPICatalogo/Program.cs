using MinimalAPICatalogo.ModularEndPoints;
using MinimalAPICatalogo.ServicesExtensionsApp;

var builder = WebApplication.CreateBuilder(args);

// Da pasta ServicesExtensionsApp e suas classes.
builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAutenticationJwt();

var app = builder.Build();

// Da pasta ModularEndPoints e sua classe ServicesCollectionExtensions.
app.MapAuenticacaoEndPoints();
app.MapCategoriasPoints();
app.MapProdutosEndPoints();


// Da pasta ModularEndPoints e sua classeAppBuildeExtensions. 
var environment = app.Environment;
app.UseExceptionHandling(environment)
   .UseSwaggerMidlleware()
   .UseAppCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
