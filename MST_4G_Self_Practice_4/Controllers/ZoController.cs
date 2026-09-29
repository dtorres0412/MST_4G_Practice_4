using Microsoft.AspNetCore.Mvc;
using MST_4G_Self_Practice_4.Services;
using MST_4G_Self_Practice_4.Dtos;

namespace MST_4G_Self_Practice_4.Controllers;

[ApiController]
[Route("[controller]")]
public class ZoController(IZoService zoService) : ControllerBase
{
   [HttpGet("{zoNo}")]
   public async Task<ActionResult<ReadZoDto>>GetByZoNo(string zoNo)
    {
        var Zo = await zoService.GetByZoNoAsync(zoNo);

        if(Zo == null)
        {
            return NotFound($"ZoNo record of '{zoNo} not found.'");
        }
        return Ok(Zo);       
    }

    [HttpPost("create")]
    public async Task<ActionResult<ReadZoDto>>CreateZo([FromBody] CreateZoDto createZoDto)
    {
        try
        {
            var result = await zoService.CreateZoAsync(createZoDto);

            return CreatedAtAction(nameof(GetByZoNo), new {zoNo = result?.ZoNo}, result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult<ReadZoDto>>UpdateZo([FromBody] UpdateZoDto updateZoDto)
    {
        var result = await zoService.UpdateZoAsync(updateZoDto);

        try
        {
            if(result == null)
            {
                return NotFound($"ZoNo record '{updateZoDto.ZoNo}' is not found.");
            }
            return Ok(result);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{zoId:Int}")]
    public async Task<IActionResult>DeleteZo(int zoId)
    {
        var isDeleted = await zoService.DeleteZoAsync(zoId);
        if (!isDeleted)
        {
            return NotFound($"ZoId {zoId} record not found");
        }

        return NoContent();
    }
}
