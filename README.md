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