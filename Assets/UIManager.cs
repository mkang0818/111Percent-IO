using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIManager : MonoBehaviour
{

    [System.Serializable]
    private class MainUI
    {
        public Slider hpBar;
    }
    [SerializeField] private MainUI mainUI;


    [System.Serializable]
    private class GameUI
    {
        public Slider hpBar;
    }
    [SerializeField] private GameUI gameUI;


    private PlayerController player;


    private void Update()
    {
        player = GameManager.Instance.player;
        if(player != null) gameUI.hpBar.value = (float)player.playerstat.CurHP / (float)player.playerstat.MaxHP;
        //print(gameUI.hpBar.value);
    }

    public void GameStart()
    {
        
    }

}