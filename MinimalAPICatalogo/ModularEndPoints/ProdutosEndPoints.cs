using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.Models;

namespace MinimalAPICatalogo.ModularEndPoints
{
    // Método de extenssão WebApplication
    public static class ProdutosEndPoints
    {
        public static void MapProdutosEndPoints(this WebApplication app)
        {
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

            app.MapPut("/Produtos/{id:int}", [Authorize] async (int id, Produto produto, AppDbContext context) =>
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
        }
    }
}
