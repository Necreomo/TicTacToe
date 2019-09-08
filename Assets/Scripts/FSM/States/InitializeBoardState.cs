using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine.AI;
using TicTacToe.UI;

namespace TicTacToe
{
    /// <summary>
    /// default state the game starts on resets the board state and requests player for marker choice
    /// </summary>
    public class InitializeBoardState : IFSMState<GameController>
    {
        private static readonly InitializeBoardState _instance = new InitializeBoardState();
        public static InitializeBoardState Instance
        {
            get
            {
                return _instance;
            }
        }

        public void Enter(GameController owner)
        {
            owner.ResetGame();
            UIManager.PickPlayerUIDisplay(show: true, Constants.GameStateStrings.PlayAsXorO);
        }

        public void Execute(GameController owner)
        {
            if (owner.UserPlayerID != GameController.PlayerID.NotSelected)
            {
                UIManager.PickPlayerUIDisplay(show: false);
                owner.StateMachine.ChangeState(ThinkingState.Instance);
            }
        }

        public void Exit(GameController owner)
        {
            
        }
    }
}
