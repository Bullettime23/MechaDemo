using UnityEngine;
using System.Collections.Generic;
using Infrastructure;
using System;
using UnityEngine.UI;

namespace Mecha
{
    public class GridBehaviour : Singleton<GridBehaviour>
    {
        public Action<List<GameObject>> OnPathChoosen;
        public Action<GameObject> OnTargetChoosen;

        [SerializeField] private int m_Rows;
        [SerializeField] private int m_Columns;
        [SerializeField] private Color m_SelectedColor;
        public Color SelectedColor => m_SelectedColor;
        [SerializeField] private Color m_DefaultColor;
        public Color DefaultColor => m_DefaultColor;

        [SerializeField] private Vector3 m_LeftBottomLocation = new Vector3(0, 0, 0);
        private int m_StartX;
        private int m_StartZ;
        private int m_EndX;
        private int m_EndZ;

        private GridStat[,] m_Grids;
        private List<GridStat> m_GridsList;
        private List<GameObject> m_Path = new List<GameObject>();

        #region Public API
        public void SelectPathEnd()
        {
            foreach (GridStat grid in m_GridsList)
            {
                grid.EnableField();
            }

            GridStat.OnGridHover += SetFinishGridOfPath;
        }

        public void DisableGridForClick()
        {
            foreach (GridStat grid in m_GridsList)
            {
                grid.HideFieldInterface();
                grid.DisableField();
            }
        }

        public void SetStartCoordinatesOfUnit(Unit unit)
        {
            m_StartX = GridToWorldAdapter.PositionToGridCoordinates(unit.transform.position).x;
            m_StartZ = GridToWorldAdapter.PositionToGridCoordinates(unit.transform.position).z;
        }

        public GridStat TryGetGrid(Vector3 position)
        {
            int x = GridToWorldAdapter.PositionToGridCoordinates(position).x;
            int z = GridToWorldAdapter.PositionToGridCoordinates(position).z;
            return m_Grids[x, z] != null ? m_Grids[x, z] : null;
        }

        public void SetGridsAroundAsCover(GridStat grid, CoverType coverType)
        {
            int x = grid.x;
            int z = grid.z;

            //left to right
            if (x - 1 > -1 && m_Grids[x - 1, z])
            {
                m_Grids[x - 1, z].AddCover(new Cover(CoverDirection.Right, coverType));
            }
            // bottom to top
            if (z - 1 > -1 && m_Grids[x, z - 1])
            {
                m_Grids[x, z - 1].AddCover(new Cover(CoverDirection.Top, coverType));
            }
            // right to left
            if (x + 1 < m_Columns && m_Grids[x + 1, z])
            {
                m_Grids[x + 1, z].AddCover(new Cover(CoverDirection.Left, coverType));
            }
            // top to bottom
            if (z + 1 < m_Rows && m_Grids[x, z + 1])
            {
                m_Grids[x, z + 1].AddCover(new Cover(CoverDirection.Bottom, coverType));
            }
        }

        //TODO: Remove cover

        public void OnGridClick(GridStat grid)
        {
            GridStat.OnGridHover -= SetFinishGridOfPath;
            OnPathChoosen?.Invoke(m_Path);
        }
        #endregion


        #region Unity Actions

        private new void Awake()
        {
            base.Awake();
            GridToWorldAdapter.LeftBottomPosition = m_LeftBottomLocation;
            GridToWorldAdapter.GridScale = (int)transform.parent.transform.localScale.x;
            m_Grids = new GridStat[m_Columns, m_Rows];
            GenerateGrid();
        }
        //private void Start()
        //{
        //    m_Grids = new GridStat[m_Columns, m_Rows];
        //    GenerateGrid();
        //}
        #endregion

        #region Utility functions
        private void GenerateGrid()
        {
            m_GridsList = new List<GridStat>();

            foreach (GridStat grid in GetComponentsInChildren<GridStat>())
            {
                m_GridsList.Add(grid);

                m_Grids[grid.x, grid.z] = grid;
            }

        }

        private void SetFinishGridOfPath(GridStat grid)
        {
            m_EndX = grid.x;
            m_EndZ = grid.z;

            SetDistanse();
            SetPath();
        }

        #region Attack
        private GridStat m_TargetGrid;
        public void SelectTarget()
        {
            foreach (GridStat grid in m_GridsList)
            {
                grid.EnableField();
            }
            GridStat.OnGridHover += DisplaySingleField;
            GridStat.OnGridClick += GetObjectOnTheGrid;
        }

        private void DisplaySingleField(GridStat grid)
        {
            if (m_TargetGrid != null) {
                m_TargetGrid.HideFieldInterface();
            }
            grid.ShowFieldInterface();
            m_TargetGrid = grid;
        }
        private void GetObjectOnTheGrid(GridStat grid)
        {
            OnTargetChoosen(grid.ObjectOnGrid);
            GridStat.OnGridClick -= GetObjectOnTheGrid;
            GridStat.OnGridHover -= DisplaySingleField;
        }


        #endregion

        private void InitialSetup()
        {
            foreach (GridStat grid in m_Grids)
            {
                if (grid != null)
                {
                    grid.visited = -1;
                    grid.HideFieldInterface();
                }
            }
            m_Grids[m_StartX, m_StartZ].visited = 0;
        }

        private bool TestDirection(int x, int z, int step, int direction)
        {

            // int direction tells me which case to use 1 is up, 2 is right, 3 is down, 4 is left
            switch (direction)
            {
                case 4:
                    return x - 1 > -1 && m_Grids[x - 1, z] && !m_Grids[x - 1, z].IsBusy && m_Grids[x - 1, z].visited == step;
                case 3:
                    return z - 1 > -1 && m_Grids[x, z - 1] && !m_Grids[x, z - 1].IsBusy && m_Grids[x, z - 1].visited == step;
                case 2:
                    return x + 1 < m_Columns && m_Grids[x + 1, z] && !m_Grids[x + 1, z].IsBusy && m_Grids[x + 1, z].visited == step;
                case 1:
                    return z + 1 < m_Rows && m_Grids[x, z + 1] && !m_Grids[x, z + 1].IsBusy && m_Grids[x, z + 1].visited == step;
                default:
                    return false;
            }
        }

        private void SetDistanse()
        {
            InitialSetup();
            int x = m_StartX;
            int z = m_StartZ;
            //int[,] testArray = new int[m_Columns, m_Rows];
            for (int step = 1; step < m_Rows * m_Columns; step++)
            {
                foreach (GridStat grid in m_Grids)
                {
                    if (grid != null && grid.visited == step - 1)
                    {
                        TestFourDirections(grid.x, grid.z, step);
                    }
                }
            }
        }

        private void SetPath()
        {
            int step;
            int x = m_EndX;
            int z = m_EndZ;
            List<GridStat> tempList = new List<GridStat>();
            m_Path.Clear();

            GridStat destination = m_Grids[m_EndX, m_EndZ];

            if (destination && destination.visited > 0
            )
            {
                m_Path.Add(m_Grids[x, z].gameObject);
                step = m_Grids[x, z].visited - 1;
                m_Grids[x, z].ShowFieldInterface();
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

                if (tempList.Count > 0)
                {
                    GridStat tempObject = FindClosest(m_Grids[m_EndX, m_EndZ].transform, tempList);
                    tempObject.GetComponentInChildren<GridStat>().ShowFieldInterface();

                    m_Path.Add(tempObject.gameObject);
                    x = tempObject.GetComponent<GridStat>().x;
                    z = tempObject.GetComponent<GridStat>().z;
                    tempList.Clear();
                }
            }
            m_Path.Reverse();
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

        private GridStat FindClosest(Transform targetLocation, List<GridStat> list)
        {
            float currentDistance = m_Rows * m_Columns;
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
        #endregion
    }
}