using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : MonoBehaviour
{
    Rigidbody rigid;

    // 참조할 부모 오브젝트 (플레이어 오브젝트)
    private GameObject playerObject;
    private PlayerController playerController;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();

        // 부모 오브젝트 설정
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();

        // 부모 오브젝트 스탯 초기화
    }

    private void OnCollisionEnter(Collision col)
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
        print("플레이어 레벨 : "+playerController.playerstat.Lv);


        // 같은 종류 동물과 충돌 시
        if (Lv == playerController.playerstat.Lv)
        {
            // 아군으로 자리 이동
            print("공격력증가");
            playerController.playerstat.At += prey.stat.At;

            //따라다니기
            prey.state = State.Follow;
            playerController.AllyList.Add(prey.gameObject);

            prey.gameObject.tag = "Player";

            int index = playerController.AllyList.Count;
            print(index);
            prey.target = playerController.FollowPos[index];
        }
        //강한 동물과 충돌 시
        else if (Lv > playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("체력 감소");
                Destroy(col.gameObject);

            }
            else if (prey.stat.At < playerController.playerstat.At)
            {
                print("다른 동물로 성장");

                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //약한 동물과 충돌 시
        else if (Lv < playerController.playerstat.Lv) 
        {
            // 체력 회복
            print("체력회복");
        }
    }
}
