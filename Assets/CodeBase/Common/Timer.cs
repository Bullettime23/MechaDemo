using UnityEngine;

namespace Common
{
    public class Timer
    {

        private float m_CurrentTime;

        public bool IsFinish => m_CurrentTime <= 0;

        public Timer(float startTime)
        {
            Start(startTime);
        }
        public void Start(float startTime)
        {
            m_CurrentTime = startTime;
        }

        public void RemoveTime(float deltaTime)
        {
            if (m_CurrentTime > 0)
                m_CurrentTime -= deltaTime;
        }

    }
}