using UnityEngine;
using UnityEngine.InputSystem;

namespace Mecha
{
    public class Testing : MonoBehaviour
    {
        private Grid m_Grid;
        void Start()
        {
            m_Grid = new(4, 4, 0.5f, new Vector3(-2, 0, -2));
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log(Utility.GetMouseWorldPosition().ToString());
                //m_Grid.SetValue(Utility.GetMouseWorldPosition(), 99);
            }
        }
    }
}