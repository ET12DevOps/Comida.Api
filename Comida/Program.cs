using Comida.Modelo;
using Comida.Persistencia;
using Comida.ViewModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("comida_db");

var mysqlVersion = new MySqlServerVersion("8.0.30");

builder.Services.AddDbContext<ComidaDbContext>(opciones => opciones.UseMySql(connectionString, mysqlVersion));

builder.Services.AddScoped<ComidaDbContext>();

//Permite crear la base de datos desde C#
var opcionesContexto = new DbContextOptionsBuilder<ComidaDbContext>();

opcionesContexto.UseMySql(connectionString, mysqlVersion);

var dbComida = new ComidaDbContext(opcionesContexto.Options);

dbComida.Database.EnsureCreated();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/ingrediente", async (ComidaDbContext db) =>
{
    var ingredientes = await db.Ingredientes.ToListAsync();
    return Results.Ok(ingredientes);
});

app.MapGet("/ingrediente/{id}", async (ComidaDbContext db, Guid id) =>
{
    return Results.Ok(await db.Ingredientes.FindAsync(id));
});

app.MapPost("/ingrediente", async (ComidaDbContext db, IngredienteViewModel ingrediente) =>
{

    var nuevoIngrediente = new Ingrediente
    {
        Nombre = ingrediente.Nombre,
        Calorias = ingrediente.Calorias
    };

    await db.Ingredientes.AddAsync(nuevoIngrediente);
    await db.SaveChangesAsync();
    return Results.Created($"/ingrediente/{nuevoIngrediente.Id}", nuevoIngrediente);
});


app.MapGet("/plato", async (ComidaDbContext db) =>
{
    return Results.Ok(await db.Platos.Include(x => x.Ingredientes).ToListAsync());
});


app.MapPost("/plato/", async (ComidaDbContext db, PlatoViewModel plato) =>
{
    var nuevoPlato = new Plato
    {
        Nombre = plato.Nombre,
    };

    await db.Platos.AddAsync(nuevoPlato);
    await db.SaveChangesAsync();
    return Results.Created($"/plato/{nuevoPlato.Id}", nuevoPlato);
});

app.MapPost("/plato/{id}/ingredientes", async (ComidaDbContext db, int id, List<IngredienteViewModel> ingredientes) =>
{
    var plato = await db.Platos.FindAsync(id);
    var ingredientesAdicionales = new List<Ingrediente>();

    foreach (var ingrediente in ingredientes)
    {
        ingredientesAdicionales.Add(new Ingrediente
        {
            Nombre = ingrediente.Nombre,
            Calorias = ingrediente.Calorias
        });
    }

    plato.Ingredientes.AddRange(ingredientesAdicionales);

    db.Entry(plato).State = EntityState.Modified;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPost("/plato/{id}/ingrediente", async (ComidaDbContext db, int id, IngredienteViewModel ingrediente) =>
{
    var plato = await db.Platos.FindAsync(id);
    var nuevoIngrediente = new Ingrediente
    {
        Nombre = ingrediente.Nombre,
        Calorias = ingrediente.Calorias
    };
    plato.Ingredientes.Add(nuevoIngrediente);

    db.Entry(plato).State = EntityState.Modified;

    await db.SaveChangesAsync();
    return Results.Created($"/ingrediente/{nuevoIngrediente.Id}", nuevoIngrediente);
});

app.MapPost("/plato/{id}/ingrediente/{idIngrediente}", async (ComidaDbContext db, int id, int idIngrediente) =>
{
    var plato = await db.Platos.FindAsync(id);

    var ingrediente = await db.Ingredientes.FindAsync(idIngrediente);

    plato.Ingredientes.Add(ingrediente);

    db.Entry(plato).State = EntityState.Modified;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
