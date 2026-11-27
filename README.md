# InventoryRD

InventoryRD es un sistema administrativo para gestionar productos, categorías, proveedores y existencias. La interfaz está en español y consume la API del repositorio.

## Funcionalidades

- CRUD de productos, categorías y proveedores. Los productos se pueden buscar y filtrar por categoría, proveedor y estado.
- Entradas y salidas con historial, validación de cantidades y control de concurrencia. El stock inicial se registra como una entrada. Los cambios posteriores se registran desde Movimientos de inventario.
- Panel con totales, valor en pesos dominicanos y movimientos recientes. Se consideran de pocas existencias los productos activos con cinco unidades o menos.
- Reportes de productos, existencias, categorías, proveedores y movimientos con exportación CSV.
- Protección de las relaciones. Las categorías y los proveedores asociados a productos deben conservarse. Los productos con historial se pueden desactivar para mantener la trazabilidad.

## Tecnologías y arquitectura

La solución usa C#, .NET 10, ASP.NET Core Web API, Entity Framework Core 10.0.9, SQL Server y Swagger. La interfaz usa React 19, Vite 8, JavaScript y CSS.

| Ruta | Responsabilidad |
| --- | --- |
| `InventoryRD.slnx` | Solución .NET desde la raíz |
| `InventoryRD.Domain/` | Entidades y clase base |
| `InventoryRD.Infrastructure/` | DataContext, repositorios y migraciones |
| `InventoryRD.Application/` | Contratos, DTOs, validaciones y servicios |
| `InventoryRD.API/` | Controladores, Swagger y registro de dependencias |
| `InventoryRD.Frontend/` | Interfaz y proxy de Vite |

Los servicios usan repositorios síncronos mediante inyección de dependencias. Los cambios de existencias y su historial se guardan juntos. SQL Server usa `rowversion` para detectar cambios simultáneos y llaves foráneas y restricciones para proteger los datos. La estructura se basa en ParkRD.

## Requisitos

- SDK de .NET 10 o posterior con el runtime de ASP.NET Core 10.
- SQL Server Express o una instancia de SQL Server accesible.
- Node.js 22.12 o posterior y npm.

## Configuración y migraciones

La configuración de desarrollo en `InventoryRD.API/appsettings.Development.json` usa la instancia local `.\SQLEXPRESS`, la base `InventoryRD` y la autenticación de Windows. Para usar otra instancia o base, configura `ConnectionStrings__DefaultConnection` en la terminal. Para pruebas, usa una base dedicada.

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ConnectionStrings__DefaultConnection = 'Server=.\SQLEXPRESS;Database=InventoryRD;Trusted_Connection=True;TrustServerCertificate=True'

dotnet tool restore
dotnet restore InventoryRD.slnx
dotnet build InventoryRD.slnx --no-restore
dotnet ef database update --project InventoryRD.Infrastructure --startup-project InventoryRD.API
```

Ambas migraciones están en `InventoryRD.Infrastructure/Migrations`. La primera crea las tablas del catálogo. La segunda incorpora el historial y registra como entradas las existencias que ya tenían los productos.

## Ejecución de la API

Desde la raíz, prepara el certificado de desarrollo y ejecuta el perfil HTTPS.

```powershell
dotnet dev-certs https --trust
dotnet run --project InventoryRD.API --launch-profile https
```

Swagger está en [https://localhost:7080/swagger](https://localhost:7080/swagger). El perfil también escucha en el puerto HTTP `5080`. El archivo `InventoryRD.API/InventoryRD.API.http` incluye ejemplos de consulta.

## Ejecución del frontend

Abre otra terminal en la raíz del repositorio y ejecuta estos comandos.

```powershell
Set-Location InventoryRD.Frontend
npm ci
npm run dev
```

La interfaz está en [https://localhost:3000](https://localhost:3000) y usa un certificado local de Vite. El proxy envía las solicitudes `/api` a `https://localhost:7080`. Si la API usa otro puerto, establece `INVENTORY_API_URL` antes de ejecutar Vite. Para crear el compilado, ejecuta `npm run build`.

## Prueba del CRUD

1. En Categorías, agrega una categoría. En Proveedores, registra un proveedor con correo y teléfono válidos.
2. En Productos, crea un producto con ambos registros, asigna un precio y consulta su fila. Edita el precio o el estado y comprueba que los cambios se guardaron.
3. En Movimientos de inventario, registra una entrada de 8 y una salida de 3 sobre un producto con stock inicial de 0. Comprueba el saldo de 5 y verifica que se rechace una salida superior.
4. Revisa Inicio y exporta un CSV desde Reportes. El historial y el saldo deben conservarse al reiniciar la API.
5. Para probar la eliminación, crea un producto de prueba con stock inicial de 0 y sin movimientos. Elimínalo y verifica en Swagger que su consulta por ID devuelve `404`. Elimina las categorías y los proveedores de prueba cuando queden libres.

Las rutas del catálogo son `/api/products`, `/api/categories` y `/api/suppliers`. Las rutas adicionales son `/api/products/low-stock`, `/api/stockmovements` y `/api/reports/{products,stock,categories,suppliers,movements}`.

## Flujo de Git

Cada etapa se desarrolla en una rama `feature/` desde `developer`. Se valida localmente y se abre un Pull Request directo desde la feature a cada destino, en el orden `developer`, `qa`, `release` y `main`. Las integraciones usan commits de merge y las ramas de trabajo se conservan en GitHub.
