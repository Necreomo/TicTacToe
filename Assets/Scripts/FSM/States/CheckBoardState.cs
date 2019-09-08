using Engine.AI;

namespace TicTacToe
{
    /// <summary>
    /// state to check the board after a player or an ai move
    /// </summary>
    public class CheckBoardState : IFSMState<GameController>
    {
        private static readonly CheckBoardState _instance = new CheckBoardState();
        public static CheckBoardState Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Enter(GameController owner)
        {
            if (owner.HasCurrentPlayerWon())
            {
                owner.StateMachine.ChangeState(EndGameState.Instance);
            }
            else
            {
                if (owner.AreAllSpacesFilled())
                {
                    owner.GameResult = GameController.Result.Tie;
                    owner.StateMachine.ChangeState(EndGameState.Instance);
                }
                else
                {
                    owner.EndTurnLogic();
                    owner.StateMachine.ChangeState(ThinkingState.Instance);
                }
            }
        }

        public void Execute(GameController owner)
        {
        }

        public void Exit(GameController owner)
        {

        }
    }
}