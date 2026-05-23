using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuffCard : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI buffName;

    public void Setup(BuffSO buff)
    {
        icon.sprite = buff.Icon;
        buffName.text = buff.name;
    }
}
