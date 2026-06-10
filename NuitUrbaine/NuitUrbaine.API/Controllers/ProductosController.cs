using Microsoft.AspNetCore.Mvc;
using NuitUrbaine.API.Data;
using NuitUrbaine.API.Models;

namespace NuitUrbaine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly NuitRepository _repo;
    public ProductosController(NuitRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _repo.GetProductos());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var p = await _repo.GetProducto(id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Producto p)
    {
        var newId = await _repo.InsertProducto(p);
        return Ok(new { ProductoID = newId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Producto p)
    {
        p.ProductoID = id;
        await _repo.UpdateProducto(p);
        return Ok(new { ok = true });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.DeleteProducto(id);
        return Ok(new { ok = true });
    }
}
