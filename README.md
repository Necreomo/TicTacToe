# TicTacToe
Basic 1 Player Tic Tac Toe Game Built In Unity
</br>
This project was bulit with *Unity Version 2019.2.0f1*
</br>
The main scene file is *SC_Gameplay.unity* if it doesn't load automatically when launcing the project.
</br>
The project should run/build as is without modifications.
</br>
</br>
**Game Flow**
1. User has to pick to either be the X or O marker (X goes first)
2. Awaits input from either the player or the AI system based upon who's turn it is (AI has a value to adjust how long it takes to "Simulate Thinking" in the AISystem Component; default 2 seconds)
3. Evaluates the board state based upon the last placed marker for a win or a loss and end the game (Step 6)
4. Swap the current players turn
5. If condition 3 is not met Repeat steps 2-4 until the board is full then end the game as a tie
6. Prompt player to play again or quit the application

**Note:** The AI for this game is completely randomising positions that are available to the board and there is no logic behind it.
There is structure to allow different AI types however they are not implmented
