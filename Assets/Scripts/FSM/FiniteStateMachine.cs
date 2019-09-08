using System;

namespace Engine.AI
{

    /// <summary>
    /// Basic state machine reference
    /// </summary>
    /// <typeparam name="T">The type of unit using the state machine.</typeparam>
    public class FiniteStateMachine<T>
    {
        /// <summary>
        /// The owner of the state machine
        /// </summary>
        private T Owner;

        /// <summary>
        /// The current state the owner is in.
        /// </summary>
        private IFSMState<T> CurrentState { get; set; }

        /// <summary>
        /// The previous state of the owner.
        /// </summary>
        private IFSMState<T> PreviousState { get; set; }

        /// <summary>
        /// The global state of the owner.  Something they should be doing regardless of the state they are in.
        /// </summary>
        private IFSMState<T> GlobalState { get; set; }

        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="owner">The owner of the state mahcine.</param>
        /// <param name="initialState">The initial state of the owner.</param>
        /// <param name="globalState">The global state of the unit.</param>
        public FiniteStateMachine(T owner, IFSMState<T> initialState, IFSMState<T> globalState = null)
        {
            Owner = owner;
            ChangeState(initialState);
            GlobalState = globalState;
        }

        /// <summary>
        /// Execute the state update calls
        /// </summary>
        public void Update()
        {
            if (GlobalState != null)
            {
                GlobalState.Execute(Owner);
            }

            if (CurrentState != null)
            {
                CurrentState.Execute(Owner);
            }
        }

        /// <summary>
        /// Exit the current state and transition to the new state.
        /// </summary>
        /// <param name="newState">The new state the unit will transition to after leaving the current state.</param>
        public void ChangeState(IFSMState<T> newState)
        {
            if (newState == null)
            {
                throw new ArgumentNullException("A new state is required when changing states");
            }

            PreviousState = CurrentState;

            if (CurrentState != null)
            {
                CurrentState.Exit(Owner);
            }

            CurrentState = newState;
            CurrentState.Enter(Owner);
        }

        /// <summary>
        /// Returns the unit to the previous state.
        /// </summary>
        public void RevertToPreviousState()
        {
            ChangeState(PreviousState);
        }

        /// <summary>
        /// Check to see what if the current state of the FSM is what passed in
        /// </summary>
        /// <param name="state">state to check against</param>
        /// <returns></returns>
        public bool IsInState(IFSMState<T> state)
        {
            return CurrentState.GetType() == state.GetType();
        }
    }
}