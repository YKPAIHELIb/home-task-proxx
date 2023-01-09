using HomeTask.Proxx.Domain.Interfaces;

namespace HomeTask.Proxx.Domain.Services;

public class ProxxGameFactory : IProxxGameFactory
{
    public IProxxGame CreateProxxGame(int height, int width, int numberOfBlackHoles)
    {
        return new ProxxGame(height, width, numberOfBlackHoles);
    }
}
