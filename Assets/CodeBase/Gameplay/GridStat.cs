using System;
using System.Collections.Generic;
using Common;
using TMPro;
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
        public DestructibleBase Desturctible;

        public Cover(CoverDirection direction, CoverType type, DestructibleBase desturctible)
        {
            Direction = direction;
            Type = type;
            Desturctible = desturctible;
        }
    }
    public class GridStat : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
    {
        // Событие при наведении
        public static Action<GridStat> OnGridHover;
        // Событие при нажатии
        public static Action<(GridStat, PointerEventData)> OnGridClick;

        // Знает, какой объект сейчас находится на клетке
        [SerializeField] private GameObject m_ObjectOnGrid;
        public bool IsBusy => m_ObjectOnGrid != null;
        public GameObject ObjectOnGrid => m_ObjectOnGrid;

        // Хранит иконки интерфейса
        [SerializeField] private Canvas m_ClickableField;
        [SerializeField] private Image m_FieldImage;
        // Знает свои координаты
        public int x => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).x;
        public int z => GridToWorldAdapter.LocalCoordinatesToGrid(transform.localPosition).z;
        // Хранит промежуточные значения при рассчете маршрута
        private int _visited = -1;

        // Pathfinding
        public GridStat ParentGrid;
        public int visited
        {
            get { return _visited; }
            set
            {
                text.text = value.ToString();
                _visited = value;
                if (value == -1)
                {
                    text.enabled = false;
                }
                else
                {
                    text.enabled = true;
                }
            }
        }

        [SerializeField] private TextMeshProUGUI text;

        #region Pointer events
        public void OnPointerDown(PointerEventData eventData)
        {
            OnGridClick?.Invoke((this, eventData));
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnGridHover?.Invoke(this);
        }
        #endregion

        #region Unity events

        // Перечень укрытий по направлениям
        private void Awake()
        {
            m_Covers = new Dictionary<CoverDirection, Cover>();
        }
        private void Start()
        {
            text.enabled = false;

            // Тайл должен иметь целочисленное значение координат, иначе будут ошибки
            m_ClickableField.gameObject.SetActive(false);

            if (m_ObjectOnGrid != null)
            {
                GridContent obstacle = m_ObjectOnGrid.GetComponent<GridContent>();
                if (obstacle != null)
                {
                    GridBehaviour.Instance.SetGridsAroundAsCover(this, obstacle);
                    obstacle.EventOnDeath.AddListener(CoverWasDestroyed);
                }

                if (m_ObjectOnGrid.GetComponent<Unit>() && m_Covers.Count > 0)
                {
                    foreach (Cover cover in m_Covers.Values)
                    {
                        m_ObjectOnGrid.GetComponent<Unit>().CoverTook = cover;
                    }
                }
            }
        }

        #endregion

        #region Public API
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
            // TODO: неплохо бы вынести цвета интерфейса в какой-нибудь ScriptableObject
            m_FieldImage.color = Color.white;
            foreach (Cover cover in m_Covers.Values)
            {
                m_CoverIndicators[(int)cover.Direction - 1].color = Color.white;
            }
        }

        public void ShowFieldInterfaceCovers()
        {
            foreach (Cover cover in m_Covers.Values)
            {
                m_CoverIndicators[(int)cover.Direction - 1].color = Color.white;
            }
        }

        public void HideFieldInterface()
        {
            m_FieldImage.color = new Color(0, 0, 0, 0);
            foreach (Image coverImage in m_CoverIndicators)
            {
                coverImage.color = new Color(0, 0, 0, 0);
            }
        }
        public void PlaceObjectToGrid(GameObject go)
        {
            m_ObjectOnGrid = go;

            if (m_Covers.Count != 0 && go.GetComponent<Unit>())
            {
                foreach (Cover cover in m_Covers.Values)
                {
                    go.GetComponent<Unit>().CoverTook = cover;
                }
            }

            go.transform.position = new Vector3(transform.position.x, go.transform.position.y, transform.position.z);
        }

        public void RemoveObjectFromGrid()
        {
            m_ObjectOnGrid = null;
        }

        #endregion

        #region Cover Functionality

        [Header("Resources")]
        [SerializeField] private Sprite m_SpriteCoverFull;
        [SerializeField] private Sprite m_SpriteCoverHalf;

        [SerializeField] private Image[] m_CoverIndicators;



        private Dictionary<CoverDirection, Cover> m_Covers;
        public Dictionary<CoverDirection, Cover> Covers => m_Covers;

        public void AddCover(Cover cover)
        {
            m_Covers.TryAdd(cover.Direction, cover);
            m_CoverIndicators[(int)cover.Direction - 1].sprite =
                cover.Type == CoverType.FullCover ? m_SpriteCoverFull : m_SpriteCoverHalf;
        }

        public void RemoveCover(CoverDirection direction)
        {
            m_Covers.Remove(direction);
        }

        public void ResetPath()
        {
            visited = -1;
            ParentGrid = null;
        }

        private void CoverWasDestroyed(DestructibleBase destructible)
        {
            RemoveObjectFromGrid();
            GridBehaviour.Instance.RemoveCoverFromGridsAround(this);
            destructible.EventOnDeath.RemoveListener(CoverWasDestroyed);
        }
        #endregion



#if UNITY_EDITOR
        private Color m_GizmosColor = new Color(1f, 0, 0, 0.6f);
        private void OnDrawGizmos()
        {
            Gizmos.color = m_GizmosColor;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 6, Vector3.one * 6);
        }
#endif
    }
}