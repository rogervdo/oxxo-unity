using UnityEngine;
using UnityEngine.EventSystems;

public class UIBringToFront : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.SetAsLastSibling(); // Trae este objeto al frente en el Canvas
    }
}
