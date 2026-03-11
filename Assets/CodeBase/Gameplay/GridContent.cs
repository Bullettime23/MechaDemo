using Common;
using UnityEngine;

namespace Mecha
{
    public class GridContent : DestructibleBase
    {
        public int x => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).x;
        public int z => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).z;

        [SerializeField] private CoverType m_CoverType = CoverType.FullCover;
        public CoverType ObstacleCoverType => m_CoverType;
    }
}