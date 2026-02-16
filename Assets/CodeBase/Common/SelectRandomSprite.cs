using UnityEngine;

namespace Common
{
    public class SelectRandomSprite : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_SRenderer;
        [SerializeField] private Sprite[] m_Sprites;

        void Start()
        {
            m_SRenderer.sprite = m_Sprites[Random.Range(0, m_Sprites.Length)];

            PolygonCollider2D collider = GetComponent<PolygonCollider2D>();
            if (collider != null)
            {
                GameObject obj = collider.gameObject;
                Destroy(collider);

                obj.AddComponent<PolygonCollider2D>();
            }
        }

    }
}