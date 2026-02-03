using UnityEngine;
using UnityEngine.EventSystems;

namespace Mecha
{
    public class GridStat : MonoBehaviour, IPointerDownHandler
    {

        public enum GridType
        {
            None = 0,
            HalfObstacle = 1,
            FullObstacle = 2,
        }
        public GridType type;

        [SerializeField] private Canvas m_ClickableField;
        public int x => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).x;
        public int z => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).z;
        public int visited = -1;

        public void OnPointerDown(PointerEventData eventData)
        {
            GridBehaviour.Instance.OnGridClick(this);
        }

        private void Start()
        {
            // Тайл должен иметь целочисленное значение координат, иначе будут ошибки
            m_ClickableField.gameObject.SetActive(false);
        }

        public void EnableField()
        {
            m_ClickableField.gameObject.SetActive(true);
        }

        public void DisableField()
        {
            m_ClickableField.gameObject.SetActive(false);
        }
    }
}