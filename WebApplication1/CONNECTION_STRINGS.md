# Connection strings — cómo evitar pisarnos entre integrantes

El proyecto tiene DOS formas de conectarse a SQL Server, según cómo lo tenga
cada uno instalado:

- **Martín**: SQL Server Express local (`localhost\SQLEXPRESS`, sin usuario/contraseña,
  usa la sesión de Windows — "Trusted_Connection").
- **[Nombre de tu compañera]**: Docker con SQL Server (`localhost,1433`, usuario `sa`
  y contraseña).

## Cómo está resuelto

`appsettings.json` (este archivo SE SUBE a git) tiene el connection string de
SQL Server Express, porque es genérico — cualquiera que tenga SQL Server
Express instalado con el nombre de instancia por defecto lo puede usar tal cual.

Si vos usás Docker en cambio, creá un archivo `appsettings.Development.json`
en la misma carpeta (`WebApplication1/`) con este contenido:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MusicTradeDB;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True;"
  }
}
```

Ese archivo **NO se sube a git** (está en `.gitignore`), así que tu connection
string con tu contraseña queda solo en tu máquina, y `appsettings.json`
(el compartido) sigue intacto para los demás.

ASP.NET Core lee primero `appsettings.json` y después, si existe,
`appsettings.Development.json` pisa solo lo que esté repetido — por eso
alcanza con poner únicamente `ConnectionStrings` ahí, no hace falta copiar
todo el archivo.
