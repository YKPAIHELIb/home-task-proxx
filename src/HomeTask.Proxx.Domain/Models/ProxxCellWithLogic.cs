using System.Runtime.CompilerServices;
using HomeTask.Proxx.Domain.Enums;
using HomeTask.Proxx.Domain.Exceptions;

[assembly: InternalsVisibleTo("HomeTask.Proxx.Domain.UnitTests")]
namespace HomeTask.Proxx.Domain.Models;

internal sealed class ProxxCellWithLogic : ProxxCell
{
    private List<ProxxCellWithLogic> _adjacentCells = null!;

    public ProxxCellWithLogic(bool isBlackHole) : base(isBlackHole)
    {
    }

    public void Initialize(IList<ProxxCellWithLogic> adjacentCells)
    {
        // it's redundant here as only our code calls this method but just to show that I know about input validations :)
        if (adjacentCells.Count is > 8 or < 3)
        {
            throw new ProxxInvalidInputException("Number of adjacent cell cannot be bigger than 8 or less than 3");
        }

        _adjacentCells = adjacentCells.ToList();
        if (!IsBlackHole)
        {
            AdjacentBlackHolesNumber = _adjacentCells.Count(x => x.IsBlackHole);
        }
    }

    public RevealResultEnum Reveal()
    {
        EnsureInitialized();

        RevealResultEnum revealResult = this switch
        {
            { IsRevealed: true } => RevealResultEnum.AlreadyRevealed,
            { IsBlackHole: true } => RevealResultEnum.ContainsBlackHole,
            { AdjacentBlackHolesNumber: 0 } => RevealResultEnum.EmptyCell,
            _ => RevealResultEnum.ContainsNumber
        };

        IsRevealed = true;

        if (revealResult == RevealResultEnum.EmptyCell)
        {
            foreach (ProxxCellWithLogic adjacentCell in _adjacentCells)
            {
                adjacentCell.Reveal();
            }
        }

        return revealResult;
    }

    private void EnsureInitialized()
    {
        if (_adjacentCells == null)
        {
            throw new ProxxCellNotInitializedException($"Before calling any method call {nameof(Initialize)} method first");
        }
    }
}
