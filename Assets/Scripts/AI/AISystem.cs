using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.AI
{
    /// <summary>
    /// Enum to set teh ai type the computer to use
    /// </summary>
    public enum AIType
    {
        Random,
        AIType2_NotImplmented,
    }

    /// <summary>
    /// Class that runs the AI on the current board state
    /// </summary>
    public class AISystem : MonoBehaviour
    {
        #region events
        /// <summary>
        /// Delegate used to fake thinking time for the ai
        /// </summary>
        /// <param name="aiChosenPosition">the id of the position the ai picked</param>
        public delegate void AICalculationComplete(Vector2Int aiChosenPosition);
        public static event AICalculationComplete OnAICalculationComplete;
        #endregion

        [SerializeField, Tooltip("Set The Ai Used for the current session")]
        private AIType _aiType = AIType.Random;

        [SerializeField, Tooltip("Time to fake how much the ai will think before doing a move")]
        private float _aiFakeThinkingTime = 1.0f;

        /// <summary>
        /// reference to the wait coroutine for the ai
        /// </summary>
        private Coroutine FakeAIThinking;

        private static AISystem _instance;
        public static AISystem Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

        }

        /// <summary>
        /// calculation used to determine where the ai wants to place thier marker
        /// </summary>
        /// <param name="boardState">current state of the board</param>
        /// <param name="aiPlayerID">id represents if ai is X or O</param>
        public void CalculateAITurn(GameController.PlayerID[,] boardState, GameController.PlayerID aiPlayerID)
        {
            Vector2Int retValue = new Vector2Int(int.MaxValue, int.MaxValue);

            switch (_aiType)
            {
                case AIType.Random:
                    retValue = RandomisePlacement(boardState);
                    break;
                case AIType.AIType2_NotImplmented:
                    //Todo Not Implmented
                    break;
                default:
                    break;
            }

            if (FakeAIThinking != null)
            {
                StopCoroutine(FakeAIThinking);
            }

            FakeAIThinking = StartCoroutine(FakeAIProcessing(retValue));
            
        }

        /// <summary>
        /// logic to dumbly randomize a position that is availible on the board
        /// </summary>
        /// <param name="boardState">reference to the current board state</param>
        /// <returns>placement id of the space the ai picked</returns>
        public Vector2Int RandomisePlacement(GameController.PlayerID[,] boardState)
        {
            List<Vector2Int> validPositions = new List<Vector2Int>();

            Vector2Int retValue = new Vector2Int(int.MaxValue, int.MaxValue);

            for (int i = 0; i < boardState.GetLength(0); i++)
            {
                for (int j = 0; j < boardState.GetLength(1); j++)
                {
                    if (boardState[i, j] == GameController.PlayerID.NotSelected)
                    {
                        validPositions.Add(new Vector2Int(i, j));
                    }
                }
            }

            if (validPositions.Count > 0)
            {
                int randomPosition = Random.Range(0, validPositions.Count);

                retValue.x = validPositions[randomPosition].x;
                retValue.y = validPositions[randomPosition].y;
            }


            return retValue;
        }

        /// <summary>
        /// Coroutine taht fakes thinking time for the ai
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private IEnumerator FakeAIProcessing(Vector2Int result)
        {
            yield return new WaitForSeconds(_aiFakeThinkingTime);

            OnAICalculationComplete?.Invoke(result);
        }
    }
}
