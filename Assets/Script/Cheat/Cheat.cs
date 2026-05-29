using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cheat : MonoBehaviour
{
    public GameObject debugButton;

    private void Update()
    {
        if (Keyboard.current.leftCtrlKey.isPressed &&
            Keyboard.current.hKey.wasPressedThisFrame)
        {
            debugButton.SetActive(true);
            Debug.Log("Cheat activated");
        }
    }
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
    
