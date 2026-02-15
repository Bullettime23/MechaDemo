using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mecha
{
    public enum CoverDirection
    {
        Top = 1,
        Right = 2,
        Bottom = 3,
        Left = 4,
    }

    public enum CoverType
    {
        HalfCover = 1,
        FullCover = 2,
    }

    public class Cover
    {
        public CoverDirection Direction;
        public CoverType Type;

        public Cover(CoverDirection direction, CoverType type)
        {
            Direction = direction;
            Type = type;
        }
    }
    public class GridStat : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
    {
        public static Action<GridStat> OnGridHover;
        public static Action<GridStat> OnGridClick;

        [SerializeField] private GameObject m_ObjectOnGrid;
        public bool IsBusy => m_ObjectOnGrid != null;
        public GameObject ObjectOnGrid => m_ObjectOnGrid;

        [SerializeField] private Canvas m_ClickableField;
        [SerializeField] private Image m_FieldImage;
        public int x => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).x;
        public int z => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).z;
        public int visited = -1;


        #region Pointer events
        public void OnPointerDown(PointerEventData eventData)
        {
            GridBehaviour.Instance.OnGridClick(this);
            OnGridClick?.Invoke(this);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnGridHover?.Invoke(this);
        }
        #endregion

        private void Awake()
        {
            m_Covers = new Dictionary<CoverDirection, Cover>();
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

        public void ShowFieldInterface()
        {
            m_FieldImage.color = GridBehaviour.Instance.SelectedColor;
            foreach (Cover cover in m_Covers.Values)
            {
                m_CoverIndicators[(int)cover.Direction - 1].color = GridBehaviour.Instance.SelectedColor;
            }
        }

        public void HideFieldInterface()
        {
            m_FieldImage.color = GridBehaviour.Instance.DefaultColor;
            foreach (Image coverImage in m_CoverIndicators)
            {
                coverImage.color = GridBehaviour.Instance.DefaultColor;
            }
        }

        #region Cover Functionality

        [Header("Resources")]
        [SerializeField] private Sprite m_SpriteCoverFull;
        [SerializeField] private Sprite m_SpriteCoverHalf;

        [SerializeField] private Image[] m_CoverIndicators;



        private Dictionary<CoverDirection, Cover> m_Covers;
        public Dictionary<CoverDirection, Cover> Covers => m_Covers;

        public void AddCover(Cover cover)
        {
            // Могут ли клетки занятые препятствиями нести функцию укрытия?
            //if (type != GridType.FullObstacle && type != GridType.HalfObstacle)
            m_Covers.Add(cover.Direction, cover);
            m_CoverIndicators[(int)cover.Direction - 1].sprite =
                cover.Type == CoverType.FullCover ? m_SpriteCoverFull : m_SpriteCoverHalf;
        }

        public void RemoveCover(Cover cover)
        {
            m_Covers.Remove(cover.Direction);
        }

        public void PlaceObjectToGrid(GameObject go)
        {
            m_ObjectOnGrid = go;
            go.transform.position = new Vector3(transform.position.x, go.transform.position.y, transform.position.z);
        }

        public void RemoveObjectFromGrid()
        {
            m_ObjectOnGrid = null;
        }

        public Dictionary<CoverDirection, Cover> TakeCover()
        {
            return m_Covers;
        }
        #endregion

    }
}