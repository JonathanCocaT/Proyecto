# Nuit Urbaine — Proyecto con Base de Datos Real

Este proyecto conecta el frontend HTML de Nuit Urbaine a **SQL Server** mediante
una **API REST en ASP.NET Core 8**. Ya no hay datos simulados en JavaScript.

---

## Estructura del proyecto

```
NuitUrbaine/
├── NuitUrbaine.sln                 ← Solución de Visual Studio
├── migration.sql                   ← Script SQL para preparar la BD
├── NuitUrbaine.API/                ← Proyecto ASP.NET Core Web API
│   ├── NuitUrbaine.API.csproj
│   ├── Program.cs
│   ├── appsettings.json            ← ⚙️ Aquí configuras la conexión
│   ├── Models/Models.cs            ← Clases de los datos
│   ├── Data/NuitRepository.cs      ← Todas las queries a SQL Server
│   └── Controllers/
│       ├── ProductosController.cs
│       ├── InventarioController.cs
│       ├── EmpleadosController.cs
│       └── OtherControllers.cs     ← Auth, Terceros, Dashboard
└── NuitUrbaine.Web/
    └── index.html                  ← Frontend modificado (ya sin datos simulados)
```

---

## Pasos para poner a funcionar

### Paso 1 — Importar la base de datos en SQL Server

1. Abre **SQL Server Management Studio (SSMS)**
2. Clic derecho en **Databases** → **Import Data-tier Application...**
3. Selecciona el archivo `Nuit.bacpac`
4. Nombre de la BD: **`Nuitt`** (tal como está en el .bacpac)
5. Finaliza el asistente

### Paso 2 — Ejecutar el script de migración

1. En SSMS abre el archivo `migration.sql` (está en la raíz del proyecto)
2. Ejecuta con **F5**
3. Verás el mensaje: `✅ Migración completada.`

### Paso 3 — Configurar la cadena de conexión

Abre `NuitUrbaine.API/appsettings.json` y ajusta la conexión si es necesario:

```json
"ConnectionStrings": {
  "NuitDB": "Data Source=localhost;Initial Catalog=Nuitt;Integrated Security=True;TrustServerCertificate=True;"
}
```

- Si SQL Server tiene un nombre de instancia (ej: `MSSQLSERVER`), usa:
  `Data Source=localhost\\MSSQLSERVER;...`
- Si usas usuario/contraseña en vez de Windows Auth:
  `...;User Id=sa;Password=TuPassword;`

### Paso 4 — Abrir y ejecutar la API en Visual Studio

1. Abre `NuitUrbaine.sln` en **Visual Studio 2022**
2. Si pide instalar paquetes NuGet, acepta (se instalan automáticamente)
3. Presiona **F5** para iniciar la API
4. En la consola verás: `✅ Conexión a SQL Server exitosa`
5. La API queda corriendo en `http://localhost:5000`

### Paso 5 — Abrir el frontend

1. Ve a la carpeta `NuitUrbaine.Web/`
2. Abre `index.html` en el navegador (doble clic, o abre con Chrome/Edge)
3. El catálogo público carga los productos **directamente de SQL Server**
4. Inicia sesión como `admin` / `admin123` para el panel de administración

---

## Endpoints de la API

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/login` | Login con usuario/contraseña |
| GET | `/api/productos` | Lista de productos activos |
| POST | `/api/productos` | Crear producto |
| PUT | `/api/productos/{id}` | Actualizar producto |
| DELETE | `/api/productos/{id}` | Desactivar producto (soft delete) |
| GET | `/api/inventario` | Inventario con nombre de producto |
| PUT | `/api/inventario/{id}/stock` | Actualizar stock |
| GET | `/api/empleados` | Lista empleados activos |
| PUT | `/api/empleados/{id}` | Actualizar empleado |
| GET | `/api/terceros/empresas` | Empresas activas |
| GET | `/api/terceros/proveedores` | Proveedores activos |
| GET | `/api/terceros/bancos` | Bancos activos |
| GET | `/api/dashboard/resumen` | KPIs para el dashboard ejecutivo |

---

## Requisitos

- Visual Studio 2022 (cualquier edición, incluyendo Community gratuita)
- .NET 8 SDK ([descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0))
- SQL Server (Express o superior) con SSMS
