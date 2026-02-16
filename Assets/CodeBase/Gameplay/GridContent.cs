using Common;
using UnityEngine;

namespace Mecha {
    public class GridContent : DestructibleBase
    {
        /// <summary>
        /// Мне нужно знать количество начального здоровья
        /// Тип укрытия
        /// </summary>
        //[SerializeField] private CoverProperties;
        
        public int x => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).x;
        public int z => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).z;

        private GridStat m_UnderGrid;

        private void Start()
        {
            m_UnderGrid = GridBehaviour.Instance.TryGetGrid(transform.position);
            if (m_UnderGrid != null)
            {
                // Назначить клетке объект укрытия

                // Назначить клетками вокруг направление укрытия и тип
                GridBehaviour.Instance.SetGridsAroundAsCover(m_UnderGrid, CoverType.FullCover);
            }
        }

        private void OnCoverDestroyed()
        {
            // Убрать укрытия с соседних клеток
            // Освободить клетку для прохода
        }
    }
}