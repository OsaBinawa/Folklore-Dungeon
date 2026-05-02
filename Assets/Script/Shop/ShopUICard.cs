using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUICard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private Weapon weapon;
    private Buff buff;
    //private ShopItemType type;

    public void SetupWeapon(Weapon w, System.Action onBuy)
    {
        //type = ShopItemType.Weapon;
        weapon = w;

        nameText.text = w.weapon.name;
        priceText.text = w.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy());
    }

    public void SetupBuff(Buff b, System.Action onBuy)
    {
        //type = ShopItemType.Buff;
        buff = b;

        nameText.text = b.buff.name;
        priceText.text = b.price.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy());
    }
}
