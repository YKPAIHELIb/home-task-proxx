using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Interfaces;
using Outreach.HomeTask.Proxx.Domain.Models;

namespace Outreach.HomeTask.Proxx.Domain.Services;

public class ProxxGame : IProxxGame
{
    private readonly ProxxBoard _board;

    public int Height { get; }
    public int Width { get; }
    public int NumberOfBlackHoles { get; }
    public ClickOnFieldResultActionEnum? GameFinishedWithAction { get; private set; }
    public ProxxCell[][] Board => _board.Board;

    public ProxxGame(int height, int width, int numberOfBlackHoles)
    {
        Height = height;
        Width = width;
        NumberOfBlackHoles = numberOfBlackHoles;
        _board = new(height, width, numberOfBlackHoles);
    }

    public ClickOnFieldResultActionEnum ClickOnCell(int i, int j)
    {
        if (i < 0 || i >= Height)
        {
            throw new ProxxInvalidInputException($"Height index was out of range of [0:{Height})");
        }
        if (j < 0 || j >= Width)
        {
            throw new ProxxInvalidInputException($"Width index was out of range of [0:{Width})");
        }

        if (GameFinishedWithAction != null)
        {
            return GameFinishedWithAction.Value;
        }

        ClickOnFieldResultActionEnum resultAction = _board.Board[i][j].Reveal() switch
        {
            RevealResultEnum.AlreadyRevealed => ClickOnFieldResultActionEnum.NothingToDo,
            RevealResultEnum.ContainsBlackHole => ClickOnFieldResultActionEnum.GameOver,
            RevealResultEnum.EmptyCell => CheckWin() ? ClickOnFieldResultActionEnum.GameWin : ClickOnFieldResultActionEnum.RedrawBoard,
            RevealResultEnum.ContainsNumber => CheckWin() ? ClickOnFieldResultActionEnum.GameWin : ClickOnFieldResultActionEnum.RedrawCell,
            _ => throw new Exception("Unreachable code")
        };

        if (resultAction is ClickOnFieldResultActionEnum.GameOver or ClickOnFieldResultActionEnum.GameWin)
        {
            GameFinishedWithAction = resultAction;
        }

        return resultAction;
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
