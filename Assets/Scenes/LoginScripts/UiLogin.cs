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
        // Limpia cualquier ID de usuario guardado previamente al inicio.
        PlayerPrefs.DeleteKey("id_usuario");
    }

    // Se llama cuando se hace clic en el botón de login.
    public void OnLoginButtonClick()
    {
        string nickname = inputNickname.text;
        string password = inputPassword.text;

        // Validación básica de entrada.
        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password))
        {
            MostrarMensaje("Por favor ingresa tus datos.");
            return;
        }

        // Llama al API manager para intentar el login.
        APIManagerLogin.Instance.IniciarLogin(nickname, password, (exitoso) =>
        {
            // Maneja el resultado del login.
            if (exitoso)
            {
                SceneManager.LoadScene("Menu"); // Carga menú si éxito.
            }
            else
            {
                MostrarMensaje("Usuario o contraseña incorrectos."); // Muestra error si fallo.
            }
        });
    }

    // Método auxiliar para mostrar mensajes en el campo de texto de error.
    void MostrarMensaje(string mensaje)
    {
        if (txtMensajeError != null)
        {
            txtMensajeError.text = mensaje;
            txtMensajeError.gameObject.SetActive(true);
        }
    }
} 