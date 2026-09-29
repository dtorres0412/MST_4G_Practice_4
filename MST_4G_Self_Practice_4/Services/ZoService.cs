using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MST_4G_Self_Practice_4.Data;
using MST_4G_Self_Practice_4.Dtos;
using MST_4G_Self_Practice_4.Models;
using MST_4G_Self_Practice_4.Extensions;

namespace MST_4G_Self_Practice_4.Services;

public class ZoService(AppDbContext context) : IZoService
{
    public async Task<ReadZoDto?>GetByZoNoAsync(string zoNo)
    {

        Validations.ValidateBusinessRules((zoNo, 4, allowSpaces: false));
        return await context.Zo
            .AsNoTracking()
            .Where(z => z.ZoNo == zoNo.Trim())
            .Select(z => new ReadZoDto
            {
                ZoNo = z.ZoNo,
                ZoName = z.ZoName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ReadZoDto>CreateZoAsync(CreateZoDto createZoDto)
    {

        Validations.ValidateBusinessRules(
            (createZoDto.ZoNo,4, allowSpaces: false),
            (createZoDto.ZoName,50, allowSpaces: false)
        );
        string zoNo = createZoDto.ZoNo.Trim();
        string zoName = createZoDto.ZoName.Trim();

        var existingZo = await context.Zo
            .FirstOrDefaultAsync(z => z.ZoNo == zoNo);

        if(existingZo != null)
        {
            throw new ArgumentException($"Zone number '{zoNo} already existed'");    
        }

        var newZo = new Zo
        {
            ZoNo = zoNo,
            ZoName = zoName
        };

        context.Zo.Add(newZo);
        await context.SaveChangesAsync();

        return new ReadZoDto
        {
            ZoId = newZo.ZoId,
            ZoNo = newZo.ZoNo,
            ZoName = newZo.ZoName
        };
    }

    public async Task<ReadZoDto>UpdateZoAsync(UpdateZoDto updateZoDto)
    {
        Validations.ValidateBusinessRules(
            (updateZoDto.ZoNo,4, allowSpaces: false),
            (updateZoDto.ZoName,50, allowSpaces: false)
        );
            var existingZo = await context.Zo
            .FirstOrDefaultAsync(z => z.ZoId == updateZoDto.ZoId);

            if(existingZo == null)
            {
                return null;
            }

            existingZo.ZoNo = updateZoDto.ZoNo.Trim();
            existingZo.ZoName = updateZoDto.ZoName.Trim();

            await context.SaveChangesAsync();

            return new ReadZoDto
            {
                ZoId = existingZo.ZoId,
                ZoNo = existingZo.ZoNo,
                ZoName = existingZo.ZoName
            };
    }

    public async Task<bool>DeleteZoAsync(int zoId)
    {
        var zoRecord = await context.Zo.FindAsync(zoId);

        if(zoRecord == null)
        {
            return false;
        }

        context.Zo.Remove(zoRecord);
        await context.SaveChangesAsync();

        return true;
    }
}