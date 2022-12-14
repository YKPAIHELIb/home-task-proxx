using System.Runtime.CompilerServices;
using Outreach.HomeTask.Proxx.Domain.Exceptions;
using Outreach.HomeTask.Proxx.Domain.Helpers;

[assembly: InternalsVisibleTo("Outreach.HomeTask.Proxx.Domain.UnitTests")]
namespace Outreach.HomeTask.Proxx.Domain.Models;

internal class ProxxBoard
{
    // nubers taken from validation of inputs in https://proxx.app/
    private const int ConstMinSideLenght = 5;
    private const int ConstMaxSideLenght = 40;

    public ProxxCellWithLogic[][] Board { get; set; }
    public int NumberOfBlackHoles { get; }

    // Ther requirements said that width and lenght should be the same (NxN board)
    // For the real game I'd prefer to separate width and lenght
    public ProxxBoard(int height, int width, int numberOfBlackHoles)
    {
        if (height is < ConstMinSideLenght or > ConstMaxSideLenght)
        {
            throw new ProxxInvalidInputException($"Height is out of allowed ranges [{ConstMinSideLenght}, {ConstMaxSideLenght}]");
        }
        if (width is < ConstMinSideLenght or > ConstMaxSideLenght)
        {
            throw new ProxxInvalidInputException($"Width is out of allowed ranges [{ConstMinSideLenght}, {ConstMaxSideLenght}]");
        }
        if (numberOfBlackHoles < 1)
        {
            throw new ProxxInvalidInputException("Number of black holes cannot be 0");
        }

        // This valiidation rule comes from https://proxx.app/
        int maxAllowedCountOfBlackHoles = height * width - 9;
        if (numberOfBlackHoles > maxAllowedCountOfBlackHoles)
        {
            throw new ProxxInvalidInputException($"Number of black holes cannot be bigger than {maxAllowedCountOfBlackHoles} for {height}x{width} field");
        }

        NumberOfBlackHoles = numberOfBlackHoles;
        Board = new ProxxCellWithLogic[height][];
        for (int i = 0; i < Board.Length; i++)
        {
            Board[i] = new ProxxCellWithLogic[width];
        }

        PopulateBoardWithCellsAndRandomBlackHoles(numberOfBlackHoles);
        CallInitializeCellsWithAdjacentCells();
    }

    private void PopulateBoardWithCellsAndRandomBlackHoles(int numberOfBlackHoles)
    {
        int lenghtForI = Board.Length;
        int lenghtForJ = Board[0].Length;

        HashSet<int> blackHolesPositions = RandomHelper.CreateRandomDistributedNumbers(lenghtForI * lenghtForJ, numberOfBlackHoles);

        for (int i = 0; i < lenghtForI; i++)
        {
            for (int j = 0; j < lenghtForJ; j++)
            {
                bool isBlackHole = blackHolesPositions.Contains(i * lenghtForJ + j);
                Board[i][j] = new ProxxCellWithLogic(isBlackHole);
            }
        }
    }

    private void CallInitializeCellsWithAdjacentCells()
    {
        int lenghtForI = Board.Length;
        int lenghtForJ = Board[0].Length;

        for (int i = 0; i < lenghtForI; i++)
        {
            for (int j = 0; j < lenghtForJ; j++)
            {
                ProxxCellWithLogic cell = Board[i][j];
                cell.Initialize(GetAdjacentCellsForPosition(i, j));
            }
        }

        // LOCAL FUNCTIONS
        List<ProxxCellWithLogic> GetAdjacentCellsForPosition(int centerI, int centerJ)
        {
            List<ProxxCellWithLogic> adjacentCells = new();
            for (int i = centerI - 1; i <= centerI + 1; i++)
            {
                for (int j = centerJ - 1; j <= centerJ + 1; j++)
                {
                    if (i == centerI && j == centerJ)
                    {
                        continue;
                    }
                    if (i < 0 || i >= lenghtForI || j < 0 || j >= lenghtForJ)
                    {
                        continue;
                    }

                    adjacentCells.Add(Board[i][j]);
                }
            }
            return adjacentCells;
        }
    }
}
