using UnityEngine;

public class UserManager : MonoBehaviour
{

    public static UserManager Instance { get; private set; }
    // Almacena el ID del usuario actualmente logueado. Null si no hay usuario logueado.
    public int? CurrentUserId { get; private set; }

    void Awake()
    {
        // Implementación del patrón Singleton.
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentUserId = null; 
        } else {
            Destroy(gameObject);
        }
    }

    // Establece el ID del usuario actual.
    public void SetCurrentUser(int userId) {
        CurrentUserId = userId;

    }

    // Limpia el ID del usuario actual (cierra sesión).
    public void ClearUser() {
        CurrentUserId = null;
    }

    public void SetCurrentUser2(int id)
    {
        CurrentUserId = id;
    }

    public int GetCurrentUser2()
    {
        return CurrentUserId ?? -1;
    }

} 