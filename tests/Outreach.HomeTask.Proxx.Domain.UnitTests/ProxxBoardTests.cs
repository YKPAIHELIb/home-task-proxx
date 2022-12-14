using FluentAssertions;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Models;
using Xunit;

namespace Outreach.HomeTask.Proxx.Domain.UnitTests;

public class ProxxBoardTests
{
    [Theory]
    //testing side length validation
    [InlineData(-1, -1, 5, "Height is out of allowed ranges [5, 40]")]
    [InlineData(0, 0, 5, "Height is out of allowed ranges [5, 40]")]
    [InlineData(2, 2, 5, "Height is out of allowed ranges [5, 40]")]
    [InlineData(4, 4, 5, "Height is out of allowed ranges [5, 40]")]
    [InlineData(41, 41, 5, "Height is out of allowed ranges [5, 40]")]
    [InlineData(50, 50, 5, "Height is out of allowed ranges [5, 40]")]
    [InlineData(100, 100, 5, "Height is out of allowed ranges [5, 40]")]
    [InlineData(5, -1, 5, "Width is out of allowed ranges [5, 40]")]
    [InlineData(5, 0, 5, "Width is out of allowed ranges [5, 40]")]
    [InlineData(5, 2, 5, "Width is out of allowed ranges [5, 40]")]
    [InlineData(5, 4, 5, "Width is out of allowed ranges [5, 40]")]
    [InlineData(5, 41, 5, "Width is out of allowed ranges [5, 40]")]
    [InlineData(5, 50, 5, "Width is out of allowed ranges [5, 40]")]
    [InlineData(5, 100, 5, "Width is out of allowed ranges [5, 40]")]
    //tesing number of black holes validation
    [InlineData(5, 6, 0, "Number of black holes cannot be 0")]
    [InlineData(5, 6, 22, "Number of black holes cannot be bigger than 21 for 5x6 field")] // too high as the rule says 5*6-9 = 21 is upper limit
    [InlineData(5, 6, 300, "Number of black holes cannot be bigger than 21 for 5x6 field")]
    [InlineData(30, 30, 0, "Number of black holes cannot be 0")]
    [InlineData(30, 30, 892, "Number of black holes cannot be bigger than 891 for 30x30 field")] // too high as the rule says 30*30-9 = 891 is upper limit
    [InlineData(30, 30, 1000, "Number of black holes cannot be bigger than 891 for 30x30 field")]
    public void GivenWrongSideLenghtOrNumberOfBlackHoles_When_CreateBoard_Then_ExceptionIsThrown(int height, int width, int numberOfBlackHoles, string expectedErrorMessage)
    {
        //Act
        ProxxInvalidInputException exception = Assert.Throws<ProxxInvalidInputException>(() =>
            new ProxxBoard(height, width, numberOfBlackHoles));

        //Assert
        exception.Message.Should().Be(expectedErrorMessage);
    }

    [Theory]
    [InlineData(5, 5, 5)]
    [InlineData(5, 6, 21)]
    [InlineData(10, 10, 1)]
    [InlineData(10, 10, 10)]
    [InlineData(10, 10, 88)]
    [InlineData(10, 10, 91)]
    [InlineData(40, 40, 1)]
    [InlineData(40, 40, 25)]
    [InlineData(40, 40, 1000)]
    [InlineData(40, 40, 1590)]
    [InlineData(40, 40, 1591)]
    public void GivenCorrectSideLenghtAndNumberOfBlackHoles_When_CreateBoard_Then_NoError(int height, int width, int numberOfBlackHoles)
    {
        //Act
        ProxxBoard board = new ProxxBoard(height, width, numberOfBlackHoles);

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
        ProxxBoard board = new ProxxBoard(sideLength, sideLength, numberOfBlackHoles);

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
        ProxxBoard board = new ProxxBoard(sideLength, sideLength, numberOfBlackHoles);

        //Assert
        (ProxxCellWithLogic cellWithAdjacentBlackHoles, int cellI, int cellJ) = board.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber > 0);
        List<ProxxCellWithLogic> adjencentCells = GetAdjacentCellsForPosition(board.Board, cellI, cellJ);
        adjencentCells.Count(x => x.IsBlackHole).Should().Be(cellWithAdjacentBlackHoles.AdjacentBlackHolesNumber);

        (ProxxCellWithLogic cellWithoutAdjacentBlackHoles, cellI, cellJ) = board.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber == 0);
        adjencentCells = GetAdjacentCellsForPosition(board.Board, cellI, cellJ);
        adjencentCells.Count(x => x.IsBlackHole).Should().Be(0);
    }

    private static List<ProxxCellWithLogic> GetAdjacentCellsForPosition(ProxxCellWithLogic[][] board, int centerI, int centerJ)
    {
        List<ProxxCellWithLogic> adjacentCells = new();
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
