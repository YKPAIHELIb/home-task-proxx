namespace HomeTask.Proxx.Domain.Interfaces;

public interface IProxxGameFactory
{
    IProxxGame CreateProxxGame(int height, int width, int numberOfBlackHoles);
}
