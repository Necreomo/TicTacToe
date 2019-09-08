using UnityEngine;
using Engine.Input;

namespace TicTacToe
{
    /// <summary>
    /// contains a refernce to the touch zones in the gameboard
    /// </summary>
    public class GameboardController : MonoBehaviour
    {
        [SerializeField, Tooltip("Reference to the parent of the touch areas")]
        private Transform _parentOfTouchAreas;

        /// <summary>
        /// reference to all the touch handlers attached to board
        /// </summary>
        private SpaceInputHandler[] touchAreaHandlers;

        private static GameboardController _instance;
        public static GameboardController Instance
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

            touchAreaHandlers = new SpaceInputHandler[_parentOfTouchAreas.childCount];

            for (int i = 0; i < _parentOfTouchAreas.childCount; i++)
            {
                touchAreaHandlers[i] = _parentOfTouchAreas.GetChild(i).GetComponent<SpaceInputHandler>();
            }
        }

        /// <summary>
        /// Gets the Position data of a input trigger based on an gameboard id
        /// </summary>
        /// <param name="boardPosID">id representation of one of the spots</param>
        /// <returns>middle position of the trigger</returns>
        public Vector3 GetTransformFromBoardPosID(Vector2Int boardPosID)
        {
            return GetTransformFromBoardPosID(boardPosID.x, boardPosID.y);
        }

        /// <summary>
        /// Gets the Position data of a input trigger based on an gameboard id same as above but int represetnation
        /// </summary>
        /// <param name="row">row representation of one of the spots</param>
        /// <param name="col">column representation of one of the spots</param>
        /// <returns>middle position of the trigger</returns>
        public Vector3 GetTransformFromBoardPosID(int row, int col)
        {
            for (int i = 0; i < touchAreaHandlers.Length; i++)
            {
                if (touchAreaHandlers[i].RowID == row && touchAreaHandlers[i].ColID == col)
                {
                    return touchAreaHandlers[i].transform.position;
                }
            }

            return Vector3.zero;

        }
    }
}
