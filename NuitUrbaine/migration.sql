-- ╔══════════════════════════════════════════════════════════════╗
-- ║  NUIT URBAINE — Script de Migración SQL Server               ║
-- ║  Ejecutar en SSMS sobre la base de datos "Nuitt"             ║
-- ║  después de importar el archivo Nuit.bacpac                  ║
-- ╚══════════════════════════════════════════════════════════════╝

USE Nuit_Urbaine;
GO

-- ─────────────────────────────────────────────
-- 1. Columnas nuevas en BASE_DATOS_PRODUCTOS
-- ─────────────────────────────────────────────
IF NOT EXISTS (
  SELECT 1 FROM sys.columns
  WHERE object_id = OBJECT_ID('dbo.BASE_DATOS_PRODUCTOS') AND name = 'ImagenURL'
)
  ALTER TABLE dbo.BASE_DATOS_PRODUCTOS ADD ImagenURL nvarchar(MAX) NULL;
ELSE
BEGIN
  -- Ampliar la columna si ya existe con nvarchar(500) para soportar base64 o URLs largas
  IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.BASE_DATOS_PRODUCTOS') AND name = 'ImagenURL'
      AND max_length < 0 -- max_length = -1 significa nvarchar(MAX), cualquier otro es limitado
  ) BEGIN PRINT 'ImagenURL ya es nvarchar(MAX).'; END
  ELSE
    ALTER TABLE dbo.BASE_DATOS_PRODUCTOS ALTER COLUMN ImagenURL nvarchar(MAX) NULL;
END

IF NOT EXISTS (
  SELECT 1 FROM sys.columns
  WHERE object_id = OBJECT_ID('dbo.BASE_DATOS_PRODUCTOS') AND name = 'Descripcion'
)
  ALTER TABLE dbo.BASE_DATOS_PRODUCTOS ADD Descripcion nvarchar(1000) NULL;

IF NOT EXISTS (
  SELECT 1 FROM sys.columns
  WHERE object_id = OBJECT_ID('dbo.BASE_DATOS_PRODUCTOS') AND name = 'Estado'
)
  ALTER TABLE dbo.BASE_DATOS_PRODUCTOS ADD Estado nvarchar(20) NULL DEFAULT 'nuevo';

-- ─────────────────────────────────────────────
-- 2. Columna de foto en BASE_DATOS_EMPLEADOS
-- ─────────────────────────────────────────────
IF NOT EXISTS (
  SELECT 1 FROM sys.columns
  WHERE object_id = OBJECT_ID('dbo.BASE_DATOS_EMPLEADOS') AND name = 'FotoURL'
)
  ALTER TABLE dbo.BASE_DATOS_EMPLEADOS ADD FotoURL nvarchar(MAX) NULL;
ELSE
BEGIN
  IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.BASE_DATOS_EMPLEADOS') AND name = 'FotoURL'
      AND max_length = -1
  )
    ALTER TABLE dbo.BASE_DATOS_EMPLEADOS ALTER COLUMN FotoURL nvarchar(MAX) NULL;
END

-- ─────────────────────────────────────────────
-- 3. Tabla de usuarios para login del sistema
-- ─────────────────────────────────────────────
IF NOT EXISTS (
  SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'USUARIOS_SISTEMA'
)
BEGIN
  CREATE TABLE dbo.USUARIOS_SISTEMA (
    UsuarioID     int           IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Username      nvarchar(50)  NOT NULL UNIQUE,
    PasswordHash  nvarchar(200) NOT NULL,
    Rol           nvarchar(20)  NOT NULL DEFAULT 'admin',
    Activo        bit           NOT NULL DEFAULT 1,
    FechaCreacion datetime2     NOT NULL DEFAULT GETDATE()
  );
  PRINT 'Tabla USUARIOS_SISTEMA creada.';
END

-- Usuario admin por defecto (contraseña: admin123)
-- ⚠️ Cambiar en producción
IF NOT EXISTS (SELECT 1 FROM dbo.USUARIOS_SISTEMA WHERE Username = 'admin')
  INSERT INTO dbo.USUARIOS_SISTEMA (Username, PasswordHash, Rol)
  VALUES ('admin', 'admin123', 'admin');

-- ─────────────────────────────────────────────
-- 4. Vista resumen ejecutivo (usada por el Dashboard)
-- ─────────────────────────────────────────────
IF OBJECT_ID('dbo.VW_RESUMEN_EJECUTIVO', 'V') IS NOT NULL
  DROP VIEW dbo.VW_RESUMEN_EJECUTIVO;
GO
CREATE VIEW dbo.VW_RESUMEN_EJECUTIVO AS
SELECT
  (SELECT COUNT(1) FROM dbo.EMPRESAS    WHERE Activo = 1) AS TotalEmpresas,
  (SELECT COUNT(1) FROM dbo.PROVEEDORES WHERE Activo = 1) AS TotalProveedores,
  (SELECT COUNT(1) FROM dbo.BANCOS      WHERE Activo = 1) AS TotalBancos,
  (SELECT COUNT(1) FROM dbo.BASE_DATOS_EMPLEADOS WHERE Activo = 1) AS TotalEmpleados,
  (SELECT COUNT(1) FROM dbo.BASE_DATOS_PRODUCTOS  WHERE Activo = 1) AS TotalProductos,
  (SELECT COUNT(1) FROM dbo.INVENTARIO_PRODUCTOS)                   AS TotalVariantesInventario,
  ISNULL((SELECT SUM(StockDisponible) FROM dbo.INVENTARIO_PRODUCTOS), 0) AS UnidadesEnInventario,
  ISNULL((SELECT SUM(CAST(StockDisponible AS bigint) * PrecioVenta) FROM dbo.INVENTARIO_PRODUCTOS), 0) AS ValorTotalInventario,
  ISNULL((SELECT SUM(SalarioBasico) FROM dbo.BASE_DATOS_EMPLEADOS WHERE Activo = 1), 0) AS NominaMensualTotal;
GO

PRINT '✅ Migración completada. La API ya puede conectarse.';
