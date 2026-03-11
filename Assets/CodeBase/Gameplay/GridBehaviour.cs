using UnityEngine;
using System.Collections.Generic;
using Infrastructure;
using System;
using UnityEngine.EventSystems;

namespace Mecha
{
    [RequireComponent(typeof(LineRenderer))]
    public class GridBehaviour : Singleton<GridBehaviour>
    {
        /// <summary>
        /// События при построении пути, выборе цели и отмене действия
        /// </summary>
        public Action<List<GameObject>> OnPathChoosen;
        public Action<GameObject> OnTargetChoosen;
        public Action OnAbortSelect;

        // Знает количество строк и столбцов (Задается от руки)
        [SerializeField] private int m_Rows;
        [SerializeField] private int m_Columns;

        //Нижнее левое положение в реальном мире
        [SerializeField] private Vector3Int m_LeftBottomLocation = new Vector3Int(0, 0, 0);

        //Двумерный массив ссылок на клетки игрового поля
        private GridStat[,] m_Grids;
        public GridStat[,] Grids => m_Grids;

        private List<GridStat> m_GridsList;

        private Unit m_UnitToMove;

        #region Public API
        /// <summary>
        /// Активирует интерфейс поля для выбора последней клетки на пути
        /// </summary>
        public void BuildPath(Unit unit)
        {
            m_UnitToMove = unit;
            foreach (GridStat grid in m_GridsList)
            {
                grid.EnableField();
            }

            // Рассчитать количество ходов до доступных клеток
            if (unit.ReachableGrids == null)
            {
                unit.ReachableGrids = GridBehaviourPathing.FindReachableGridsWithBreadthFirstSearch(unit);
            }            

            // Разметить достижимые клетки поиском в ширину
            foreach (GridStat grid in unit.ReachableGrids)
            {
                grid.ShowFieldInterfaceCovers();
            }

            GridStat.OnGridHover += SetFinishGridOfPath;
            GridStat.OnGridClick += OnGridClick;
        }

        /// <summary>
        /// Отключает интерфейс клеток на поле, отписывается от событий
        /// </summary>
        public void DisableGridForClick()
        {
            foreach (GridStat grid in m_GridsList)
            {
                grid.HideFieldInterface();
                grid.DisableField();
            }
        }

        // Д
        public GridStat TryGetGrid(Vector3 position)
        {
            int x = GridToWorldAdapter.PositionToGridCoordinates(position).x;
            int z = GridToWorldAdapter.PositionToGridCoordinates(position).z;
            return m_Grids[x, z] != null ? m_Grids[x, z] : null;
        }

        #region Covers

        // TODO: Можно упростить

        /// <summary>
        /// Клетки вокруг получают соответствующее укрытие
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="gridContent"></param>
        public void SetGridsAroundAsCover(GridStat grid, GridContent gridContent)
        {
            int x = grid.x;
            int z = grid.z;

            //left to right
            if (x - 1 > -1 && m_Grids[x - 1, z])
            {
                m_Grids[x - 1, z].AddCover(new Cover(CoverDirection.Right, gridContent.ObstacleCoverType, gridContent));
            }
            // bottom to top
            if (z - 1 > -1 && m_Grids[x, z - 1])
            {
                m_Grids[x, z - 1].AddCover(new Cover(CoverDirection.Top, gridContent.ObstacleCoverType, gridContent));
            }
            // right to left
            if (x + 1 < m_Columns && m_Grids[x + 1, z])
            {
                m_Grids[x + 1, z].AddCover(new Cover(CoverDirection.Left, gridContent.ObstacleCoverType, gridContent));
            }
            // top to bottom
            if (z + 1 < m_Rows && m_Grids[x, z + 1])
            {
                m_Grids[x, z + 1].AddCover(new Cover(CoverDirection.Bottom, gridContent.ObstacleCoverType, gridContent));
            }
        }

        public void RemoveCoverFromGridsAround(GridStat grid)
        {
            int x = grid.x;
            int z = grid.z;

            //left to right
            if (x - 1 > -1 && m_Grids[x - 1, z])
            {
                m_Grids[x - 1, z].RemoveCover(CoverDirection.Right);
            }
            // bottom to top
            if (z - 1 > -1 && m_Grids[x, z - 1])
            {
                m_Grids[x, z - 1].RemoveCover(CoverDirection.Top);
            }
            // right to left
            if (x + 1 < m_Columns && m_Grids[x + 1, z])
            {
                m_Grids[x + 1, z].RemoveCover(CoverDirection.Left);
            }
            // top to bottom
            if (z + 1 < m_Rows && m_Grids[x, z + 1])
            {
                m_Grids[x, z + 1].RemoveCover(CoverDirection.Bottom);
            }
        }

        #endregion
        #endregion

        #region Unity Actions

        private new void Awake()
        {
            base.Awake();
            m_LineRend = GetComponent<LineRenderer>();
            m_LineRend.enabled = false;

            GridToWorldAdapter.LeftBottomPosition = m_LeftBottomLocation;
            GridToWorldAdapter.GridScale = (int)transform.parent.transform.localScale.x;
            m_Grids = new GridStat[m_Columns, m_Rows];
            GenerateGrid();
        }
        #endregion

        #region Utility functions

        // Создание двумерного массива с клетками
        private void GenerateGrid()
        {
            m_GridsList = new List<GridStat>();

            foreach (GridStat grid in GetComponentsInChildren<GridStat>())
            {
                m_GridsList.Add(grid);

                m_Grids[grid.x, grid.z] = grid;
            }

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
            if (m_TargetGrid != null)
            {
                m_TargetGrid.HideFieldInterface();
            }
            grid.ShowFieldInterface();
            m_TargetGrid = grid;
        }
        private void GetObjectOnTheGrid((GridStat grid, PointerEventData pointerData) props)
        {
            GridStat.OnGridClick -= GetObjectOnTheGrid;
            GridStat.OnGridHover -= DisplaySingleField;
            if (props.pointerData.button == PointerEventData.InputButton.Right || !props.grid.IsBusy)
            {
                OnAbortSelect?.Invoke();
            }
            if (props.pointerData.button == PointerEventData.InputButton.Left && props.grid.IsBusy)
            {
                OnTargetChoosen(props.grid.ObjectOnGrid);
            }
            m_TargetGrid.HideFieldInterface();
            m_TargetGrid = null;
        }

        /// <summary>
        /// Удаляет индекс шага со всех клеток на поле и отключает интерфейс
        /// </summary>
        //private void InitialSetup()
        //{
        //    foreach (GridStat grid in m_Grids)
        //    {
        //        if (grid != null)
        //        {
        //            grid.visited = -1;
        //            grid.HideFieldInterface();
        //        }
        //    }
        //}
        #endregion

        #region Pathfinding
        //Одноразовый объект пути
        private List<GameObject> m_Path = new List<GameObject>();
        private LineRenderer m_LineRend;

        public void ResetReachableGrids(Unit unit)
        {
            foreach (GridStat grid in unit.ReachableGrids)
            {
                unit.ReachableGrids = null;
                grid.ResetPath();
                grid.HideFieldInterface();
            }

            m_Path.Clear();                
        }

        /// <summary>
        /// Срабатывает при наведении на клетку поля. Сначала размечает все клетки на поле шагами
        /// Затем устанавливает путь
        /// </summary>
        /// <param name="grid"></param>
        private void SetFinishGridOfPath(GridStat destination)
        {
            if (m_UnitToMove.ReachableGrids.Contains(destination))
            {
                m_Path = GridBehaviourPathing.CreatePathBetweenGrids(m_UnitToMove.CurrentGrid, destination);
                GridBehaviourPathing.HighlightPathWithLine(m_Path, m_LineRend);
            }
        }

        private void OnGridClick((GridStat grid, PointerEventData pointerData) props)
        {
            DisableGridForClick();

            GridStat.OnGridHover -= SetFinishGridOfPath;
            GridStat.OnGridClick -= OnGridClick;

            m_LineRend.enabled = false;
            // Если кликнуть не правой кнопкой мыши или слишком далеко от юнита
            if (props.pointerData.button == PointerEventData.InputButton.Left && props.grid.visited != -1)
            {
                OnPathChoosen?.Invoke(m_Path);
                return;
            }

            OnAbortSelect?.Invoke();
        }
        #endregion
        #endregion
    }
}
