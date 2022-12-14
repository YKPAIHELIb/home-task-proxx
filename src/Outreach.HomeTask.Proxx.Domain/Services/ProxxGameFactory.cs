using Outreach.HomeTask.Proxx.Domain.Interfaces;

namespace Outreach.HomeTask.Proxx.Domain.Services;

public class ProxxGameFactory : IProxxGameFactory
{
    public IProxxGame CreateProxxGame(int height, int width, int numberOfBlackHoles)
    {
        return new ProxxGame(height, width, numberOfBlackHoles);
    }
}
