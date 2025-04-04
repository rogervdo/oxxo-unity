using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifeTime = 1.0f; // Default to 1 second

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}