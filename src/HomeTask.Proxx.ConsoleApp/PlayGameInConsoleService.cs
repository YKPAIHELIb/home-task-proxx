using System.Text.RegularExpressions;
using HomeTask.Proxx.Domain.Enums;
using HomeTask.Proxx.Domain.Interfaces;
using HomeTask.Proxx.Domain.Models;

namespace HomeTask.Proxx.ConsoleApp;

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
                - return the hint of possible actions for real game on each click on cell
                - recognize and keep win/lose state of game

            The console app is able to:
                - start new game, allow to enter the input values
                - draw the board as for player (hide non-revealed cells) or as for tester (show all board as revealed)
                - "click" on cell by typing a command
                - see the result of "click"

            What can be improved:
                - generate the field after first click so user never hits the black hole on game start
            
            -------------------------------------------

            To start new game type "new"
            To draw field with all info revealed type "draw"
            To click on cell type "cell <i> <j>"
            To exit type "exit" (or press CTRL+C)
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
            else if (userInput == "draw")
            {
                if (proxxGame == null)
                {
                    Console.WriteLine("Please start new game first");
                    continue;
                }
                DrawField(proxxGame, true);
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
        Console.WriteLine("New game. Enter board height, width and number of black holes (space separated):");
        Console.WriteLine("Example: 5 10 8");

        while (true)
        {
            try
            {
                string? userInput = Console.ReadLine();
                while (string.IsNullOrEmpty(userInput) || !Regex.IsMatch(userInput, @"^\d+ +\d+ +\d+$"))
                {
                    Console.WriteLine("Invalid input. Try again.");
                    userInput = Console.ReadLine();
                }

                string[] strValues = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int height = int.Parse(strValues[0]);
                int width = int.Parse(strValues[1]);
                int numberOfBlackHoles = int.Parse(strValues[2]);

                IProxxGame proxxGame = _proxxGameFactory.CreateProxxGame(height, width, numberOfBlackHoles);

                Console.WriteLine("Here is the board created for the game: ");
                Console.WriteLine();

                DrawField(proxxGame, false);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("""
                    To start new game type "new"
                    To click on cell type "cell <i> <j>"
                    To draw field with all info revealed type "draw"
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

    private static void DrawField(IProxxGame proxxGame, bool revealAll)
    {
        //write top numbers (tens)
        int currentWidth = proxxGame.Width;
        string topNumbersTensString = "";
        for (int tenNumber = 0; currentWidth > 0; tenNumber++, currentWidth -= 10)
        {
            char symbol = tenNumber == 0 ? ' ' : tenNumber.ToString()[0];
            topNumbersTensString += new string(symbol, currentWidth >= 10 ? 10 : currentWidth);
        }
        Console.WriteLine("   " + topNumbersTensString);

        //write top numbers (ones)
        string allDigits = "0123456789";
        string topNumbersOnesString = string.Concat(Enumerable.Repeat(allDigits, proxxGame.Width / 10)) + allDigits.Substring(0, proxxGame.Width % 10);
        Console.WriteLine("   " + topNumbersOnesString);

        Console.Write("  ");
        Console.Write(new string('█', proxxGame.Board[0].Length + 2));
        Console.WriteLine();

        for (int i = 0; i < proxxGame.Board.Length; i++)
        {
            Console.Write($"{i,2}█");
            for (int j = 0; j < proxxGame.Board[0].Length; j++)
            {
                char toWrite = proxxGame.Board[i][j] switch
                {
                    { IsRevealed: false } when !revealAll => '#',
                    { IsBlackHole: true } => 'X',
                    { AdjacentBlackHolesNumber: 0 } => ' ',
                    ProxxCell cell => cell.AdjacentBlackHolesNumber!.Value.ToString()[0]
                };
                Console.Write(toWrite);
            }
            Console.WriteLine('█');
        }

        Console.Write("  ");
        Console.Write(new string('█', proxxGame.Board[0].Length + 2));
        Console.WriteLine();
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

        if (proxxGame.GameFinishedWithAction != null)
        {
            string message = proxxGame.GameFinishedWithAction == ClickOnFieldResultActionEnum.GameWin
                ? "You win. Start new Game or review current."
                : "Game over. Start new Game or review current.";

            Console.WriteLine(message);
            return;
        }

        string[] strValues = userInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int i = int.Parse(strValues[1]);
        int j = int.Parse(strValues[2]);

        try
        {
            ClickOnFieldResultActionEnum clickResultAction = proxxGame.ClickOnCell(i, j);

            Console.WriteLine();
            DrawField(proxxGame, false);
            Console.WriteLine();

            string message = clickResultAction switch
            {
                ClickOnFieldResultActionEnum.NothingToDo => "Cell is already revealed.",
                ClickOnFieldResultActionEnum.RedrawCell => "Cell with number became revealed.",
                ClickOnFieldResultActionEnum.RedrawBoard => "Empty cell became revealed along with adjacent cells.",
                ClickOnFieldResultActionEnum.GameOver => "Game over.",
                ClickOnFieldResultActionEnum.GameWin => "You win.",
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
