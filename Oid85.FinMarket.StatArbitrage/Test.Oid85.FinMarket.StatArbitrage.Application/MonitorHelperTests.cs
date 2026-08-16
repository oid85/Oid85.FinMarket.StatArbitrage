using Oid85.FinMarket.StatArbitrage.Application.Helpers;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Test.Oid85.FinMarket.StatArbitrage.Application;

public class MonitorHelperTests
{
    [Fact]
    public void Map_Positions_return_is_correct()
    {
        // Arrange
        List<DateOnly> dates =
                [
                    DateOnly.Parse("2026.07.01"),
                    DateOnly.Parse("2026.07.02"),
                    DateOnly.Parse("2026.07.03"),
                    DateOnly.Parse("2026.07.04"),
                    DateOnly.Parse("2026.07.05"),
                    DateOnly.Parse("2026.07.06"),
                    DateOnly.Parse("2026.07.07"),
                    DateOnly.Parse("2026.07.08"),
                    DateOnly.Parse("2026.07.09"),
                    DateOnly.Parse("2026.07.10")
                ];

        SortedDictionary<DateOnly, Position> positions = new()
        {
            { DateOnly.Parse("2026.07.02"), new() { IsActive = false, IsLongShort = true, IsShortLong = false, EntryDate = DateOnly.Parse("2026.07.02"), ExitDate = DateOnly.Parse("2026.07.04") } },
            { DateOnly.Parse("2026.07.06"), new() { IsActive = false, IsLongShort = false, IsShortLong = true, EntryDate = DateOnly.Parse("2026.07.06"), ExitDate = DateOnly.Parse("2026.07.07") } },
            { DateOnly.Parse("2026.07.09"), new() { IsActive = true, IsLongShort = true, IsShortLong = false, EntryDate = DateOnly.Parse("2026.07.09"), ExitDate = null } }
        };

        // Act
        List<DateValue<int>> expectedFirst =
            [
                new () { Date = DateOnly.Parse("2026.07.01"), Value = 0 },
                new () { Date = DateOnly.Parse("2026.07.02"), Value = 1 },
                new () { Date = DateOnly.Parse("2026.07.03"), Value = 1 },
                new () { Date = DateOnly.Parse("2026.07.04"), Value = 1 },
                new () { Date = DateOnly.Parse("2026.07.05"), Value = 0 },
                new () { Date = DateOnly.Parse("2026.07.06"), Value = -1 },
                new () { Date = DateOnly.Parse("2026.07.07"), Value = -1 },
                new () { Date = DateOnly.Parse("2026.07.08"), Value = 0 },
                new () { Date = DateOnly.Parse("2026.07.09"), Value = 1 },
                new () { Date = DateOnly.Parse("2026.07.10"), Value = 1 }
            ];

        List<DateValue<int>> expectedSecond =
            [
                new () { Date = DateOnly.Parse("2026.07.01"), Value = 0 },
                new () { Date = DateOnly.Parse("2026.07.02"), Value = -1 },
                new () { Date = DateOnly.Parse("2026.07.03"), Value = -1 },
                new () { Date = DateOnly.Parse("2026.07.04"), Value = -1 },
                new () { Date = DateOnly.Parse("2026.07.05"), Value = 0 },
                new () { Date = DateOnly.Parse("2026.07.06"), Value = 1 },
                new () { Date = DateOnly.Parse("2026.07.07"), Value = 1 },
                new () { Date = DateOnly.Parse("2026.07.08"), Value = 0 },
                new () { Date = DateOnly.Parse("2026.07.09"), Value = -1 },
                new () { Date = DateOnly.Parse("2026.07.10"), Value = -1 }
            ];

        var sut = MonitorHelper.Map(positions, dates);

        //Assert
        Assert.Equivalent(sut.First, expectedFirst);
        Assert.Equivalent(sut.Second, expectedSecond);
    }

    [Fact]
    public void Merge_return_is_correct()
    {
        // Arrange
        List<DateOnly> dates =
                [
                    DateOnly.Parse("2026.07.01"),
                    DateOnly.Parse("2026.07.02"),
                    DateOnly.Parse("2026.07.03")
                ];

        List<List<DateValue<int>>> data =
            [
                [
                    new () { Date = DateOnly.Parse("2026.07.01"), Value = 0 },
                    new () { Date = DateOnly.Parse("2026.07.02"), Value = 1 },
                    new () { Date = DateOnly.Parse("2026.07.03"), Value = 1 }
                ],
                [
                    new () { Date = DateOnly.Parse("2026.07.01"), Value = 0 },
                    new () { Date = DateOnly.Parse("2026.07.02"), Value = 1 },
                    new () { Date = DateOnly.Parse("2026.07.03"), Value = 0 }
                ],
                [
                    new () { Date = DateOnly.Parse("2026.07.01"), Value = 1 },
                    new () { Date = DateOnly.Parse("2026.07.02"), Value = 1 },
                    new () { Date = DateOnly.Parse("2026.07.03"), Value = 0 }
                ]
            ];

        // Act
        List<DateValue<int>> expected =
            [
                new () { Date = DateOnly.Parse("2026.07.01"), Value = 1 },
                new () { Date = DateOnly.Parse("2026.07.02"), Value = 3 },
                new () { Date = DateOnly.Parse("2026.07.03"), Value = 1 }
            ];

        var sut = MonitorHelper.Merge(data, dates);

        //Assert
        Assert.Equivalent(sut, expected);
    }

    [Fact]
    public void GetPositionWeightData_return_is_correct()
    {
        // Arrange
        List<DateOnly> dates =
                [
                    DateOnly.Parse("2026.07.01"),
                    DateOnly.Parse("2026.07.02"),
                    DateOnly.Parse("2026.07.03"),
                    DateOnly.Parse("2026.07.04"),
                    DateOnly.Parse("2026.07.05"),
                    DateOnly.Parse("2026.07.06"),
                    DateOnly.Parse("2026.07.07"),
                    DateOnly.Parse("2026.07.08"),
                    DateOnly.Parse("2026.07.09"),
                    DateOnly.Parse("2026.07.10")
                ];
        List<string> tickers =
                [
                    "TickerA",
                    "TickerB",
                    "TickerC"
                ];

        List<StrategyExecuteResult> strategyExecuteResults =
            [
                new StrategyExecuteResult
                {
                    TickerFirst = "TickerA",
                    TickerSecond = "TickerC",

                    Positions = new SortedDictionary<DateOnly, Position>
                    {
                        { DateOnly.Parse("2026.07.02"), new Position { IsLongShort = true, IsShortLong = false, EntryDate = DateOnly.Parse("2026.07.02"), ExitDate = DateOnly.Parse("2026.07.07") } }
                    }
                },

                new StrategyExecuteResult
                {
                    TickerFirst = "TickerB",
                    TickerSecond = "TickerC",

                    Positions = new SortedDictionary<DateOnly, Position>
                    {
                        { DateOnly.Parse("2026.07.05"), new Position { IsLongShort = true, IsShortLong = false, EntryDate = DateOnly.Parse("2026.07.05"), ExitDate = null } }
                    }
                }
            ];

        // Act
        var sut = MonitorHelper.GetPositionWeightData(strategyExecuteResults, tickers, dates);

        var sutA = sut.Find(x => x.Ticker == "TickerA").WeightData;
        var sutB = sut.Find(x => x.Ticker == "TickerB").WeightData;
        var sutC = sut.Find(x => x.Ticker == "TickerC").WeightData;

        // Assert
        Assert.NotNull(sut);

        Assert.Equal(0, sutA[0].Weight); // 2026.07.01
        Assert.Equal(1, sutA[1].Weight); // 2026.07.02
        Assert.Equal(1, sutA[2].Weight); // 2026.07.03
        Assert.Equal(1, sutA[3].Weight); // 2026.07.04
        Assert.Equal(1, sutA[4].Weight); // 2026.07.05
        Assert.Equal(1, sutA[5].Weight); // 2026.07.06
        Assert.Equal(1, sutA[6].Weight); // 2026.07.07
        Assert.Equal(0, sutA[7].Weight); // 2026.07.08
        Assert.Equal(0, sutA[8].Weight); // 2026.07.09
        Assert.Equal(0, sutA[9].Weight); // 2026.07.10

        Assert.Equal(0, sutB[0].Weight); // 2026.07.01
        Assert.Equal(0, sutB[1].Weight); // 2026.07.02
        Assert.Equal(0, sutB[2].Weight); // 2026.07.03
        Assert.Equal(0, sutB[3].Weight); // 2026.07.04
        Assert.Equal(1, sutB[4].Weight); // 2026.07.05
        Assert.Equal(1, sutB[5].Weight); // 2026.07.06
        Assert.Equal(1, sutB[6].Weight); // 2026.07.07
        Assert.Equal(1, sutB[7].Weight); // 2026.07.08
        Assert.Equal(1, sutB[8].Weight); // 2026.07.09
        Assert.Equal(1, sutB[9].Weight); // 2026.07.10

        Assert.Equal(0, sutC[0].Weight); // 2026.07.01
        Assert.Equal(-1, sutC[1].Weight); // 2026.07.02
        Assert.Equal(-1, sutC[2].Weight); // 2026.07.03
        Assert.Equal(-1, sutC[3].Weight); // 2026.07.04
        Assert.Equal(-2, sutC[4].Weight); // 2026.07.05
        Assert.Equal(-2, sutC[5].Weight); // 2026.07.06
        Assert.Equal(-2, sutC[6].Weight); // 2026.07.07
        Assert.Equal(-1, sutC[7].Weight); // 2026.07.08
        Assert.Equal(-1, sutC[8].Weight); // 2026.07.09
        Assert.Equal(-1, sutC[9].Weight); // 2026.07.10
    }
}