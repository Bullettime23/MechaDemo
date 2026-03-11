using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mecha
{
    public static class GridBehaviourPathing
    {
        /// <summary>
        /// Попытается взять получить клетку по координатам, иначе вернет null
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="direction">from 1 to 8, from left top corner through right bottom and left as 8</param>
        /// <returns>GridStat</returns>
        private static GridStat TryGetGridByDirection(int x, int z, int direction)
        {
            // int direction tells me which case to use 1 is up-left, 2 - up, 3- up-right, 4 - right, 5 - bottom-right
            // 6 - bottom, 7 bottom-left, 8 - left
            switch (direction)
            {
                case 1:
                    return TryGetUnsteppedAvaliableGrid(x - 1, z + 1);
                case 2:
                    return TryGetUnsteppedAvaliableGrid(x, z + 1);
                case 3:
                    return TryGetUnsteppedAvaliableGrid(x + 1, z + 1);
                case 4:
                    return TryGetUnsteppedAvaliableGrid(x + 1, z);
                case 5:
                    return TryGetUnsteppedAvaliableGrid(x + 1, z - 1);
                case 6:
                    return TryGetUnsteppedAvaliableGrid(x, z - 1);
                case 7:
                    return TryGetUnsteppedAvaliableGrid(x - 1, z - 1);
                case 8:
                    return TryGetUnsteppedAvaliableGrid(x - 1, z);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Построить путь от конченой клетки к юниту
        /// </summary>
        /// <param name="destinationGrid">Конечная клетка пути</param>
        public static List<GameObject> CreatePathBetweenGrids(GridStat start, GridStat finish)
        {
            List<GameObject> path = new();

            GridStat currentGrid = finish;

            while (currentGrid != start)
            {
                path.Add(currentGrid.gameObject);
                currentGrid = currentGrid.ParentGrid;
            }

            path.Reverse();
            return path;
        }

        private static readonly Vector3 m_PathlineOffsetTop = Vector3.up * 3;
        public static void HighlightPathWithLine(List<GameObject> path, LineRenderer lineRenderer)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = path.Count;

            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i, path[i].transform.position + m_PathlineOffsetTop);
            }
        }

        // На клетку не настпали, если её visited = -1
        public static bool IsGridExistAvaliableAndHaveStep(int x, int z, int step)
        {
            GridStat[,] grids = GridBehaviour.Instance.Grids;
            int columns = grids.GetLength(0);
            int rows = grids.GetLength(1);

            return x < columns && x > -1 &&
                z > -1 && z < rows &&
                grids[x, z] != null &&
                !grids[x, z].IsBusy &&
                grids[x, z].visited == step;
        }

        private static GridStat TryGetUnsteppedAvaliableGrid(int x, int z)
        {
            GridStat[,] grids = GridBehaviour.Instance.Grids;
            int columns = grids.GetLength(0);
            int rows = grids.GetLength(1);

            if (x < columns && x > -1 &&
                z > -1 && z < rows &&
                grids[x, z] != null &&
                !grids[x, z].IsBusy &&
                grids[x, z].visited == -1)
            {
                return grids[x, z];
            }
            return null;
        }

        /// <summary>
        /// Найти все клетки, до которых юнит может дойти
        /// </summary>
        public static HashSet<GridStat> FindReachableGridsWithBreadthFirstSearch(Unit unit)
        {
            GridStat startGrid = unit.CurrentGrid;
            startGrid.visited = 0;
            GridStat currentGrid = startGrid;
            List<GridStat> childrenOfCurrentGrid;

            HashSet<GridStat> checkedList = new();
            Queue<GridStat> checkQueue = new();


            checkQueue.Enqueue(startGrid);


            void MarkGridsAsNextIterationAndAddToQueue(List<GridStat> grids)
            {
                int visitedIndex = currentGrid.visited + 1;

                foreach (GridStat grid in grids)
                {
                    // На этой клетке ещё никто не побывал
                    if (grid.visited == -1)
                    {
                        grid.visited = visitedIndex;
                        grid.ParentGrid = currentGrid;
                    }
                    checkQueue.Enqueue(grid);
                }
            }

            // Пока у юнита хватает дистанции
            while (checkQueue.Count != 0)

            //TODO: Что будет, если игрок нажмет переместиться на клетку 0?
            {
                currentGrid = checkQueue.Dequeue();

                // Пропускаем то, что уже было проверено, клетки, шаг которых больше, чем доступно юниту, просто достаем из очереди
                if (checkedList.Contains(currentGrid))
                    continue;

                else
                {
                    // Добавить в список проверенных
                    checkedList.Add(currentGrid);

                    // Получить дочерние клетки
                    childrenOfCurrentGrid = GetUnsteppedGridsNearbyWithLimit(currentGrid, startGrid, unit.MoveDistance);
                    // Назначить клеткам родителя и добавить в очередь
                    if (currentGrid.visited != unit.MoveDistance)
                        MarkGridsAsNextIterationAndAddToQueue(childrenOfCurrentGrid);
                }
            }

            return checkedList;

        }

        /// <summary>
        /// Получить доступные для шага ещё не учтенные клетки
        /// Приоритет отдается не-диагональным клеткам
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private static List<GridStat> GetUnsteppedGridsNearbyWithLimit(GridStat grid, GridStat limitedZoneCenter, int limit)
        {
            int topOffset = limitedZoneCenter.z + limit;
            int bootomOffset = limitedZoneCenter.z - limit;
            int leftOffset = limitedZoneCenter.x - limit;
            int rightOffset = limitedZoneCenter.x + limit;

            // Временное решение, придает круглую форму доступной области
            bool isGridInsideRoundZone(GridStat grid)
            {
                if (grid.x == limitedZoneCenter.x)
                    return true;
                if (grid.z == limitedZoneCenter.z)
                    return true;
                if (grid.x < rightOffset && grid.x > leftOffset && grid.z > bootomOffset & grid.z < topOffset)
                    return true;
                return false;
            }

            List<GridStat> gridsNearby = new();
            GridStat currentGrid;


            for (int direction = 2; direction < 9; direction += 2)
            {
                currentGrid = TryGetGridByDirection(grid.x, grid.z, direction);
                if (currentGrid != null && isGridInsideRoundZone(currentGrid))
                    gridsNearby.Add(currentGrid);
            }

            for (int direction = 1; direction < 9; direction += 2)
            {
                currentGrid = TryGetGridByDirection(grid.x, grid.z, direction);
                if (currentGrid != null && isGridInsideRoundZone(currentGrid))
                    gridsNearby.Add(currentGrid);
            }

            return gridsNearby;
        }




    }
}