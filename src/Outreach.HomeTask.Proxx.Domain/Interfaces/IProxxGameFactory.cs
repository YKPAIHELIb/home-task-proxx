namespace Outreach.HomeTask.Proxx.Domain.Interfaces;

public interface IProxxGameFactory
{
    IProxxGame CreateProxxGame(int sideLength, int numberOfBlackHoles);
}
