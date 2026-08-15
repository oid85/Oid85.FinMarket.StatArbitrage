using Oid85.FinMarket.StatArbitrage.Application.Helpers;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Test.Oid85.FinMarket.StatArbitrage.Application;

public class PortfolioPositionHelperTests
{
    [Fact]
    public void Create_long_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.CreateNewPortfolioPosition(
            targetWeight: 5, targetSize: 10, currentPrice: 10.0);

        //Assert
        Assert.Equal(-100.0, sut.MoneyChange);

        Assert.Equal(10.0, sut.Position.EntryPrice);
        Assert.Equal(5, sut.Position.Weight);
        Assert.Equal(10, sut.Position.Size);
        Assert.Equal(0.0, sut.Position.Profit);
    }

    [Fact]
    public void Create_short_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.CreateNewPortfolioPosition(
            targetWeight: -5, targetSize: 10, currentPrice: 10.0);

        //Assert
        Assert.Equal(-100.0, sut.MoneyChange);

        Assert.Equal(10.0, sut.Position.EntryPrice);
        Assert.Equal(-5, sut.Position.Weight);
        Assert.Equal(10, sut.Position.Size);
        Assert.Equal(0.0, sut.Position.Profit);
    }

    [Fact]
    public void Up_long_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.UpLongPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = 5,
                Size = 10
            },
            targetWeight: 7, targetSize: 15, currentPrice: 11.0);

        //Assert
        Assert.Equal(-55.0, sut.MoneyChange);

        Assert.True(Math.Abs(sut.Position.EntryPrice!.Value - 10.33333) < 0.001);
        Assert.Equal(7, sut.Position.Weight);
        Assert.Equal(15, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 10.0) < 0.001);
    }

    [Fact]
    public void Up_short_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.UpShortPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = -5,
                Size = 10
            },
            targetWeight: -7, targetSize: 15, currentPrice: 9.0);

        //Assert
        Assert.Equal(-45.0, sut.MoneyChange);

        Assert.True(Math.Abs(sut.Position.EntryPrice!.Value - 9.66666) < 0.001);
        Assert.Equal(-7, sut.Position.Weight);
        Assert.Equal(15, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 10.0) < 0.001);
    }

    [Fact]
    public void Down_long_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.DownLongPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = 5,
                Size = 10
            },
            targetWeight: 3, targetSize: 5, currentPrice: 11.0);

        //Assert
        Assert.Equal(55.0, sut.MoneyChange);

        Assert.True(Math.Abs(sut.Position.EntryPrice!.Value - 10.0) < 0.001);
        Assert.Equal(3, sut.Position.Weight);
        Assert.Equal(5, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 5.0) < 0.001);
    }

    [Fact]
    public void Down_short_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.DownShortPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = 5,
                Size = 10
            },
            targetWeight: 3, targetSize: 5, currentPrice: 9.0);

        //Assert
        Assert.Equal(45.0, sut.MoneyChange);

        Assert.True(Math.Abs(sut.Position.EntryPrice!.Value - 10.0) < 0.001);
        Assert.Equal(3, sut.Position.Weight);
        Assert.Equal(5, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 5.0) < 0.001);
    }

    [Fact]
    public void Reverse_long_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.ReverseLongPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = 5,
                Size = 10
            },
            targetWeight: -3, targetSize: 3, currentPrice: 11.0);

        //Assert
        Assert.Equal(110.0 - 33.0, sut.MoneyChange);

        Assert.True(Math.Abs(sut.Position.EntryPrice!.Value - 11.0) < 0.001);
        Assert.Equal(-3, sut.Position.Weight);
        Assert.Equal(3, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 0.0) < 0.001);
    }

    [Fact]
    public void Reverse_short_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.ReverseShortPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = -5,
                Size = 10
            },
            targetWeight: 3, targetSize: 3, currentPrice: 9.0);

        //Assert
        Assert.Equal(100.0 + 10.0 - 27.0, sut.MoneyChange);

        Assert.True(Math.Abs(sut.Position.EntryPrice!.Value - 9.0) < 0.001);
        Assert.Equal(3, sut.Position.Weight);
        Assert.Equal(3, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 0.0) < 0.001);
    }

    [Fact]
    public void Close_long_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.CloseLongPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = 5,
                Size = 10
            },
            currentPrice: 11.0);

        //Assert
        Assert.Equal(110.0, sut.MoneyChange);

        Assert.Null(sut.Position.EntryPrice);
        Assert.Equal(0, sut.Position.Weight);
        Assert.Equal(0, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 0.0) < 0.001);
    }

    [Fact]
    public void Close_short_position_return_is_correct()
    {
        // Arrange

        // Act
        var sut = PortfolioPositionHelper.CloseShortPortfolioPosition(
            new PortfolioPosition
            {
                EntryPrice = 10.0,
                Weight = -5,
                Size = 10
            },
            currentPrice: 9.0);

        //Assert
        Assert.Equal(110.0, sut.MoneyChange);

        Assert.Null(sut.Position.EntryPrice);
        Assert.Equal(0, sut.Position.Weight);
        Assert.Equal(0, sut.Position.Size);
        Assert.True(Math.Abs(sut.Position.Profit - 0.0) < 0.001);
    }
}