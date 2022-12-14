using System.Text.RegularExpressions;
using Outreach.HomeTask.Proxx.Domain.Enums;
using Outreach.HomeTask.Proxx.Domain.Interfaces;
using Outreach.HomeTask.Proxx.Domain.Models;

namespace Outreach.HomeTask.Proxx.ConsoleApp;

public class PlayGameInConsoleService
{
    private readonly IProxxGameFactory _proxxGameFactory;

    public PlayGameInConsoleService(IProxxGameFactory proxxGameFactory)
    {
        _proxxGameFactory = proxxGameFactory;
    }

    public void PlayGame()
    {
        Console.WriteLine("""
            -------------------------------------------
            --- Proxx game background logic example ---
            -------------------------------------------
            
            Done by Mykola Melnychuk, senior .NET developer (mmelnichyk@gmail.com)
            Feel free to look at code and ask any questions.

            Rules of game described here: https://proxx.app/

            I tried to add Dependency Injections, unit tests, split to different Domain models / services, etc.

            The project is able to:
                - building the board of specified size with randomly placed specified amount of black holes
                - provide current state of game board via interface IProxxGame
                - reveal specifig cell on board and keep revealed state
                - recursievly reveal adjacent cells if hit on empty cell (with no adjancent black holes)
                - return the hint of possible actions on each click on cell

            What can be improved:
                - win handling
                - generate the field after first click so user never hits the black hole on game start
                - rectangular board (different width and height)

            -------------------------------------------

            To start new game type "new"
            To click on cell type "cell <i> <j>"
            To exit type "exit"
            """);

        IProxxGame? proxxGame = null;

        while (true)
        {
            string? userInput = Console.ReadLine()?.Trim()?.ToLower();
            if (userInput == "new")
            {
                proxxGame = CreateNewGame();
            }
            else if (userInput?.StartsWith("cell") == true)
            {
                ClickOnCell(userInput, proxxGame);
            }
            else if (userInput == "exit")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Try again.");
            }
        }

        Console.WriteLine("""


            Thank you for your time.
            Good bye!
            """);
    }

    private IProxxGame CreateNewGame()
    {
        Console.Clear();
        Console.WriteLine("New game. Enter board side length and number of black holes (space separated):");
        Console.WriteLine("Example: 10 10");

        while (true)
        {
            try
            {
                string? userInput = Console.ReadLine();
                while (string.IsNullOrEmpty(userInput) || !Regex.IsMatch(userInput, @"^\d+ +\d+$"))
                {
                    Console.WriteLine("Invalid input. Try again.");
                    userInput = Console.ReadLine();
                }

                string[] strValues = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int sideLength = int.Parse(strValues[0]);
                int numberOfBlackHoles = int.Parse(strValues[1]);

                IProxxGame proxxGame = _proxxGameFactory.CreateProxxGame(sideLength, numberOfBlackHoles);

                Console.WriteLine("Here is the board created for the game: ");
                Console.WriteLine();

                DrawField(proxxGame);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("""
                    To start new game type "new"
                    To click on cell type "cell <i> <j>"
                    To exit type "exit"
                    """);
                Console.WriteLine();

                return proxxGame;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try again.");
            }
        }
    }

    private static void DrawField(IProxxGame proxxGame)
    {
        Console.WriteLine(new string('#', proxxGame.Board[0].Length + 2));
        for (int i = 0; i < proxxGame.Board.Length; i++)
        {
            Console.Write('#');
            for (int j = 0; j < proxxGame.Board[0].Length; j++)
            {
                char toWrite = proxxGame.Board[i][j] switch
                {
                    { IsBlackHole: true } => 'X',
                    { AdjacentBlackHolesNumber: 0 } => ' ',
                    ProxxCell cell => cell.AdjacentBlackHolesNumber!.Value.ToString()[0]
                };
                Console.Write(toWrite);
            }
            Console.WriteLine('#');
        }
        Console.WriteLine(new string('#', proxxGame.Board[0].Length + 2));
    }

    private static void ClickOnCell(string userInput, IProxxGame? proxxGame)
    {
        if (string.IsNullOrEmpty(userInput) || !Regex.IsMatch(userInput, @"^cell +\d+ +\d+$"))
        {
            Console.WriteLine("Invalid input. Try again.");
            return;
        }

        if (proxxGame == null)
        {
            Console.WriteLine("Please start new game first");
            return;
        }

        string[] strValues = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int i = int.Parse(strValues[1]);
        int j = int.Parse(strValues[2]);

        try
        {
            ClickOnFieldResultActionEnum clickResultAction = proxxGame.ClickOnCell(i, j);
            string message = clickResultAction switch
            {
                ClickOnFieldResultActionEnum.NothingToDo => "Cell is already revealed. Nothing to do.",
                ClickOnFieldResultActionEnum.RedrawCell => "Cell became revealed. Redraw of cell might be needed.",
                ClickOnFieldResultActionEnum.RedrawBoard => "Cell became revealed along with adjacent cells. Redraw of the board might be needed.",
                ClickOnFieldResultActionEnum.GameOver => "Game over. Displaying the message to gamer might be needed.",
                ClickOnFieldResultActionEnum.GameWin => "Not implemented for now.",
                _ => throw new Exception("Unreachable code")
            };

            Console.WriteLine(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
