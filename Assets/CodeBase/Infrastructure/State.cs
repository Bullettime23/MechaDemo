
namespace Infrastructure {
    public abstract class State
    {
        protected StateMachine m_StateMachine;
        public State(StateMachine stateMachine)
        {
            m_StateMachine = stateMachine;
        }
    }
}