dotnet add package Microsoft.EntityFrameworkCore --version 6.0.11

dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.11

dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.11

dotnet add package Pomelo.EntityFrameworkCore.MySql 

dotnet ef migrations add MigracionInicial --context ComidaDbContext --output-dir Persistencia/Migraciones --project Comida --startup-project Comida

dotnet add package Swashbuckle.AspNetCore --version 6.4.0