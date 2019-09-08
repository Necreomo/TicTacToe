using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TicTacToe.UI
{

    /// <summary>
    /// Class that handles all the ui logic
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class UIManager : MonoBehaviour
    {
        public enum ButtonType
        {
            Pick_X,
            Pick_O,
            Retry,
            Quit
        }

        [SerializeField, Tooltip("Refernce to the generic label used for all ui")]
        private TMP_Text _gameStateLabel;

        [SerializeField, Tooltip("reference to the ui dialog that holds the button for picking a marker")]
        private GameObject UIPickPlayerParent;

        [SerializeField, Tooltip("Reference to the ui dialog that holds the buttons for retrying or quitting after a game has finished")]
        private GameObject UIRetryParent;

        private static UIManager _instance;
        public static UIManager Instance
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

           // _gameStateLabel.gameObject.SetActive(false);
           // UIPickPlayerParent.SetActive(false);
           // UIRetryParent.SetActive(false);
        }

        /// <summary>
        /// function that shows or hides the retry dialog
        /// </summary>
        /// <param name="show"></param>
        public static void RetryUIDisplay(bool show)
        {
            Instance.UIRetryParent.SetActive(show);
        }

        /// <summary>
        /// function that shows or hides the marker choice ui 
        /// </summary>
        /// <param name="show"></param>
        /// <param name="labelOverride"></param>
        public static void PickPlayerUIDisplay(bool show, string labelOverride = "")
        {

            OverrideStateLabel(labelOverride);
            Instance._gameStateLabel.gameObject.SetActive(true);
            Instance.UIPickPlayerParent.SetActive(show);

        }

        /// <summary>
        /// override the generic label text string
        /// </summary>
        /// <param name="textString"></param>
        public static void OverrideStateLabel(string textString)
        {
            if (Instance._gameStateLabel != null)
            {
                Instance._gameStateLabel.text = textString;
            }
            
        }

        /// <summary>
        /// ui button callbacks 
        /// </summary>
        /// <param name="id"></param>
        public void ButtonCallback(int id)
        {
            switch ((ButtonType)id)
            {
                case ButtonType.Pick_X:
                case ButtonType.Pick_O:
                    GameController.Instance.SetPlayerMarker(id);
                    break;
                case ButtonType.Retry:
                    RetryUIDisplay(show: false);
                    GameController.Instance.StateMachine.ChangeState(InitializeBoardState.Instance);
                    break;
                case ButtonType.Quit:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                    break;
                default:
                    break;
            }


        }

    }
}
