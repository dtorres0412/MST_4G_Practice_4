using MST_4G_Self_Practice_4.Dtos;

namespace MST_4G_Self_Practice_4.Services;

public interface IZoService
{
    Task<ReadZoDto>CreateZoAsync(CreateZoDto createZoDto);
    Task<ReadZoDto>GetByZoNoAsync(string ZoNo);
    Task<ReadZoDto>UpdateZoAsync(UpdateZoDto updateZoDto);
    Task<bool>DeleteZoAsync(int zoId);
}