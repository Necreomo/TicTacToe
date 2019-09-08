using UnityEngine;
using Engine.AI;
using TicTacToe.AI;

namespace TicTacToe
{
    /// <summary>
    /// Class that handles all of the game logic a handling
    /// </summary>
    public class GameController : MonoBehaviour
    {
        /// <summary>
        /// max size of the current board
        /// </summary>
        private const int BoardSize = 3;

        #region Enums
        public enum PlayerID
        {
            X,
            O,
            NotSelected,
        }

        /// <summary>
        /// flag for who the winner is
        /// </summary>
        public enum Result
        {
            Player,
            AI,
            Tie,
            InProgress
        }

        /// <summary>
        /// position on grid used for reducing win check calculations
        /// </summary>
        public enum MarkerPositionType
        {
            Edge,
            Corner,
            Middle,
            Undefined
        }

        #endregion

        #region Inspector Variables

        [Header("References")]
        [SerializeField, Tooltip("Reference to O marker Prefab for the game board")]
        private GameObject _O_MarkerPrefab;

        [SerializeField, Tooltip("Reference to X marker Prefab for the game board")]
        private GameObject _X_MarkerPrefab;

        [SerializeField, Tooltip("Reference to the parent transform that will house all instantiated markers")]
        private Transform _markersParentRef;

        #endregion

        #region Properties
        /// <summary>
        /// state machine that runs the flow of the game
        /// </summary>
        public FiniteStateMachine<GameController> StateMachine { get; private set; }

        /// <summary>
        /// user maker representation
        /// </summary>
        public PlayerID UserPlayerID { get; private set; } = PlayerID.NotSelected;

        /// <summary>
        /// current board state
        /// </summary>
        public PlayerID[,] BoardState = new PlayerID[,] { 
                                                          { PlayerID.NotSelected, PlayerID.NotSelected, PlayerID.NotSelected},
                                                          { PlayerID.NotSelected, PlayerID.NotSelected, PlayerID.NotSelected},
                                                          { PlayerID.NotSelected, PlayerID.NotSelected, PlayerID.NotSelected}
                                                        };

        /// <summary>
        /// flag for the current turn being played
        /// </summary>
        public PlayerID CurrentPlayerTurn { get; set; } = PlayerID.X;

        /// <summary>
        /// bool represents that the player or ai has placed a marker
        /// </summary>
        public bool PlacedMarker { get; set; } = false;

        /// <summary>
        /// gameboard slot reference to when placing a marker on the board
        /// </summary>
        public Vector2Int PlacedMarkerPosID { get; set; } = new Vector2Int(int.MaxValue, int.MaxValue);

        /// <summary>
        /// tracks when both players have had a turn
        /// </summary>
        public int CurrentRound { get; set; } = 0;

        /// <summary>
        /// final result of the current game
        /// </summary>
        public Result GameResult { get; set; } = Result.InProgress;

        private static GameController _instance;
        public static GameController Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

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

            StateMachine = new FiniteStateMachine<GameController>(this, InitializeBoardState.Instance);
        }

        private void OnEnable()
        {
            AISystem.OnAICalculationComplete += AIResultCallback;
        }

        private void OnDisable()
        {
            AISystem.OnAICalculationComplete -= AIResultCallback;
        }
        

        private void Update()
        {
            StateMachine.Update();
        }

        /// <summary>
        /// resets all local variables and board state before starting a game
        /// </summary>
        public void ResetGame()
        {
            for (int i = 0; i < _markersParentRef.childCount; i++)
            {
                Destroy(_markersParentRef.GetChild(i).gameObject);
            }

            for (int i = 0; i < BoardState.GetLength(0); i++)
            {
                for (int j = 0; j < BoardState.GetLength(1); j++)
                {
                    BoardState[i, j] = PlayerID.NotSelected;
                }
            }

            UserPlayerID = PlayerID.NotSelected;
            CurrentPlayerTurn = PlayerID.X;
            CurrentRound = 0;
            GameResult = Result.InProgress;
            PlacedMarker = false;
            PlacedMarkerPosID = new Vector2Int(int.MaxValue, int.MaxValue);
        }

        /// <summary>
        /// assigns the player marker from the ui
        /// </summary>
        /// <param name="pickedPlayerMarker">reverence to the id picked from the ui</param>
        public void SetPlayerMarker(int pickedPlayerMarker)
        {
            UserPlayerID = (PlayerID)pickedPlayerMarker;
        }

        /// <summary>
        /// Try to process the user input on the game board
        /// </summary>
        /// <param name="row">row id of the trigger tapped</param>
        /// <param name="col">col id of the trigger tapped</param>
        /// <param name="position">physical position of the trigger to parent marker if successful</param>
        public void ExamineInputFromUser(int row, int col, Vector3 position)
        {
            if (UserPlayerID == PlayerID.NotSelected)
            {
                Debug.Log("Input Ignored Havn't selected Player yet");
                return;
            }
            else if (!StateMachine.IsInState(ThinkingState.Instance))
            {
                Debug.Log("Input Ignored Not in the awaiting iput state");
                return;
            }
            else if (UserPlayerID != CurrentPlayerTurn)
            {
                Debug.Log("Input Ignored Not your turn");
                return;
            }
            else if (!VerifyIfBoardSlotIsAvaiable(row, col))
            {
                Debug.Log("Input Ignored not valid slot selected");
                return;
            }

            BoardState[row, col] = CurrentPlayerTurn;
            if (UserPlayerID == PlayerID.X)
            {
                Instantiate(_X_MarkerPrefab, position, Quaternion.identity, _markersParentRef);
            }
            else
            {
                Instantiate(_O_MarkerPrefab, position, Quaternion.identity, _markersParentRef);
            }

            PlacedMarkerPosID = new Vector2Int(row, col);

            PlacedMarker = true;


        }

        
        /// <summary>
        /// Tells AI system to make a move
        /// </summary>
        public void SimulateAITurn()
        {
            AISystem.Instance.CalculateAITurn(BoardState, CurrentPlayerTurn);
        }

        /// <summary>
        /// verification check to see if the current player has won after placing a marker
        /// </summary>
        /// <returns>if the game should end</returns>
        public bool HasCurrentPlayerWon()
        {
            //Needs to at least have 3 pieces on the board before checking if winning
            if (CurrentRound >= 2)
            {

                MarkerPositionType markerPosType = MarkerPositionType.Undefined;

                bool hasWon = false;

                if (PlacedMarkerPosID.x == 1 && PlacedMarkerPosID.y == 1)
                {
                    markerPosType = MarkerPositionType.Middle;
                }
                else if ((PlacedMarkerPosID.x == 0 && PlacedMarkerPosID.y == 0) ||
                         (PlacedMarkerPosID.x == 0 && PlacedMarkerPosID.y == (BoardSize - 1)) ||
                         (PlacedMarkerPosID.x == (BoardSize - 1) && PlacedMarkerPosID.y == 0) ||
                         (PlacedMarkerPosID.x == (BoardSize - 1) && PlacedMarkerPosID.y == (BoardSize - 1)))
                {
                    markerPosType = MarkerPositionType.Corner;
                }
                else
                {
                    markerPosType = MarkerPositionType.Edge;
                }



                switch (markerPosType)
                {
                    case MarkerPositionType.Edge:
                        if (CheckRowWin(PlacedMarkerPosID.x) || CheckColWin(PlacedMarkerPosID.y))
                        {
                            hasWon = true;
                        }
                        break;
                    case MarkerPositionType.Corner:
                        if (CheckRowWin(PlacedMarkerPosID.x) ||
                            CheckColWin(PlacedMarkerPosID.y))
                        {
                            hasWon = true;
                        }
                        else
                        {
                            if (PlacedMarkerPosID.x - PlacedMarkerPosID.y == 0)
                            {
                                if (CheckDiagBackSlash())
                                {
                                    hasWon = true;
                                }
                            }
                            else
                            {
                                if (CheckDiagForwardSlash())
                                {
                                    hasWon = true;
                                }
                            }
                        }
                        break;
                    case MarkerPositionType.Middle:
                        if (CheckRowWin(PlacedMarkerPosID.x) || 
                            CheckColWin(PlacedMarkerPosID.y) ||
                            CheckDiagBackSlash() ||
                            CheckDiagForwardSlash())
                        {
                            hasWon = true;
                        }
                        break;
                    default:
                        break;
                }

                PlacedMarkerPosID = new Vector2Int(int.MaxValue, int.MaxValue);

                if (hasWon)
                {
                    if (CurrentPlayerTurn == UserPlayerID)
                    {
                        GameResult = Result.Player;
                    }
                    else
                    {
                        GameResult = Result.AI;
                    }

                    return true;

                }
            }
            return false;
        }

        /// <summary>
        /// checks to see if the current board has all its slots filled
        /// </summary>
        /// <returns></returns>
        public bool AreAllSpacesFilled()
        {
            return _markersParentRef.childCount == (BoardSize * BoardSize);
        }

        /// <summary>
        /// variables that need to be adjusted when a players turn is over
        /// </summary>
        public void EndTurnLogic()
        {

            if(CurrentPlayerTurn == PlayerID.O)
            {
                CurrentRound++;
                CurrentPlayerTurn = PlayerID.X;
            }
            else if (CurrentPlayerTurn == PlayerID.X)
            {
                CurrentPlayerTurn = PlayerID.O;
            }
        }



        /// <summary>
        /// checks to see if slot has not been occupied
        /// </summary>
        /// <param name="row">row id of the gameboard</param>
        /// <param name="col">col id of the gameboard</param>
        /// <returns>if slot has been occupied</returns>
        private bool VerifyIfBoardSlotIsAvaiable(int row, int col)
        {
            if (row >= 0 && row < BoardSize && col >= 0 && col < BoardSize)
            {
                return (BoardState[row, col] == PlayerID.NotSelected);
            }

            return false;
        }

        #region Verification Win Functions
        /// <summary>
        /// Check to see if the current player has won on the row the last piece was placed
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private bool CheckRowWin(int rowIndex)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                if (BoardState[rowIndex, y] != CurrentPlayerTurn)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check to see if the current player has won on the column of the last piece placed
        /// </summary>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        private bool CheckColWin(int colIndex)
        {
            for (int x = 0; x < BoardSize; x++)
            {
                if (BoardState[x, colIndex] != CurrentPlayerTurn)
                {
                    return false;
                }
            }

            return true;    
        }

        /// <summary>
        /// check the diagonal win that is from top left to bottom right
        /// </summary>
        /// <returns></returns>
        private bool CheckDiagBackSlash()
        {
            for (int x = 0; x < BoardSize; x++)
            {
                if (BoardState[x, x] != CurrentPlayerTurn)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// check the diagonal win that is from bottom left to top right
        /// </summary>
        /// <returns></returns>
        private bool CheckDiagForwardSlash()
        {
            for (int x = 0; x < BoardSize; x++)
            {
                if (BoardState[BoardSize - 1 - x, x] != CurrentPlayerTurn)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Callback for the ai system when the ai has done calculating a move
        /// </summary>
        /// <param name="results">slot id on the gameboard on where the ai picked</param>
        private void AIResultCallback(Vector2Int results)
        {
            if (results.x == int.MaxValue)
            {
                Debug.LogError("CANT PROCESS AI POSITION INVALID CHOICE");
                return;
            }

            Vector3 MarkerPosition = GameboardController.Instance.GetTransformFromBoardPosID(results);

            BoardState[results.x, results.y] = CurrentPlayerTurn;
            if (UserPlayerID == PlayerID.X)
            {
                Instantiate(_O_MarkerPrefab, MarkerPosition, Quaternion.identity, _markersParentRef);
            }
            else
            {
                Instantiate(_X_MarkerPrefab, MarkerPosition, Quaternion.identity, _markersParentRef);
            }

            PlacedMarker = true;
            PlacedMarkerPosID = results;
        }

    }
}
