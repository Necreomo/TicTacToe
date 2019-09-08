namespace Engine.AI
{

    /// <summary>
    /// Interface for the states of the FSM.
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    public interface IFSMState<T>
    {
        /// <summary>
        /// Called when entering a state.
        /// </summary>
        /// <param name="owner">The owner of the state</param>
        void Enter(T owner);

        /// <summary>
        /// Called while actively in the state.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        void Execute(T owner);

        /// <summary>
        /// Called when leaving the state.
        /// </summary>
        /// <param name="owner">The owner of the state.</param>
        void Exit(T owner);
    }
}