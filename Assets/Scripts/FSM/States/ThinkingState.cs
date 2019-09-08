using Engine.AI;
using TicTacToe.UI;

namespace TicTacToe
{
    /// <summary>
    /// State that awaits user input or ai input
    /// </summary>
    public class ThinkingState : IFSMState<GameController>
    {
        private static readonly ThinkingState _instance = new ThinkingState();
        public static ThinkingState Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Enter(GameController owner)
        {
            string stateString;
            if (owner.CurrentPlayerTurn == owner.UserPlayerID)
            {
                stateString = string.Format("{0} {1}", Constants.GameStateStrings.Player, Constants.GameStateStrings.Turn);
            }
            else
            {
                stateString = string.Format("{0} {1}", Constants.GameStateStrings.AI, Constants.GameStateStrings.Thinking);
                owner.SimulateAITurn();
            }

            UIManager.OverrideStateLabel(stateString);
        }

        public void Execute(GameController owner)
        {
            if (owner.PlacedMarker)
            {
                owner.PlacedMarker = false;
                owner.StateMachine.ChangeState(CheckBoardState.Instance);
            }
        }

        public void Exit(GameController owner)
        {
            
        }
    }
}
