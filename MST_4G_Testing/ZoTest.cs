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

    public static IEnumerable<object[]> GetExceededZoNo()
    {
        yield return new object[] {"123456789"};
        yield return new object[] {"51281"};
        yield return new object[] {"51231231"};
    }

    public static IEnumerable<object[]> GetExistedZoIds()
    {
        yield return new object[] { 1 };
        yield return new object[] { 2 };
        yield return new object[] { 3 };
        yield return new object[] { 4 };
        yield return new object[] { 5 };
        yield return new object[] { 6 };
    }

    public static IEnumerable<object[]> GetNonExistedZoIds()
    {
        yield return new object[] { 21 };
        yield return new object[] { 32 };
        yield return new object[] { 45 };
        yield return new object[] { 55 };
        yield return new object[] { 67 };
        yield return new object[] { 79 };
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

    // [Fact]
    // public async Task GetByZoNoAsync_WhenZoNoExceedsMaxLength_ThrowsMaxLengthErrorMessage()
    // {
    //     var readZoDto = new ReadZoDto
    //     {
    //         ZoNo = "123456789"
    //     };

    //     var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByZoNoAsync(readZoDto.ZoNo));
    //     Assert.Contains("The input exceeds on the maxlength of 4", exception.Message);
    // }

    [Theory]
    [MemberData(nameof(GetExceededZoNo))]
    public async Task GetByZoNoAsync_WhenZoNoExceedsMaxLength_ThrowsMaxLengthErrorMessage(string zoNo)
    {
        var readZoDto = new ReadZoDto
        {
            ZoNo = zoNo
        };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.GetByZoNoAsync(readZoDto.ZoNo));
        Assert.Contains("The input exceeds on the maxlength of 4", exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetExceededZoNo))]
    public async Task CreateZoAsync_WhenZoNoExceedsMaxLength_ThrowsMaxLengthErrorMessage(string zoNo)
    {
        var createZoDto = new CreateZoDto
        {
            ZoNo = zoNo
        };

       var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateZoAsync(createZoDto));
       Assert.Contains("The input exceeds on the maxlength of 4", exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetExceededZoNo))]
    public async Task UpdateZoAsync_WhenZoNoExceedsMaxLength_ThrowsMaxLengthErrorMessage(string zoNo)
    {
        var updateZoDto = new UpdateZoDto
        {
            ZoNo = zoNo
        };

       var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateZoAsync(updateZoDto));
       Assert.Contains("The input exceeds on the maxlength of 4", exception.Message);
    }

    // [Fact]
    // public async Task DeleteZoAsync_WhenZoIdExists_DeletesAndReturnsTrue()
    // {
    //     int existingZoId = 1;

    //     var result = await _service.DeleteZoAsync(existingZoId);

    //     Assert.True(result);

    //     var deletedRecord = await _context.Zo.FindAsync(existingZoId);
    //     Assert.Null(deletedRecord);
    // }

    // [Fact]
    // public async Task DeleteZoAsync_WhenZoIdDoesNotExist_ReturnsFalse()
    // {
    //     int nonExistedZoId = 2141;

    //     var result = await _service.DeleteZoAsync(nonExistedZoId);

    //     Assert.False(result);
    // }

    [Theory]
    [MemberData(nameof(GetExistedZoIds))]
    public async Task DeleteZoAsync_WhenZoIdExists_DeletesAndReturnsTrue(int zoId)
    {
        int existingZoId = zoId;

        var result = await _service.DeleteZoAsync(existingZoId);

        Assert.True(result);

        var deletedRecord = await _context.Zo.FindAsync(existingZoId);
        Assert.Null(deletedRecord);
    }

    [Theory]
    [MemberData(nameof(GetNonExistedZoIds))]
    public async Task DeleteZoAsync_WhenZoIdDoesNotExist_ReturnsFalse(int zoId)
    {
        int nonExistedZoId = zoId;

        var result = await _service.DeleteZoAsync(nonExistedZoId);

        Assert.False(result);
    }
}