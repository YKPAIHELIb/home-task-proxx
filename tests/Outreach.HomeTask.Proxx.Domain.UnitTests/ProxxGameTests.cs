using FluentAssertions;
using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Models;
using Outreach.HomeTask.Proxx.Domain.Services;
using Xunit;

namespace Outreach.HomeTask.Proxx.Domain.UnitTests;

public class ProxxGameTests
{
    [Fact]
    public void Given_IncorrectParameters_When_CreateProxxGame_Then_ExceptionIsThrown()
    {
        //Act
        Exception exception = Assert.ThrowsAny<Exception>(() => new ProxxGame(2, 2, 100));

        //Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Given_CorrectParameters_When_CreateProxxGame_Then_GameIsCreatedAndPropertiesAreAvailable()
    {
        //Arrange
        const int height = 10;
        const int width = 15;
        const int numbersOfBlackHoles = 20;

        //Act
        ProxxGame game = new ProxxGame(height, width, numbersOfBlackHoles);

        //Assert
        game.Height.Should().Be(height);
        game.Width.Should().Be(width);
        game.NumberOfBlackHoles.Should().Be(numbersOfBlackHoles);

        game.Board.Length.Should().Be(height);
        game.Board[0].Length.Should().Be(width);
    }

    [Theory]
    [InlineData(15, 4, "Height index was out of range of [0:10)")]
    [InlineData(4, 15, "Width index was out of range of [0:10)")]
    public void Given_ProxxGame_When_ClickOnFieldWithInvalidParameters_Then_ExceptionIsThrown(int i, int j, string expectedExceptionMessage)
    {
        //Arrange
        ProxxGame game = new(10, 10, 10);

        //Act
        ProxxInvalidInputException exception = Assert.Throws<ProxxInvalidInputException>(() =>
            game.ClickOnCell(i, j));

        //Assert
        exception.Message.Should().Be(expectedExceptionMessage);
    }

    [Fact]
    public void Given_ProxxGame_When_ClickOnAlreadyRevealedNotEndGame_Then_NothingToDoIsReturned()
    {
        //Arrange
        const int sideLength = 10;
        const int numbersOfBlackHoles = 20;
        ProxxGame game = new ProxxGame(sideLength, sideLength, numbersOfBlackHoles);

        // cell with number so we definitely won't end the game on first move
        (_, int i, int j) = game.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber > 0);
        game.ClickOnCell(i, j);

        //Act
        ClickOnFieldResultActionEnum result = game.ClickOnCell(i, j);

        //Assert
        result.Should().Be(ClickOnFieldResultActionEnum.NothingToDo);
    }

    [Fact]
    public void Given_ProxxGame_When_ClickOnEmptyCell_Then_RedrawBoardIsReturned()
    {
        //Arrange
        const int sideLength = 10;
        const int numbersOfBlackHoles = 10;
        ProxxGame game = new ProxxGame(sideLength, sideLength, numbersOfBlackHoles);

        (_, int i, int j) = game.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber == 0);

        //Act
        ClickOnFieldResultActionEnum result = game.ClickOnCell(i, j);

        //Assert
        result.Should().Be(ClickOnFieldResultActionEnum.RedrawBoard);
    }

    [Fact]
    public void Given_ProxxGame_When_ClickOnCellWithNumber_Then_RedrawCellIsReturned()
    {
        //Arrange
        const int sideLength = 10;
        const int numbersOfBlackHoles = 10;
        ProxxGame game = new ProxxGame(sideLength, sideLength, numbersOfBlackHoles);

        (_, int i, int j) = game.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber > 0);

        //Act
        ClickOnFieldResultActionEnum result = game.ClickOnCell(i, j);

        //Assert
        result.Should().Be(ClickOnFieldResultActionEnum.RedrawCell);
    }

    [Fact]
    public void Given_ProxxGame_When_ClickOnCellWithBlackHole_Then_GameOverIsReturned()
    {
        //Arrange
        const int sideLength = 10;
        const int numbersOfBlackHoles = 10;
        ProxxGame game = new ProxxGame(sideLength, sideLength, numbersOfBlackHoles);

        (_, int i, int j) = game.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.IsBlackHole);

        //Act
        ClickOnFieldResultActionEnum result = game.ClickOnCell(i, j);

        //Assert
        result.Should().Be(ClickOnFieldResultActionEnum.GameOver);
    }

    [Fact]
    public void Given_ProxxGameAlmostFinished_When_ClickOnLastCell_Then_GameWinIsReturned()
    {
        //Arrange
        const int sideLength = 5;
        const int numbersOfBlackHoles = 1;
        ProxxGame game = new ProxxGame(sideLength, sideLength, numbersOfBlackHoles);

        // reveal all numbers
        foreach ((ProxxCell cellWithNumber, int cellI, int cellJ) in game.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).Where(x => x.cell.AdjacentBlackHolesNumber > 0))
        {
            game.ClickOnCell(cellI, cellJ);
        }

        (ProxxCell emptyCell, int i, int j) = game.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.AdjacentBlackHolesNumber == 0);

        //Act
        ClickOnFieldResultActionEnum result = game.ClickOnCell(i, j);

        //Assert
        result.Should().Be(ClickOnFieldResultActionEnum.GameWin, "when click on empty cell while all number cells are revealed game should be finished");
    }
}
