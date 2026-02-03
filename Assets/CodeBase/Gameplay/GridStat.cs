using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mecha
{
    public class GridStat : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
    {
        public static Action<GridStat> OnGridHover;

        public enum GridType
        {
            None = 0,
            HalfObstacle = 1,
            FullObstacle = 2,
            Busy = 3,
        }
        public GridType type;

        [SerializeField] private Canvas m_ClickableField;
        public int x => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).x;
        public int z => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).z;
        public int visited = -1;

        #region Pointer events
        public void OnPointerDown(PointerEventData eventData)
        {
            GridBehaviour.Instance.OnGridClick(this);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnGridHover?.Invoke(this);
        }
        #endregion

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