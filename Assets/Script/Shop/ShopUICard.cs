using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopUICard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Image img;
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
        img.sprite = w.weapon.icon;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy());
    }

    public void SetupBuff(Buff b, System.Action onBuy)
    {
        //type = ShopItemType.Buff;
        buff = b;

        nameText.text = b.Item.name;
        priceText.text = b.price.ToString();
        img.sprite = b.Item.Sprite;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy());
    }
}
