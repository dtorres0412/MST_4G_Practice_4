using System.Security.Cryptography;

namespace MST_4G_Self_Practice_4.Dtos;

public class ReadZoDto
{
    public int ZoId { get; set; }
    public string ZoNo { get; set; } = string.Empty;
    public string ZoName { get; set; } = string.Empty;
}