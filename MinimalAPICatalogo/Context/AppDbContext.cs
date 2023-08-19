using Microsoft.EntityFrameworkCore;
using MinimalAPICatalogo.Models;

namespace MinimalAPICatalogo.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    { }

    public DbSet <Produto> Produtos { get; set; }
    public DbSet <Categoria> Categorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Uso de Fluent API

        //Categoria:
        modelBuilder.Entity<Categoria>().HasKey(ch => ch.CategoriaId);

        modelBuilder.Entity<Categoria>().Property(n => n.Nome)
                                        .HasMaxLength(100)
                                        .IsRequired();

        modelBuilder.Entity<Categoria>().Property(d => d.Descricao)
                                       .HasMaxLength(250)
                                       .IsRequired();
        //

        //Produto:
        modelBuilder.Entity<Produto>().HasKey(ch => ch.ProdutoId);

        modelBuilder.Entity<Produto>().Property(n => n.Nome)
                                      .HasMaxLength(100)
                                      .IsRequired();

        modelBuilder.Entity<Produto>().Property(d => d.Descricao)
                                      .HasMaxLength(250)
                                      .IsRequired();

        modelBuilder.Entity<Produto>().Property(i => i.Imagem)
                                      .HasMaxLength(350)
                                      .IsRequired();

        modelBuilder.Entity<Produto>().Property(p => p.preco)
                                       .HasPrecision(18, 2)
                                       .IsRequired();
        //

        // Relacionamento:
        modelBuilder.Entity<Produto>()
                    .HasOne<Categoria>(c => c.Categoria)
                    .WithMany(p => p.Produtos)
                    .HasForeignKey(c => c.CategoriaId);
    }
}
