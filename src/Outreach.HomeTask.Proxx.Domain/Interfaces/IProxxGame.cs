using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Models;

namespace Outreach.HomeTask.Proxx.Domain.Interfaces;

public interface IProxxGame
{
    int Height { get; }
    int Width { get; }
    int NumberOfBlackHoles { get; }
    ClickOnFieldResultActionEnum? GameFinishedWithAction { get; }
    ProxxCell[][] Board { get; }

    ClickOnFieldResultActionEnum ClickOnCell(int i, int j);
}
