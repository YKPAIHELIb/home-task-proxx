using FluentAssertions;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Models;
using Xunit;

namespace Outreach.HomeTask.Proxx.Domain.UnitTests;

public class ProxxBoardTests
{
    [Theory]
    //testing side length validation
    [InlineData(-1, 5, "Side lenth is out of allowed ranges [5, 40]")]
    [InlineData(0, 5, "Side lenth is out of allowed ranges [5, 40]")]
    [InlineData(2, 5, "Side lenth is out of allowed ranges [5, 40]")]
    [InlineData(4, 5, "Side lenth is out of allowed ranges [5, 40]")]
    [InlineData(41, 5, "Side lenth is out of allowed ranges [5, 40]")]
    [InlineData(50, 5, "Side lenth is out of allowed ranges [5, 40]")]
    [InlineData(100, 5, "Side lenth is out of allowed ranges [5, 40]")]
    //tesing number of black holes validation
    [InlineData(5, 0, "Number of black holes cannot be 0")]
    [InlineData(5, 17, "Number of black holes cannot be bigger than 16 for 5x5 field")] // too high as the rule says 5*5-9 = 16 is upper limit
    [InlineData(5, 300, "Number of black holes cannot be bigger than 16 for 5x5 field")]
    [InlineData(30, 0, "Number of black holes cannot be 0")]
    [InlineData(30, 892, "Number of black holes cannot be bigger than 891 for 30x30 field")] // too high as the rule says 30*30-9 = 892 is upper limit
    [InlineData(30, 1000, "Number of black holes cannot be bigger than 891 for 30x30 field")]
    public void GivenWrongSideLenghtOrNumberOfBlackHoles_When_CreateBoard_Then_ExceptionIsThrown(int sideLength, int numberOfBlackHoles, string expectedErrorMessage)
    {
        //Act
        ProxxInvalidInputException exception = Assert.Throws<ProxxInvalidInputException>(() =>
            new ProxxBoard(sideLength, numberOfBlackHoles));

        //Assert
        exception.Message.Should().Be(expectedErrorMessage);
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(5, 16)]
    [InlineData(10, 1)]
    [InlineData(10, 10)]
    [InlineData(10, 88)]
    [InlineData(10, 91)]
    [InlineData(40, 1)]
    [InlineData(40, 25)]
    [InlineData(40, 1000)]
    [InlineData(40, 1590)]
    [InlineData(40, 1591)]
    public void GivenCorrectSideLenghtAndNumberOfBlackHoles_When_CreateBoard_Then_NoError(int sideLength, int numberOfBlackHoles)
    {
        //Act
        ProxxBoard board = new ProxxBoard(sideLength, numberOfBlackHoles);

        //Assert
        board.Should().NotBeNull();
    }

    [Fact]
    public void GivenCorrectInput_When_CreateBoard_Then_BoardIsCreated_And_BlackHolesNumberIsCorrect()
    {
        //Arrange
        int sideLength = 10;
        int numberOfBlackHoles = 30;

        //Act
        ProxxBoard board = new ProxxBoard(sideLength, numberOfBlackHoles);

        //Assert
        int actualBlackHolesCount = board.Board.SelectMany(x => x).Count(x => x.IsBlackHole);
        actualBlackHolesCount.Should().Be(numberOfBlackHoles);
    }

    [Fact]
    public void GivenCorrectInput_When_CreateBoard_Then_BoardIsCreated_And_AdjacentNumbersSetCorrectly()
    {
        //Arrange
        int sideLength = 10;
        int numberOfBlackHoles = 10;

        //Act
        ProxxBoard board = new ProxxBoard(sideLength, numberOfBlackHoles);

        //Assert
        (ProxxCell cellWithAdjacentBlackHoles, int cellI, int cellJ) = board.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber > 0);
        List<ProxxCell> adjencentCells = GetAdjacentCellsForPosition(board.Board, cellI, cellJ);
        adjencentCells.Count(x => x.IsBlackHole).Should().Be(cellWithAdjacentBlackHoles.AdjacentBlackHolesNumber);

        (ProxxCell cellWithoutAdjacentBlackHoles, cellI, cellJ) = board.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber == 0);
        adjencentCells = GetAdjacentCellsForPosition(board.Board, cellI, cellJ);
        adjencentCells.Count(x => x.IsBlackHole).Should().Be(0);
    }

    private static List<ProxxCell> GetAdjacentCellsForPosition(ProxxCell[][] board, int centerI, int centerJ)
    {
        List<ProxxCell> adjacentCells = new();
        for (int i = centerI - 1; i <= centerI + 1; i++)
        {
            for (int j = centerJ - 1; j <= centerJ + 1; j++)
            {
                if (i == centerI && j == centerJ)
                {
                    continue;
                }
                if (i < 0 || i >= board.Length || j < 0 || j >= board[0].Length)
                {
                    continue;
                }

                adjacentCells.Add(board[i][j]);
            }
        }
        return adjacentCells;
    }
}
