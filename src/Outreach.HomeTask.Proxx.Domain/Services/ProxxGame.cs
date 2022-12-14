using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Interfaces;
using Outreach.HomeTask.Proxx.Domain.Models;

namespace Outreach.HomeTask.Proxx.Domain.Services;

public class ProxxGame : IProxxGame
{
    private readonly ProxxBoard _board;

    public int SideLength { get; }
    public int NumberOfBlackHoles { get; }
    public ProxxCell[][] Board => _board.Board;

    public ProxxGame(int sideLength, int numberOfBlackHoles)
    {
        SideLength = sideLength;
        NumberOfBlackHoles = numberOfBlackHoles;
        _board = new(sideLength, numberOfBlackHoles);
    }

    public ClickOnFieldResultActionEnum ClickOnField(int i, int j)
    {
        if (i < 0 || i >= SideLength || j < 0 || j >= SideLength)
        {
            throw new ProxxInvalidInputException($"Index was out of range of [0:{SideLength})");
        }

        return _board.Board[i][j].Reveal() switch
        {
            RevealResultEnum.AlreadyRevealed => ClickOnFieldResultActionEnum.NothingToDo,
            RevealResultEnum.EmptyCell => ClickOnFieldResultActionEnum.RedrawBoard,
            RevealResultEnum.ContainsNumber => ClickOnFieldResultActionEnum.RedrawCell,
            RevealResultEnum.ContainsBlackHole => ClickOnFieldResultActionEnum.GameOver,
            _ => throw new Exception("Unreachable code")

            // what's left is to write logic about GameWin response
        };
    }
}
