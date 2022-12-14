using Outreach.HomeTask.Proxx.Domain.Interfaces;

namespace Outreach.HomeTask.Proxx.Domain.Services;

public class ProxxGameFactory : IProxxGameFactory
{
    public IProxxGame CreateProxxGame(int sideLength, int numberOfBlackHoles)
    {
        return new ProxxGame(sideLength, numberOfBlackHoles);
    }
}
