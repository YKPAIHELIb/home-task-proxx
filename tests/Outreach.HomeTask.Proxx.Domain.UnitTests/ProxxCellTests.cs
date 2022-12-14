using FluentAssertions;
using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Models;
using Xunit;

namespace Outreach.HomeTask.Proxx.Domain.UnitTests;

public class ProxxCellTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Given_IsBlackHoleValue_When_CreateProxxCell_Then_CellWithoutAdjacentBlackHolesNumberCreated(bool isBlackHole)
    {
        //Act
        ProxxCellWithLogic cell = new(isBlackHole);

        //Assert
        cell.IsBlackHole.Should().Be(isBlackHole);
        cell.IsRevealed.Should().BeFalse();
        cell.AdjacentBlackHolesNumber.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(100)]
    public void Given_IncorrectNumberOfAdjacentCells_When_Initialize_Then_ExceptionIsThrown(int numberOfAdjacentCells)
    {
        //Arrange
        List<ProxxCellWithLogic> adjacentCells = CreateAdjacentCells(numberOfAdjacentCells, numberOfAdjacentCells);
        ProxxCellWithLogic cell = new(false);

        //Act
        ProxxInvalidInputException exception = Assert.Throws<ProxxInvalidInputException>(() =>
            cell.Initialize(adjacentCells));

        //Assert
        exception.Message.Should().Be("Number of adjacent cell cannot be bigger than 8 or less than 3");
    }

    [Fact]
    public void Given_BlackHoleCell_And_AdjacentCells_When_Initialize_Then_AdjacentBlackHolesNumberRemainsNull()
    {
        //Arrange
        List<ProxxCellWithLogic> adjacentCells = CreateAdjacentCells(8);
        ProxxCellWithLogic cell = new(true);

        //Act
        cell.Initialize(adjacentCells);

        //Assert
        cell.AdjacentBlackHolesNumber.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(AllCorrectNumbersOfAdjacentBlackHoles))]
    public void Given_NotBlackHoleCell_And_AdjacentCells_When_Initialize_Then_AdjacentBlackHolesNumberIsAssigned(int numberOfAdjacentBlackHoles)
    {
        //Arrange
        List<ProxxCellWithLogic> adjacentCells = CreateAdjacentCells(numberOfAdjacentBlackHoles);
        ProxxCellWithLogic cell = new(false);

        //Act
        cell.Initialize(adjacentCells);

        //Assert
        cell.AdjacentBlackHolesNumber.Should().Be(numberOfAdjacentBlackHoles);
    }

    [Fact]
    public void Given_NotInitializedCell_When_Reveal_Then_ExceptionIsThrown()
    {
        //Arrange
        ProxxCellWithLogic cell = new(true);

        //Act
        ProxxCellNotInitializedException exception = Assert.Throws<ProxxCellNotInitializedException>(() =>
            cell.Reveal());

        //Assert
        exception.Message.Should().Be("Before calling any method call Initialize method first");
    }

    [Fact]
    public void Given_AlreadyRevealedCell_When_Reveal_Then_AlreadyRevealedIsReturned()
    {
        //Arrange
        ProxxCellWithLogic cell = new(false);
        cell.Initialize(CreateAdjacentCells(4));
        cell.Reveal();

        //Act
        RevealResultEnum revealResult = cell.Reveal();

        //Assert
        revealResult.Should().Be(RevealResultEnum.AlreadyRevealed);
        cell.IsRevealed.Should().BeTrue();
    }

    [Fact]
    public void Given_BlackHoleCell_When_Reveal_Then_ContainsBlackHoleIsReturned()
    {
        //Arrange
        ProxxCellWithLogic cell = new(true);
        cell.Initialize(CreateAdjacentCells(4));

        //Act
        RevealResultEnum revealResult = cell.Reveal();

        //Assert
        revealResult.Should().Be(RevealResultEnum.ContainsBlackHole);
        cell.IsRevealed.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(AllCorrectNumbersOfExistingAdjacentBlackHoles))]
    public void Given_CellWithAdjacentBlackHoles_When_Reveal_Then_ContainsNumberIsReturned(int numberOfAdjacentBlackHoles)
    {
        //Arrange
        ProxxCellWithLogic cell = new(false);
        cell.Initialize(CreateAdjacentCells(numberOfAdjacentBlackHoles));

        //Act
        RevealResultEnum revealResult = cell.Reveal();

        //Assert
        revealResult.Should().Be(RevealResultEnum.ContainsNumber);
        cell.IsRevealed.Should().BeTrue();
    }

    [Fact]
    public void Given_CellWithoutAdjacentBlackHoles_And_UninitializedAdjacentCells_When_Reveal_Then_ExceptionIsThrown()
    {
        //Arrange
        ProxxCellWithLogic cell = new(false);
        cell.Initialize(CreateAdjacentCells(0));

        //Act
        ProxxCellNotInitializedException exception = Assert.Throws<ProxxCellNotInitializedException>(() =>
            cell.Reveal());

        //Assert
        exception.Message.Should().Be("Before calling any method call Initialize method first");
    }

    [Fact]
    public void Given_CellWithoutAdjacentBlackHoles_When_Reveal_Then_EmptyCellIsReturned_And_AllAdjacentCellsAreRevealed()
    {
        //Arrange
        ProxxCellWithLogic cell = new(false);
        List<ProxxCellWithLogic> adjacentCells = CreateAdjacentCells(0);
        cell.Initialize(adjacentCells);
        foreach (ProxxCellWithLogic adjacentCell in adjacentCells)
        {
            adjacentCell.Initialize(CreateAdjacentCells(2)); // we should initialize to be able to reveal
        }

        //Act
        RevealResultEnum revealedResult = cell.Reveal();

        //Assert
        revealedResult.Should().Be(RevealResultEnum.EmptyCell);
        cell.IsRevealed.Should().BeTrue();

        adjacentCells.Should().AllBeEquivalentTo(new { IsRevealed = true });
    }

    [Fact]
    public void Given_CellWithoutAdjacentBlackHoles_And_AdjacentCellWithoutAdjacentBlackHoles_When_Reveal_Then_CellsAreRevealedRecursively()
    {
        //Arrange
        ProxxCellWithLogic cell = new(false);
        List<ProxxCellWithLogic> adjacentCells = CreateAdjacentCells(0);
        cell.Initialize(adjacentCells);

        List<ProxxCellWithLogic> allFurtherAdjacentCells = new();
        foreach (ProxxCellWithLogic adjacentCell in adjacentCells)
        {
            List<ProxxCellWithLogic> furtherAdjacentCells = CreateAdjacentCells(0);
            adjacentCell.Initialize(furtherAdjacentCells); // we should initialize to be able to reveal
            foreach (ProxxCellWithLogic furtherAdjacentCell in furtherAdjacentCells)
            {
                furtherAdjacentCell.Initialize(CreateAdjacentCells(1)); // end of recursion as it's not "empty" cell
            }

            allFurtherAdjacentCells.AddRange(furtherAdjacentCells);
        }

        //Act
        RevealResultEnum revealedResult = cell.Reveal();

        //Assert
        revealedResult.Should().Be(RevealResultEnum.EmptyCell);
        cell.IsRevealed.Should().BeTrue();

        adjacentCells.Should().AllBeEquivalentTo(new { IsRevealed = true });
        allFurtherAdjacentCells.Should().AllBeEquivalentTo(new { IsRevealed = true });
    }

    private static List<ProxxCellWithLogic> CreateAdjacentCells(int blackHolesCount, int allCellsCount = 8)
    {
        return Enumerable.Range(0, allCellsCount).Select(i => new ProxxCellWithLogic(i < blackHolesCount)).ToList();
    }

    private static IEnumerable<object[]> AllCorrectNumbersOfAdjacentBlackHoles() => new[]
    {
        new object[] { 0 },
        new object[] { 1 },
        new object[] { 2 },
        new object[] { 3 },
        new object[] { 4 },
        new object[] { 5 },
        new object[] { 6 },
        new object[] { 7 },
        new object[] { 8 }
    };

    private static IEnumerable<object[]> AllCorrectNumbersOfExistingAdjacentBlackHoles() => new[]
    {
        new object[] { 1 },
        new object[] { 2 },
        new object[] { 3 },
        new object[] { 4 },
        new object[] { 5 },
        new object[] { 6 },
        new object[] { 7 },
        new object[] { 8 }
    };
}
