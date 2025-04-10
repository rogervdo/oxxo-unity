using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    public InputField inputNickname;
    public InputField inputPassword;
    public Text txtMensajeError;

    private void Start()
    {
        PlayerPrefs.DeleteKey("id_usuario");
        int id = PlayerPrefs.GetInt("id_usuario", -1);
        if (id != -1)
        {
            Debug.Log("Usuario ya logueado. ID: " + id);
            SceneManager.LoadScene("Menu");
        }
    }

    public void OnLoginButtonClick()
    {
        string nickname = inputNickname.text;
        string password = inputPassword.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password))
        {
            MostrarMensaje("Por favor ingresa tus datos.");
            return;
        }

        APIManagerLogin.Instance.IniciarLogin(nickname, password, (exitoso) =>
        {
            if (exitoso)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                MostrarMensaje("Usuario o contraseña incorrectos.");
            }
        });
    }

    void MostrarMensaje(string mensaje)
    {
        if (txtMensajeError != null)
        {
            txtMensajeError.text = mensaje;
            txtMensajeError.gameObject.SetActive(true);
        }
    }
}