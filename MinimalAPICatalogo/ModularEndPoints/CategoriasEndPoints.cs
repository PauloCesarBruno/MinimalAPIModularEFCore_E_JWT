using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.Models;

namespace MinimalAPICatalogo.ModularEndPoints
{
    // Método de extenssão WebApplication
    public static class CategoriasEndPoints
    {
        public static void MapCategoriasPoints(this WebApplication app)
        {
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
        }
    }
}
