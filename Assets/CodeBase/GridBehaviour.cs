using UnityEngine;
using System.Collections.Generic;

namespace Mecha
{
    public class GridBehaviour : MonoBehaviour
    {
        [SerializeField] bool m_FindDistance;
        [SerializeField] private int m_Rows;
        [SerializeField] private int m_Columns;
        [SerializeField] private int m_Scale = 1;
        [SerializeField] private GameObject m_GridPrefab;
        [SerializeField] private Vector3 m_LeftBottomLocation = new Vector3(0, 0, 0);
        [SerializeField] private int m_StartX;
        [SerializeField] private int m_StartZ;
        [SerializeField] private int m_EndX;
        [SerializeField] private int m_EndZ;
        private GameObject[,] m_Grids;
        private List<GameObject> m_Path = new List<GameObject>();

        //Debug
        [SerializeField] private Material m_TestMaterial;

        private void Start()
        {
            if (m_GridPrefab != null)
            {
                m_Grids = new GameObject[m_Columns, m_Rows];
                GenerateGrid();
            }
        }

        private void Update()
        {
            if (m_FindDistance)
            {
                SetDistanse();
                SetPath();
                m_FindDistance = false;
            }
        }
        public void GenerateGrid()
        {
            for (int x = 0; x < m_Columns; x++)
            {
                for (int z = 0; z < m_Columns; z++)
                {
                    GameObject grid = Instantiate(
                        m_GridPrefab,
                        m_LeftBottomLocation + new Vector3(x * m_Scale, transform.position.y, z * m_Scale),
                        Quaternion.identity,
                        gameObject.transform
                    );
                    grid.GetComponent<GridStat>().x = x;
                    grid.GetComponent<GridStat>().z = z;
                    m_Grids[x, z] = grid;
                }
            }
        }

        private void InitialSetup()
        {
            foreach (GameObject grid in m_Grids)
            {
                if (grid != null)
                    grid.GetComponent<GridStat>().visited = -1;
            }
            m_Grids[m_StartX, m_StartZ].GetComponent<GridStat>().visited = 0;
        }

        public bool TestDirection(int x, int z, int step, int direction)
        {
            // int direction tells me which case to use 1 is up, 2 is right, 3 is down, 4 is left
            switch (direction)
            {
                case 4:
                    return x - 1 > -1 && m_Grids[x - 1, z] && m_Grids[x - 1, z].GetComponent<GridStat>().visited == step;
                case 3:
                    return z - 1 > -1 && m_Grids[x, z - 1] && m_Grids[x, z - 1].GetComponent<GridStat>().visited == step;
                case 2:
                    return x + 1 < m_Columns && m_Grids[x + 1, z] && m_Grids[x + 1, z].GetComponent<GridStat>().visited == step;
                case 1:
                    return z + 1 < m_Rows && m_Grids[x, z + 1] && m_Grids[x, z + 1].GetComponent<GridStat>().visited == step;
                default:
                    return false;
            }
        }

        private void SetDistanse()
        {
            InitialSetup();
            int x = m_StartX;
            int z = m_StartZ;
            int[,] testArray = new int[m_Columns, m_Rows];
            for (int step = 1; step < m_Rows * m_Columns; step++)
            {
                foreach (GameObject gameObj in m_Grids)
                {
                    if (gameObj != null)
                    {
                        GridStat grid = gameObj.GetComponent<GridStat>();

                        if (grid && grid.visited == step - 1)
                        {
                            TestFourDirections(grid.x, grid.z, step);
                        }
                    }
                }
            }
        }

        private void SetPath()
        {
            int step;
            int x = m_EndX;
            int z = m_EndZ;
            List<GameObject> tempList = new List<GameObject>();
            m_Path.Clear();

            GameObject destination = m_Grids[m_EndX, m_EndZ];

            if (destination && destination.GetComponent<GridStat>().visited > 0
            )
            {
                m_Path.Add(m_Grids[x, z]);
                step = m_Grids[x, z].GetComponent<GridStat>().visited - 1;
            }
            else
            {
                print("Cant reach this location");
                return;
            }
            for (int i = step; step > -1; step--)
            {
                if (TestDirection(x, z, step, 1))
                    tempList.Add(m_Grids[x, z + 1]);
                if (TestDirection(x, z, step, 2))
                    tempList.Add(m_Grids[x + 1, z]);
                if (TestDirection(x, z, step, 3))
                    tempList.Add(m_Grids[x, z - 1]);
                if (TestDirection(x, z, step, 4))
                    tempList.Add(m_Grids[x - 1, z]);

                GameObject tempObject = FindClosest(m_Grids[m_EndX, m_EndZ].transform, tempList);
                //Debug;
                tempObject.GetComponent<MeshRenderer>().material = m_TestMaterial;

                m_Path.Add(tempObject);
                x = tempObject.GetComponent<GridStat>().x;
                z = tempObject.GetComponent<GridStat>().z;
                tempList.Clear();
            }
        }

        private void TestFourDirections(int x, int z, int step)
        {
            if (TestDirection(x, z, -1, 1))
                SetVisited(x, z + 1, step);
            if (TestDirection(x, z, -1, 2))
                SetVisited(x + 1, z, step);
            if (TestDirection(x, z, -1, 3))
                SetVisited(x, z - 1, step);
            if (TestDirection(x, z, -1, 4))
                SetVisited(x - 1, z, step);
        }

        public void SetVisited(int x, int z, int step)
        {
            m_Grids[x, z].GetComponent<GridStat>().visited = step;
        }

        private GameObject FindClosest(Transform targetLocation, List<GameObject> list)
        {
            float currentDistance = m_Scale * m_Rows * m_Columns;
            int indexNumber = 0;
            for (int i = 0; i < list.Count; i++)
            {
                float nextDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                if (nextDistance < currentDistance)
                {
                    currentDistance = nextDistance;
                    indexNumber = i;
                }
            }
            return list[indexNumber];
        }
    }
}