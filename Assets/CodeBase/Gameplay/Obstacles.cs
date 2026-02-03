using UnityEngine;

namespace Mecha {

    public class Obstacles : MonoBehaviour
    {
        private GridStat[] m_Obstacles;

        private GridStat[,] m_ObstaclesMatrix;
        
        public void Initialize(int columns, int rows)
        {
            m_ObstaclesMatrix = new GridStat[columns, rows];

            foreach (GridStat obstacle in m_Obstacles) {
                m_ObstaclesMatrix[obstacle.x, obstacle.z] = obstacle;
            }
        }

        public GridStat TryGetObstacle(int x, int z)
        {
            return m_ObstaclesMatrix[x, z];
        }

        private void Start()
        {
            m_Obstacles = GetComponentsInChildren<GridStat>();
        }
    }
}