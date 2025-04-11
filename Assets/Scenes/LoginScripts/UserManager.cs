using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public int? CurrentUserId { get; private set; }
     private int idUsuarioActual = -1;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentUserId = null;
        } else {
            Destroy(gameObject);
        }
    }

    public void SetCurrentUser(int userId) {
        CurrentUserId = userId;
    }
    

    public void ClearUser() {
        CurrentUserId = null;
    }

    public void SetCurrentUser2(int id)
    {
        idUsuarioActual = id;
    }

    public int GetCurrentUser2()
    {
        return idUsuarioActual;
    }
}

