# Minimal Web Api

Se debe crear un proyecto web con el formato **Minimal Web Api**.

Para ello se creara el proyecto con el comando, donde **Comida** es el nombre de la aplicación:

```
dotnet new web -o Comida -f net6.0
```

# Dependencias (Paquetes Nuget)

Instalar los siguientes paquetes nuget:
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Tools
  - Pomelo.EntityFrameworkCore.MySql
  - Swashbuckle.AspNetCore

Con los siguientes comandos:

```
dotnet add package Microsoft.EntityFrameworkCore --version 6.0.11
```

```
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.11
```

```
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.11
```

```
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 6.0.2
```

```
dotnet add package Swashbuckle.AspNetCore --version 6.4.0
```

# Swagger

Swagger es un paquete nuget que nos permite probar los **endpoints** de una Web Api desde una interfaz web de manera simple

Para poder configurar **swagger** debemo completar el siguiente código en el archivo **Program.cs**:

```c#
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

y 


```c#
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

Quedando:

```c#
...
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
...
var app = builder.Build();
...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
...
```
# Persistencia

## Connection String

Configurar la cadena de conexion a la base de datos (ConnectionString) en el archivo **appsettings.json**:

Con la siguiente configuracion:

```json
"ConnectionStrings": {
    "proyecto_db" : "server=localhost;database=comida_db;User=root;password=root"
  }
```

El archivo **appsettings.json** deberia quedar:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "comida_db" : "Server=localhost;Database=comida_db;User=root;Password=root"
  }
}
```


## Contexto (DbContext)

Crear un directorio con el nombre **Persistencia** dentro del proyecto **Comida**

Dentro del directorio **Persistencia** crear una clase con el nombre **ComidaDbContext**

Escribir sl siguiente contenido del archivo **ComidaDbContext.cs**:

```c#
public class ComidaDbContext : DbContext
{
    public ComidaDbContext(DbContextOptions<ComidaDbContext> opciones) : base(opciones)
    {

    }

    public DbSet<Ingrediente> Ingredientes { get; set; }

    public DbSet<Plato> Platos { get; set; }
}
```

## Anotaciones (Data Annotations Attributes)

Para cada una de la entidades (clases) del dominio, agregar las anotaciones necesarias para explicitar el comportamiento del atributo en la base de datos.

Aqui es donde se explita que atributo sera **Primary Key**, **Foreign Key**, su tipo de dato especifico, nombre de la tabla, nombre de la columna (atributo), si es NULL, etc.  

Puede que aparezcan muchos **warnings** que indiquen que un atributo no puede ser NULL para ello vamos a desactivar esas advertencias mediante la configuración del archivo **Comida.csproj** para lo cual vamos a agregar la siguiente linea:

```xml
<NoWarn>8618</NoWarn>
```

Quedando:

```xml
<PropertyGroup>
  <TargetFramework>net6.0</TargetFramework>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
  <NoWarn>8618</NoWarn>
</PropertyGroup>
```

## Creación de la base de datos e inyección de dependencia

En el archivo **Program.cs** se debe indicar que nuestro contexto se debe asociar con la base de datos MySql para ello se debe colocar el siguiente codigo:

```c#
//obtengo el connectionString desde el archivo appsettings.json
//se debe indicar el nombre del item "proyecto_db"
var connectionString = builder.Configuration.GetConnectionString("comida_db");

var mysqlVersion = new MySqlServerVersion("8.0.30");

//agrego la configuracion al nuestro contexto AplicacionDbContexto
builder.Services.AddDbContext<ComidaDbContext>(opcion =>
    opcion.UseMySql(connectionString, mysqlVersion));

//agrego nuestro context AplicacionDbContext al contenedor de objetos
//con esto el objeto va a ser poder accedido desde cualquier otro objeto
//particularmente los controladores
builder.Services.AddDbContext<ComidaDbContext>();
```

Ademas tambien debemos crear la base de datos en MySql de forma explicita
```c#
//Por defecto, la accion de crear el objeto contexto no significa que se creara la base de datos 
//en MySql, por lo que lo debemos hacer manualmente

//creo un objeto de opciones de nuestro contexto
var opciones = new DbContextOptionsBuilder<AplicacionDbContext>();

//a las opciones creadas le asigno las credenciales para conectar la base de datos
opciones.UseMySql(connectionString, mysqlVersion);

//creo un objeto contexto con las opciones previamente definidas
var contexto = new AplicacionDbContext(opciones.Options);

//indico explicitamente que se debe crear nuestro contexto en el motor de base de datos
contexto.Database.EnsureCreated();
...
var app = builder.Build();
...
```

# Migraciones (Migrations)

Para crear la base de datos en MySql se debe traducir desde nuestro contexto (C#) a SQL, para ello debemos realizar una migracion. Esto se debe realizar una unica vez al momento de crear la base de datos. 

```
dotnet ef migrations add NOMBRE_MIGRACION --context NOMBRE_CONTEXTO --output-dir DIRECTORIO_MIGRACIONES --project NOMBRE_PROYECTO --startup-project NOMBRE_PROYECTO_EJECUTABLE
```

Ejemplo:

```
dotnet ef migrations add MigracionInicial --context ComidaDbContext --output-dir Persistencia/Migraciones --project Comida --startup-project Comida
```

Cada vez que ingresemos un nuevo cambio en el contexto. Por ejemplo agregar un nuevo atributo a una entidad. Se debe realizar una nueva migracion con un nombre distinto a los existentes.

```
dotnet ef migrations add UnNuevoCambio --context ComidaDbContext --output-dir Persistencia/Migraciones --project Comida --startup-project Comida
```

**Aclaración:** El comando para ejecutar se debe realizar por fuera de la carpeta del proyecto de web api.

# Controladores (Controllers)

Dado que estamos construyendo una **minimal web api**, no es necesario crear controladores con muchos endpoints en archivo externos.

La idea principal en una **minimal web api** es crear una cantidad reducida de **endpoints** que realicen lo necesario y nada mas en el archivo **Program.cs**.

# Ejecución del proyecto

Para poder ejecutar la aplicacion de **Comida** se debe realizar desde la terminal, posicionados por fuera de la carpeta del proyecto de **web api**:

```
dotnet run --project Comida
```

O simplemente con el siguiente comando dentro de la carpeta del proyecto de **web api**:

```
dotnet run
```

