using HomeTask.Proxx.Domain.Enums;
using HomeTask.Proxx.Domain.Models;

namespace HomeTask.Proxx.Domain.Interfaces;

public interface IProxxGame
{
    int Height { get; }
    int Width { get; }
    int NumberOfBlackHoles { get; }
    ClickOnFieldResultActionEnum? GameFinishedWithAction { get; }
    ProxxCell[][] Board { get; }

    ClickOnFieldResultActionEnum ClickOnCell(int i, int j);
}
