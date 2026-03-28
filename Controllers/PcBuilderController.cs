using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Repositories;

[Route("api/pcbuilder")]
[ApiController]
public class PcBuilderController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IProductRepository _repo;

    public PcBuilderController(ApplicationDbContext context, IProductRepository repo)
    {
        _context = context;
        _repo = repo;
    }

    [HttpGet("components")]
    public async Task<ActionResult<object[]>> GetComponents(string type)
    {
        var (items, _) = await _repo.GetPagedAsync(1, 20, type, null);
        var result = items.Select(p => new
        {
            id = p.Id,
            name = p.Name,
            price = p.Price,
            description = p.Description ?? "",
            imageUrl = p.ImageUrl ?? ""
        }).ToArray();
        return Ok(result);
    }

    [HttpGet("auto-build")]
    public async Task<ActionResult<object>> AutoBuild(decimal budget)
    {
        var types = new Dictionary<string, decimal>
        {
            {"CPU", 0.25m}, {"GPU", 0.35m}, {"RAM", 0.10m}, {"SSD", 0.10m},
            {"MAINBOARD", 0.10m}, {"PSU", 0.10m}
        };

        var build = new Dictionary<string, object>();
        decimal total = 0;

        foreach (var kv in types)
        {
            var compType = kv.Key;
            var maxPrice = budget * kv.Value;
            var (comps, _) = await _repo.GetPagedAsync(1, 10, compType, null);

            var comp = comps.FirstOrDefault(p => p.Price <= maxPrice && p.Price > 0);
            if (comp == null)
            {
                comp = comps.OrderBy(p => p.Price).FirstOrDefault(p => p.Price > 0);
            }

            if (comp != null)
            {
                build[compType.ToLower()] = new { id = comp.Id, name = comp.Name, price = comp.Price };
                total += comp.Price;
            }
        }

        return Ok(new { build, total });
    }
}

