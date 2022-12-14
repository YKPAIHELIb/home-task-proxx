using FluentAssertions;
using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Services;
using Xunit;

namespace Outreach.HomeTask.Proxx.Domain.UnitTests;

public class ProxxGameTests
{
    [Fact]
    public void Given_IncorrectParameters_When_CreateProxxGame_Then_ExceptionIsThrown()
    {
        //Act
        Exception exception = Assert.ThrowsAny<Exception>(() => new ProxxGame(2, 100));

        //Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void Given_CorrectParameters_When_CreateProxxGame_Then_GameIsCreatedAndPropertiesAreAvailable()
    {
        //Arrange
        const int sideLength = 10;
        const int numbersOfBlackHoles = 20;

        //Act
        ProxxGame game = new ProxxGame(sideLength, numbersOfBlackHoles);

        //Assert
        game.SideLength.Should().Be(sideLength);
        game.NumberOfBlackHoles.Should().Be(numbersOfBlackHoles);

        game.Board.Length.Should().Be(sideLength);
        game.Board[0].Length.Should().Be(sideLength);
    }

    [Fact]
    public void Given_ProxxGame_When_ClickOnFieldWithInvalidParameters_Then_ExceptionIsThrown()
    {
        //Arrange
        ProxxGame game = new(10, 10);

        //Act
        ProxxInvalidInputException exception = Assert.Throws<ProxxInvalidInputException>(() =>
            game.ClickOnCell(15, 4));

        //Assert
        exception.Message.Should().Be("Index was out of range of [0:10)");
    }

    [Fact]
    public void Given_ProxxGame_When_ClickOnAlreadyRevealed_Then_NothingToDoIsReturned()
    {
        //Arrange
        const int sideLength = 10;
        const int numbersOfBlackHoles = 20;
        ProxxGame game = new ProxxGame(sideLength, numbersOfBlackHoles);

        game.ClickOnCell(1, 1);

        //Act
        ClickOnFieldResultActionEnum result = game.ClickOnCell(1, 1);

        //Assert
        result.Should().Be(ClickOnFieldResultActionEnum.NothingToDo);
    }

    [Fact]
    public void Given_ProxxGame_When_ClickOnEmptyCell_Then_RedrawBoardIsReturned()
    {
        //Arrange
        const int sideLength = 10;
        const int numbersOfBlackHoles = 10;
        ProxxGame game = new ProxxGame(sideLength, numbersOfBlackHoles);

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
        ProxxGame game = new ProxxGame(sideLength, numbersOfBlackHoles);

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
        ProxxGame game = new ProxxGame(sideLength, numbersOfBlackHoles);

        (_, int i, int j) = game.Board.SelectMany((row, i) => row.Select((cell, j) => (cell, i, j))).First(x => x.cell.IsBlackHole);

        //Act
        ClickOnFieldResultActionEnum result = game.ClickOnCell(i, j);

        //Assert
        result.Should().Be(ClickOnFieldResultActionEnum.GameOver);
    }
}
