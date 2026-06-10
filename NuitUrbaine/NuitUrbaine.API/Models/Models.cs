namespace NuitUrbaine.API.Models;

public class Producto
{
    public int ProductoID { get; set; }
    public string NombreProducto { get; set; } = "";
    public string? Categoria { get; set; }
    public string? Linea { get; set; }
    public string? Silueta { get; set; }
    public string? Tiro { get; set; }
    public string? Material { get; set; }
    public string? ColorPrincipal { get; set; }
    public string? Tallas { get; set; }
    public decimal PrecioBase { get; set; }
    public string? FichaT_Ref { get; set; }
    public bool Activo { get; set; }
    public string? Estado { get; set; }
    public string? ImagenURL { get; set; }
    public string? Descripcion { get; set; }
}

public class Inventario
{
    public int InvID { get; set; }
    public int ProductoID { get; set; }
    public string? NombreProducto { get; set; }
    public string? Referencia { get; set; }
    public string? Talla { get; set; }
    public string? Color { get; set; }
    public int StockDisponible { get; set; }
    public int StockMinimo { get; set; }
    public int StockMaximo { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal PrecioVenta { get; set; }
    public string? Estado { get; set; }
    public string? Ubicacion { get; set; }
    public DateTime? FechaUltimaEntrada { get; set; }
}

public class Empleado
{
    public int EmpleadoID { get; set; }
    public string NombreEmpleado { get; set; } = "";
    public string? NoDocumento { get; set; }
    public string? Cargo { get; set; }
    public string? TipoContrato { get; set; }
    public decimal SalarioBasico { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion_Ciudad { get; set; }
    public bool Activo { get; set; }
    public string? FotoURL { get; set; }
    public string? NoCuentaBancaria { get; set; }
}

public class Empresa
{
    public int EmpresaID { get; set; }
    public string Nombre { get; set; } = "";
    public string? NIT { get; set; }
    public string? RazonSocial_TipoSociedad { get; set; }
    public string? TipoRegimen { get; set; }
    public int? GrupoNIIF { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
}

public class Proveedor
{
    public int ProveedorID { get; set; }
    public string Nombre { get; set; } = "";
    public string? NIT { get; set; }
    public string? RazonSocial_TipoSociedad { get; set; }
    public string? TipoRegimen { get; set; }
    public int? GrupoNIIF { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
}

public class Banco
{
    public int BancoID { get; set; }
    public string Nombre { get; set; } = "";
    public string? NIT { get; set; }
    public string? RazonSocial_TipoSociedad { get; set; }
    public string? TipoRegimen { get; set; }
    public int? GrupoNIIF { get; set; }
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
}

public class ResumenEjecutivo
{
    public int TotalEmpresas { get; set; }
    public int TotalProveedores { get; set; }
    public int TotalBancos { get; set; }
    public int TotalEmpleados { get; set; }
    public int TotalProductos { get; set; }
    public int TotalVariantesInventario { get; set; }
    public int UnidadesEnInventario { get; set; }
    public decimal ValorTotalInventario { get; set; }
    public decimal NominaMensualTotal { get; set; }
}

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
