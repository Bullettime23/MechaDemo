using System.Xml.Serialization;
using UnityEngine;

namespace Mecha
{
    public class Grid
    {
        private int m_Width;
        private int m_Height;
        private float m_CellSize;
        private int[,] m_GridArray;
        //Debug
        private TextMesh[,] m_TextMeshes;

        private Vector3 m_OriginPosition;

        public Grid(int width, int height, float cellSize, Vector3 originPosition)
        {
            m_Width = width;
            m_Height = height;
            m_CellSize = cellSize;
            m_OriginPosition = originPosition;

            m_GridArray = new int[width, height];
            m_TextMeshes = new TextMesh[width, height];


            for (int x = 0; x < m_GridArray.GetLength(0); x++)
            {
                for (int z = 0; z < m_GridArray.GetLength(1); z++)
                {
                    m_TextMeshes[x, z] = Utility.CreateWorldText(
                        null,
                        m_GridArray[x, z].ToString(),
                        GetWorldPosiotion(x, z) + new Vector3(m_CellSize, 0, m_CellSize) * 0.5f,
                        Vector3.right,
                        5,
                        Color.black,
                        TextAnchor.MiddleCenter,
                        TextAlignment.Center,
                        1
                    );
                    Debug.DrawLine(GetWorldPosiotion(x, z), GetWorldPosiotion(x, z + 1), Color.black, 100f);
                    Debug.DrawLine(GetWorldPosiotion(x, z), GetWorldPosiotion(x + 1, z), Color.black, 100f);
                }
            }

            Debug.DrawLine(GetWorldPosiotion(0, m_Height), GetWorldPosiotion(m_Width, m_Height), Color.black, 100f);
            Debug.DrawLine(GetWorldPosiotion(m_Width, 0), GetWorldPosiotion(m_Width, m_Height), Color.black, 100f);
            m_OriginPosition = originPosition;
        }

        private Vector3 GetWorldPosiotion(int x, int z)
        {
            return new Vector3(x, 0, z) * m_CellSize + m_OriginPosition;
        }

        /// <summary>
        /// Converts world postion to Grid position
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        private void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition - m_OriginPosition).x / m_CellSize);
            z = Mathf.FloorToInt((worldPosition - m_OriginPosition).z / m_CellSize);
        }

        public void SetValue(int x, int z, int value)
        {
            if (x >= 0 && z >= 0 && x < m_Width && z < m_Height)
            {
                m_GridArray[x, z] = value;
                m_TextMeshes[x, z].text = m_GridArray[x, z].ToString();
            }
        }

        public void SetValue(Vector3 worldPosition, int value)
        {
            int x, z;
            GetXZ(worldPosition, out x, out z);
            SetValue(x, z, value);
        }

        public int GetValue(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < m_Width && z < m_Height)
            {
                return m_GridArray[x, z];
            }
            else
            {
                return 0;
            }
        }

        public int GetValue(Vector3 worldPosition)
        {
            int x, z;
            GetXZ(worldPosition, out x, out z);
            return GetValue(x, z);
        }
    }
}
