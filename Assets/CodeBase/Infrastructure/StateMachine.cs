namespace Infrastructure {
    public abstract class StateMachine
    {
        private State m_CurrentState;

        #region Puglic API
        public State CurrentState => m_CurrentState;

        public void ChangeState(State nextState) { 
            m_CurrentState = nextState;
        }
        #endregion
    }
}