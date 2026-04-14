using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheat : MonoBehaviour
{
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
