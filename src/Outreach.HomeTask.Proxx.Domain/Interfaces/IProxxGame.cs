using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Models;

namespace Outreach.HomeTask.Proxx.Domain.Interfaces;

public interface IProxxGame
{
    ProxxCell[][] Board { get; }

    ClickOnFieldResultActionEnum ClickOnField(int i, int j);
}
