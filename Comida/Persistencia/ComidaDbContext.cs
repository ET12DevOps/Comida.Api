using Comida.Modelo;
using Microsoft.EntityFrameworkCore;

namespace Comida.Persistencia;

public class ComidaDbContext : DbContext
{
    public ComidaDbContext(DbContextOptions<ComidaDbContext> opciones) : base(opciones)
    {

    }

    public DbSet<Ingrediente> Ingredientes { get; set; }

    public DbSet<Plato> Platos { get; set; }
}