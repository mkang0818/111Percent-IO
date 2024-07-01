using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimalController : MonoBehaviour
{
    public TextMeshProUGUI AttackText;

    // 참조할 부모 오브젝트 (플레이어 오브젝트)
    private GameObject playerObject;
    private PlayerController playerController;

    // 동물의 크기가 커질 수록 FollowPos 또한 늘어나야함
    public Transform[] FollowPos;

    void Start()
    {
        // 부모 오브젝트 설정
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();

        // 부모 오브젝트 스탯 초기화
    }
    private void Update()
    {
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At); 
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000);
            aa(col.gameObject);
        }
    }

    void aa(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;
        print("상대 레벨 : " + Lv);
        print("플레이어 레벨 : " + playerController.playerstat.Lv);


        // 같은 종류 동물과 충돌 시
        if (Lv == playerController.playerstat.Lv)
        {
            // 아군으로 자리 이동
            print("공격력증가");
            playerController.playerstat.At += prey.stat.At;
            AllyTextVFX(prey.stat.At);

            //따라다니기
            prey.state = State.Follow;
            playerController.AllyList.Add(prey.gameObject);

            prey.gameObject.tag = "Player";

            int index = playerController.AllyList.Count + 1;
            print(index);
            prey.target = FollowPos[index];
            // 인덱스 넘어감 예외처리

        }
        //강한 동물과 충돌 시
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("체력 감소");
                playerController.playerstat.CurHP -= prey.stat.Lv;
                DamageTextVFX(playerController.DamageText, prey.stat.Lv);
                Destroy(col.gameObject);
            }
            else if (prey.stat.At < playerController.playerstat.At)
            {
                //print("다른 동물로 성장");
                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //약한 동물과 충돌 시
        else if (Lv <= playerController.playerstat.Lv)
        {
            // 체력 회복
            print("체력회복");
            
            playerController.playerstat.CurHP += prey.stat.Lv;

            DamageTextVFX(playerController.HeelText, prey.stat.Lv);
            Destroy(col.gameObject);
        }
    }

    void DamageTextVFX(GameObject VFXText, float Value)
    {
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv/2, playerController.playerstat.Lv/2);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject damageVFX = Instantiate(VFXText, spawnPos,Quaternion.identity);

        damageVFX.transform.localScale = new Vector3(playerController.playerstat.Lv, playerController.playerstat.Lv, playerController.playerstat.Lv);
        Transform canvasTransform = VFXText.transform.Find("Canvas");
        canvasTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Value.ToString();
    }
    void AllyTextVFX(long Value)
    {
        int Xrand = Random.Range(-playerController.playerstat.Lv / 2, playerController.playerstat.Lv / 2);
        Vector3 playerPos = playerController.transform.position;
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject AllyVFX = Instantiate(playerController.AllyText, spawnPos, Quaternion.identity);

        AllyVFX.transform.localScale = new Vector3(playerController.playerstat.Lv, playerController.playerstat.Lv, playerController.playerstat.Lv);
        Transform canvasTransform = playerController.AllyText.transform.Find("Canvas");
        canvasTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+" + GameManager.Instance.FormatNumber(Value);
    }
}
