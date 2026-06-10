using Microsoft.AspNetCore.Mvc;
using NuitUrbaine.API.Data;
using NuitUrbaine.API.Models;

namespace NuitUrbaine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly NuitRepository _repo;
    public EmpleadosController(NuitRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _repo.GetEmpleados());

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Empleado e)
    {
        e.EmpleadoID = id;
        await _repo.UpdateEmpleado(e);
        return Ok(new { ok = true });
    }
}
