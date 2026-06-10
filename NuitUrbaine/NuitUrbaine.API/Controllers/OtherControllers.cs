using Microsoft.AspNetCore.Mvc;
using NuitUrbaine.API.Data;
using NuitUrbaine.API.Models;

namespace NuitUrbaine.API.Controllers;

// ── TERCEROS ──
[ApiController]
[Route("api/[controller]")]
public class TercerosController : ControllerBase
{
    private readonly NuitRepository _repo;
    public TercerosController(NuitRepository repo) => _repo = repo;

    [HttpGet("empresas")]
    public async Task<IActionResult> Empresas() => Ok(await _repo.GetEmpresas());

    [HttpGet("proveedores")]
    public async Task<IActionResult> Proveedores() => Ok(await _repo.GetProveedores());

    [HttpGet("bancos")]
    public async Task<IActionResult> Bancos() => Ok(await _repo.GetBancos());
}

// ── AUTENTICACIÓN ──
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly NuitRepository _repo;
    public AuthController(NuitRepository repo) => _repo = repo;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var ok = await _repo.ValidarLogin(req.Username, req.Password);
        if (!ok) return Unauthorized(new { error = "Usuario o contraseña incorrectos." });
        return Ok(new { ok = true, rol = "admin" });
    }
}

// ── DASHBOARD ──
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly NuitRepository _repo;
    public DashboardController(NuitRepository repo) => _repo = repo;

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen() => Ok(await _repo.GetResumen());
}
