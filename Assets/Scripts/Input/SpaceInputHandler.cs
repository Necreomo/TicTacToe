using UnityEngine;
using TicTacToe;

namespace Engine.Input
{

    /// <summary>
    /// used for triggering when the player has click on a space
    /// </summary>
    public class SpaceInputHandler : MonoBehaviour
    {
        [Header("Board Quadrant ID")]
        [SerializeField]
        private int _rowID;
        public int RowID
        {
            get
            {
                return _rowID;
            }
        }

        [SerializeField]
        private int _colID;
        public int ColID
        {
            get
            {
                return _colID;
            }
        }

        private void OnMouseUpAsButton()
        {
            GameController.Instance.ExamineInputFromUser(_rowID, _colID, transform.position);
        }

        /// <summary>
        /// verifies the id passed is this specifics handler
        /// </summary>
        /// <param name="id">gameboard id</param>
        /// <returns></returns>
        public bool VerifyIDSpaceHandlerObject(Vector2Int id)
        {
            return VerifyIDSpaceHandlerObject(id.x, id.y);
        }

        /// <summary>
        /// verifies the id passed is this specifics handler
        /// </summary>
        /// <param name="id">gameboard id</param>
        /// <returns></returns>
        public bool VerifyIDSpaceHandlerObject(int row, int col)
        {
            return (_rowID == row && _colID == col);
        }
    }
}
