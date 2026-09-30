using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MST_4G_Self_Practice_4.Data;
using MST_4G_Self_Practice_4.Dtos;
using MST_4G_Self_Practice_4.Models;
using MST_4G_Self_Practice_4.Services;
using Xunit;
using Xunit.Sdk;

namespace MST_4G_Testing;

public class ZoTest : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ZoService _service;

    public static IEnumerable<object[]> GetNotExistedZoNumbers()
    {
        yield return new object[] { "Z009" };
        yield return new object[] { "Z010" };
        yield return new object[] { "Z011" };
    }

     public static IEnumerable<object[]> GetNotExistedZoNames()
    {
        yield return new object[] { "Zone 9" };
        yield return new object[] { "Zone 10" };
        yield return new object[] { "Zone 11" };
    }

    public static IEnumerable<object[]> GetNotExistedZoDetails()
    {
        yield return new object[] { "Z009", "Zone 9" };
        yield return new object[] { "Z010", "Zone 10" };
        yield return new object[] { "Z011", "Zone 11" };
    }

    public static IEnumerable<object[]> GetSpecialSymbols()
    {
        yield return new object[] { "^" };
        yield return new object[] { "<" };
        yield return new object[] { ">" };
        yield return new object[] { "|" };
        yield return new object[] { "&" };
        yield return new object[] { "\"" };
        yield return new object[] { "'" };
        yield return new object[] { "," };
    }

    // 1. Set-up
    public ZoTest()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        SeedTestData();

        _service = new ZoService(_context);
    }

    private void SeedTestData()
    {
        var zos = new List<Zo>
        {
            new Zo { ZoId = 1, ZoNo = "Z001", ZoName = "Zone 1" },
            new Zo { ZoId = 2, ZoNo = "Z002", ZoName = "Zone 2" },
            new Zo { ZoId = 3, ZoNo = "Z003", ZoName = "Zone 3" },
            new Zo { ZoId = 4, ZoNo = "Z004", ZoName = "Zone 4" },
            new Zo { ZoId = 5, ZoNo = "Z005", ZoName = "Zone 5" },
            new Zo { ZoId = 6, ZoNo = "Z006", ZoName = "Zone 6" },
            new Zo { ZoId = 7, ZoNo = "Z007", ZoName = "Zone 7" }            
        };
        _context.Zo.AddRange(zos);
        var zo = new Zo { ZoId = 8, ZoNo = "Z008", ZoName = "Zone 8" };
        _context.Zo.Add(zo);
        _context.SaveChanges();
    }

    //2. Teardown
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Theory]
    [MemberData(nameof(GetNotExistedZoNumbers))]
    public async Task GetByZoNoAsync_WhenZoNoExists_ReturnsZoNumber(string nonExistedZoNo)
    {
        var result = await _service.GetByZoNoAsync(nonExistedZoNo);

        Assert.Null(result);
    }
    

    // [Fact]
    // public async Task CreateZoAsync_WhenMasterZoDoesNotExist_CreatesNewMasterAndJunction()
    // {
    //     var createZoDto = new CreateZoDto
    //     {
    //         ZoNo = "Z005",
    //         ZoName = "Zone 5",
    //     };

    //     var result = await _service.CreateZoAsync(createZoDto);

    //     Assert.NotNull(result);
    //     Assert.Equal("Z005", result.ZoNo);
    //     Assert.Equal("Zone 5", result.ZoName);

    //     var savedZoMaster = await _context.Zo.FirstOrDefaultAsync(z => z.ZoNo == "Z005");
    //     Assert.NotNull(savedZoMaster);
    // }

    // [Theory]
    // [InlineData("Z005","Zone 5")]
    // [InlineData("Z006","Zone 6")]
    // public async Task CreateZoAsync_WhenMasterZoDoesNotExist_CreatesNewMasterAndJunction(string zoNo, string zoName)
    // {
    //     var createZoDto = new CreateZoDto
    //     {
    //         ZoNo = zoNo,
    //         ZoName = zoName,
    //     };

    //     var result = await _service.CreateZoAsync(createZoDto);

    //     Assert.NotNull(result);
    //     Assert.Equal(zoNo, result.ZoNo);
    //     Assert.Equal(zoName, result.ZoName);

    //     var savedZoMaster = await _context.Zo.FirstOrDefaultAsync(z => z.ZoNo == zoNo);
    //     Assert.NotNull(savedZoMaster);
    // }

    [Theory]
    [MemberData(nameof(GetNotExistedZoDetails))]
    public async Task CreateZoAsync_WhenMasterZoDoesNotExist_CreatesNewMasterAndJunction(string zoNo, string zoName)
    {
        var createZoDto = new CreateZoDto
        {
            ZoNo = zoNo,
            ZoName = zoName
        };

        var result = await _service.CreateZoAsync(createZoDto);

        Assert.NotNull(result);
        Assert.Equal(zoNo, result.ZoNo);
        Assert.Equal(zoName, result.ZoName);

        var savedZoMaster = await _context.Zo.FirstOrDefaultAsync(z => z.ZoNo == zoNo && z.ZoName == zoName);
        Assert.NotNull(savedZoMaster);
    }

    [Fact]
    public async Task UpdateZoAsync_WhenZoExists_UpdatesZoDetails()
    {
        var updateZoDto = new UpdateZoDto
        {
            ZoId = 4, 
            ZoNo = "Z004",
            ZoName = "Zone 40"
        };

        var result = await _service.UpdateZoAsync(updateZoDto);

        Assert.NotNull(result);
        Assert.Equal("Zone 40", result.ZoName);
    }

    [Theory]
    [MemberData(nameof(GetSpecialSymbols))]
    public async Task CreateZoAsync_WhenInputHasSpecialSymbols_ThrowsSpecialSymbolError(string specialSymbol)
    {
        var invalidDto = new CreateZoDto
        {
            ZoNo = $"Z{specialSymbol}01",
            ZoName = "Zone 1"
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateZoAsync(invalidDto));
        Assert.Contains("Special symbols are not accepted", exception.Message);
    }
}