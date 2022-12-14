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

    public ClickOnFieldResultActionEnum ClickOnCell(int i, int j)
    {
        if (i < 0 || i >= SideLength || j < 0 || j >= SideLength)
        {
            throw new ProxxInvalidInputException($"Index was out of range of [0:{SideLength})");
        }

        return _board.Board[i][j].Reveal() switch
        {
            RevealResultEnum.AlreadyRevealed => ClickOnFieldResultActionEnum.NothingToDo,
            RevealResultEnum.ContainsBlackHole => ClickOnFieldResultActionEnum.GameOver,
            RevealResultEnum.EmptyCell => CheckWin() ? ClickOnFieldResultActionEnum.GameWin : ClickOnFieldResultActionEnum.RedrawBoard,
            RevealResultEnum.ContainsNumber => CheckWin() ? ClickOnFieldResultActionEnum.GameWin : ClickOnFieldResultActionEnum.RedrawCell,
            _ => throw new Exception("Unreachable code")
        };
    }

    private bool CheckWin()
    {
        // if all cells except black holea are revealed then player is win
        return _board.Board
            .SelectMany(x => x)
            .Where(x => !x.IsBlackHole)
            .All(x => x.IsRevealed);
    }
}
