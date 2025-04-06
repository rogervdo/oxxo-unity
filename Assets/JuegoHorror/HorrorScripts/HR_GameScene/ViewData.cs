using UnityEngine;

public class ViewData : MonoBehaviour
{
    public int viewID;
    public Vector3 viewPosition;

    void Awake()
    {
        viewPosition = transform.position;
    }
}
