using Engine.AI;
using TicTacToe.UI;

namespace TicTacToe
{
    /// <summary>
    /// state used when either the game has ended because of a win or a draw
    /// </summary>
    public class EndGameState : IFSMState<GameController>
    {
        private static readonly EndGameState _instance = new EndGameState();
        public static EndGameState Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Enter(GameController owner)
        {
            string finalString = string.Empty;

            switch (owner.GameResult)
            {
                case GameController.Result.Player:
                    finalString = string.Format("{0} {1}", Constants.GameStateStrings.Player, Constants.GameStateStrings.Wins);
                    break;
                case GameController.Result.AI:
                    finalString = string.Format("{0} {1}", Constants.GameStateStrings.AI, Constants.GameStateStrings.Wins);
                    break;
                case GameController.Result.Tie:
                    finalString = Constants.GameStateStrings.ResultTie;
                    break;
                default:
                    break;
            }

            UIManager.OverrideStateLabel(finalString);
            UIManager.RetryUIDisplay(show: true);
        }

        public void Execute(GameController owner)
        {
        }

        public void Exit(GameController owner)
        {

        }
    }
}