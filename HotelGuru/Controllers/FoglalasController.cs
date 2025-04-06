using HotelGuru.Services;
using HotelGuru.DataContext.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HotelGuru.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoglalasController : ControllerBase
{
    private readonly IFoglalasService _service;

    public FoglalasController(IFoglalasService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllFoglalasokAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _service.GetFoglalasByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(FoglalasCreateDto dto)
        => Ok(await _service.CreateFoglalasAsync(dto));

    [HttpDelete("lemondas/{id}")]
    public async Task<IActionResult> Lemondas(int id)
        => Ok(await _service.LemondasAsync(id));

    [HttpPut("visszaigazolas/{id}")]
    public async Task<IActionResult> Visszaigazolas(int id)
        => Ok(await _service.VisszaigazolasAsync(id));
}
