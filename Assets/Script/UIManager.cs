/*using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Slider playerTimer;

    private void Awake()
    {
        Instance = this;
    }

    public void SetTurnTimer(int current, int max)
    {
        playerTimer.maxValue = max;
        playerTimer.value = current;

    }

    public void HideTurnTimer()
    {
        playerTimer.gameObject.SetActive(false);
    }

    public void ShowTurnTimer()
    {
        playerTimer.gameObject.SetActive(true);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
*/