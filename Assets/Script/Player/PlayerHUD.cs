using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    private PlayerRunData runData;
    private void OnEnable()
    {
        StartCoroutine(WaitForRunManager());
    }

    private IEnumerator WaitForRunManager()
    {
        while (RunManager.Instance == null)
            yield return null;

        runData = RunManager.Instance.Player;

        while (runData == null)
            yield return null;

        hpBar.maxValue = runData.MaxHP;
        hpBar.value = runData.CurrentHP;

        runData.OnHPChanged += HandleHPChanged;
    }


    private void OnDisable()
    {
        if (runData != null)
            runData.OnHPChanged -= HandleHPChanged;
    }

    private void HandleHPChanged(int current, int max)
    {
        hpBar.maxValue = max;
        hpBar.value = current;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
//Buat display Global UI, HPbar, gold, etc