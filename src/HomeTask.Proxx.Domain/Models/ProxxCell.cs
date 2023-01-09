namespace HomeTask.Proxx.Domain.Models;

public class ProxxCell
{
    public bool IsBlackHole { get; }
    public int? AdjacentBlackHolesNumber { get; protected set; }
    public bool IsRevealed { get; protected set; }

    internal ProxxCell(bool isBlackHole)
    {
        IsBlackHole = isBlackHole;
    }
}
