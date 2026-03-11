using UnityEngine;

namespace Mecha
{
    public static class GridToWorldAdapter
    {
        public static int GridScale;
        public static Vector3 LeftBottomPosition;

        public static (int x, int y, int z) PositionToGridCoordinates(Vector3 position)
        {
            Vector3 calculatedVector = (position / GridScale - LeftBottomPosition);
            return ((int)calculatedVector.x, (int)calculatedVector.y, (int)calculatedVector.z);
        }

        public static (int x, int y, int z) LocalCoordinatesToGrid(Vector3 local)
        {
            Vector3 calculated = local - LeftBottomPosition;
            return ((int)calculated.x, (int)calculated.y, (int)calculated.z);
        }
    }
}