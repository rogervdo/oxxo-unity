using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Cuanto más bajo está en Y, mayor el sorting order (más al frente)
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}

