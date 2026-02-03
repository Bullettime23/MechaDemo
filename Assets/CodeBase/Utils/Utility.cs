using UnityEngine;
using UnityEngine.InputSystem;

public class Utility : MonoBehaviour
{
    public static TextMesh CreateWorldText(
        Transform parent,
        string text,
        Vector3 localPositoin,
        Vector3 rotateAround,
        int fontSize,
        Color color,
        TextAnchor textAnchor,
        TextAlignment alignment,
        int sortingOrder
        )
    {
        GameObject gameObj = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObj.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPositoin;
        gameObj.transform.Rotate(rotateAround, 90f);
        TextMesh textMesh = gameObj.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = alignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    public static Vector3 GetMouseWorldPosition()
    {

        Vector3 vector = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        return vector;
    }

    //public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
    //    Vector3 worldPosion = worldCamera.ScreenToWorldPoint(screenPosition);
    //    return worldPosion;
    //}
}
