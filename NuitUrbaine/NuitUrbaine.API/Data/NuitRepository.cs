using Dapper;
using Microsoft.Data.SqlClient;
using NuitUrbaine.API.Models;

namespace NuitUrbaine.API.Data;

/// <summary>
/// Repositorio principal. Todas las operaciones hablan directamente con SQL Server.
/// Cada método corresponde a una query o stored procedure de la base de datos Nuitt.
/// </summary>
public class NuitRepository
{
    private readonly string _conn;
    public NuitRepository(string connectionString) => _conn = connectionString;

    // ──────────────────────────────────────────────────
    // AUTENTICACIÓN
    // ──────────────────────────────────────────────────
    public async Task<bool> ValidarLogin(string username, string password)
    {
        using var db = new SqlConnection(_conn);
        var count = await db.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM dbo.USUARIOS_SISTEMA WHERE Username=@u AND PasswordHash=@p AND Activo=1",
            new { u = username, p = password });
        return count > 0;
    }

    // ──────────────────────────────────────────────────
    // PRODUCTOS
    // ──────────────────────────────────────────────────
    public async Task<IEnumerable<Producto>> GetProductos()
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Producto>(
            "SELECT * FROM dbo.BASE_DATOS_PRODUCTOS WHERE Activo=1 ORDER BY ProductoID");
    }

    public async Task<Producto?> GetProducto(int id)
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryFirstOrDefaultAsync<Producto>(
            "SELECT * FROM dbo.BASE_DATOS_PRODUCTOS WHERE ProductoID=@id", new { id });
    }

    public async Task<int> InsertProducto(Producto p)
    {
        using var db = new SqlConnection(_conn);
        return await db.ExecuteScalarAsync<int>(@"
            INSERT INTO dbo.BASE_DATOS_PRODUCTOS
                (NombreProducto, Categoria, Linea, Silueta, Tiro, Material, ColorPrincipal,
                 Tallas, PrecioBase, FichaT_Ref, Activo, Estado, ImagenURL, Descripcion)
            OUTPUT INSERTED.ProductoID
            VALUES
                (@NombreProducto, @Categoria, @Linea, @Silueta, @Tiro, @Material, @ColorPrincipal,
                 @Tallas, @PrecioBase, @FichaT_Ref, 1, @Estado, @ImagenURL, @Descripcion)", p);
    }

    public async Task UpdateProducto(Producto p)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(@"
            UPDATE dbo.BASE_DATOS_PRODUCTOS SET
                NombreProducto=@NombreProducto, Categoria=@Categoria, Linea=@Linea,
                Silueta=@Silueta, Tiro=@Tiro, Material=@Material, ColorPrincipal=@ColorPrincipal,
                Tallas=@Tallas, PrecioBase=@PrecioBase, FichaT_Ref=@FichaT_Ref,
                Estado=@Estado, ImagenURL=@ImagenURL, Descripcion=@Descripcion
            WHERE ProductoID=@ProductoID", p);
    }

    public async Task DeleteProducto(int id)  // soft delete
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE dbo.BASE_DATOS_PRODUCTOS SET Activo=0 WHERE ProductoID=@id", new { id });
    }

    // ──────────────────────────────────────────────────
    // INVENTARIO
    // ──────────────────────────────────────────────────
    public async Task<IEnumerable<Inventario>> GetInventario()
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Inventario>(@"
            SELECT i.*, p.NombreProducto, p.FichaT_Ref AS Referencia
            FROM dbo.INVENTARIO_PRODUCTOS i
            JOIN dbo.BASE_DATOS_PRODUCTOS p ON p.ProductoID = i.ProductoID
            ORDER BY i.InvID");
    }

    public async Task UpdateStock(int invId, int nuevoStock)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(
            "UPDATE dbo.INVENTARIO_PRODUCTOS SET StockDisponible=@s, FechaUltimaEntrada=GETDATE() WHERE InvID=@id",
            new { s = nuevoStock, id = invId });
    }

    // ──────────────────────────────────────────────────
    // EMPLEADOS
    // ──────────────────────────────────────────────────
    public async Task<IEnumerable<Empleado>> GetEmpleados()
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Empleado>(
            "SELECT * FROM dbo.BASE_DATOS_EMPLEADOS WHERE Activo=1 ORDER BY NombreEmpleado");
    }

    public async Task UpdateEmpleado(Empleado e)
    {
        using var db = new SqlConnection(_conn);
        await db.ExecuteAsync(@"
            UPDATE dbo.BASE_DATOS_EMPLEADOS SET
                NombreEmpleado=@NombreEmpleado, Cargo=@Cargo, TipoContrato=@TipoContrato,
                SalarioBasico=@SalarioBasico, Telefono=@Telefono,
                Direccion_Ciudad=@Direccion_Ciudad, FotoURL=@FotoURL,
                NoCuentaBancaria=@NoCuentaBancaria
            WHERE EmpleadoID=@EmpleadoID", e);
    }

    // ──────────────────────────────────────────────────
    // TERCEROS
    // ──────────────────────────────────────────────────
    public async Task<IEnumerable<Empresa>> GetEmpresas()
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Empresa>(
            "SELECT * FROM dbo.EMPRESAS WHERE Activo=1 ORDER BY Nombre");
    }

    public async Task<IEnumerable<Proveedor>> GetProveedores()
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Proveedor>(
            "SELECT * FROM dbo.PROVEEDORES WHERE Activo=1 ORDER BY Nombre");
    }

    public async Task<IEnumerable<Banco>> GetBancos()
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryAsync<Banco>(
            "SELECT * FROM dbo.BANCOS WHERE Activo=1 ORDER BY Nombre");
    }

    // ──────────────────────────────────────────────────
    // DASHBOARD — RESUMEN EJECUTIVO
    // ──────────────────────────────────────────────────
    public async Task<ResumenEjecutivo> GetResumen()
    {
        using var db = new SqlConnection(_conn);
        return await db.QueryFirstAsync<ResumenEjecutivo>(@"
            SELECT
                (SELECT COUNT(1) FROM dbo.EMPRESAS    WHERE Activo=1) AS TotalEmpresas,
                (SELECT COUNT(1) FROM dbo.PROVEEDORES WHERE Activo=1) AS TotalProveedores,
                (SELECT COUNT(1) FROM dbo.BANCOS      WHERE Activo=1) AS TotalBancos,
                (SELECT COUNT(1) FROM dbo.BASE_DATOS_EMPLEADOS WHERE Activo=1) AS TotalEmpleados,
                (SELECT COUNT(1) FROM dbo.BASE_DATOS_PRODUCTOS WHERE Activo=1) AS TotalProductos,
                (SELECT COUNT(1) FROM dbo.INVENTARIO_PRODUCTOS) AS TotalVariantesInventario,
                ISNULL((SELECT SUM(StockDisponible) FROM dbo.INVENTARIO_PRODUCTOS), 0) AS UnidadesEnInventario,
                ISNULL((SELECT SUM(StockDisponible * PrecioVenta) FROM dbo.INVENTARIO_PRODUCTOS), 0) AS ValorTotalInventario,
                ISNULL((SELECT SUM(SalarioBasico) FROM dbo.BASE_DATOS_EMPLEADOS WHERE Activo=1), 0) AS NominaMensualTotal
        ");
    }
}
