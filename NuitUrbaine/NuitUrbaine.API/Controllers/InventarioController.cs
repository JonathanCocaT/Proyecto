using Microsoft.AspNetCore.Mvc;
using NuitUrbaine.API.Data;

namespace NuitUrbaine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventarioController : ControllerBase
{
    private readonly NuitRepository _repo;
    public InventarioController(NuitRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _repo.GetInventario());

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] StockUpdateRequest req)
    {
        await _repo.UpdateStock(id, req.Stock);
        return Ok(new { ok = true });
    }
}

public record StockUpdateRequest(int Stock);
