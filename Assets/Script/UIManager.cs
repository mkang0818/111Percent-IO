using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class UIManager : MonoBehaviour
{

    [System.Serializable]
    private class MainUI
    {
        public GameObject mainUI;
    }
    [SerializeField] private MainUI mainUI;


    [System.Serializable]
    private class GameUI
    {
        public GameObject gameUI;
        public Slider hpBar;
        public TextMeshProUGUI CountDownText;
    }
    [SerializeField] private GameUI gameUI;

    private PlayerController player;

    public SpawnManager spawnmanager;

    private void Update()
    {
        GameHPBar();
    }
    void GameHPBar()
    {
        if (player != null && GameManager.Instance.IsStart)
        {
            player = GameManager.Instance.player;
            if (player != null) gameUI.hpBar.value = (float)player.playerstat.CurHP / (float)player.playerstat.MaxHP;
            //print(gameUI.hpBar.value);
        }
    }

    public void GameStart()
    {
        mainUI.mainUI.SetActive(false);
        gameUI.gameUI.SetActive(true);

        GameManager.Instance.PlayerIns();
        GameManager.Instance.IsStart = true;

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int countdown = 3;

        // 카운트다운 시작
        while (countdown > 0)
        {
            gameUI.CountDownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            gameUI.CountDownText.GetComponent<CountDown>().CountTextOn();
            countdown--;
        }

        gameUI.CountDownText.text = "";
        print("게임시작 !");
        GameManager.Instance.player.PlayerStart = true;
        spawnmanager.SpawnStart();
    }
}