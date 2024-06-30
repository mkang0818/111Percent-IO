using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIManager : MonoBehaviour
{
    [System.Serializable]
    private class UIObj
    {
        public Slider hpBar;
    }


    [SerializeField] private UIObj UIS;
}