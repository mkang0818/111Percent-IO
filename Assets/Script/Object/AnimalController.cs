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

    Animator anim;
    string EatVFX = "EatVFX";
    string PlusText = "PlusText";
    string TimerText = "TimerText";

    void Start()
    {
        // 부모 오브젝트 설정
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At); // 공격력 텍스트 업데이트
        anim.SetBool("IsRun", true); // 애니메이션 작동
    }


    private void OnTriggerEnter(Collider col)
    {
        // Prey오브젝트와 충돌 시
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000); // 1초간 진동
            AnimalCollision(col.gameObject);
        }
    }

    // 충돌 오브젝트 상태에 따라 구현
    void AnimalCollision(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;


        // 1. 같은 레벨과 충돌, 2. 높은 레벨과 충돌, 낮은 레벨과 충돌

        // 같은 종 동물과 충돌
        if (Lv == playerController.playerstat.Lv)
        {
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);
            
            playerController.playerstat.At += prey.stat.At; // 공격력 증가
            GameManager.Instance.Score += Lv; // 점수 획득
            ValueText(PlusText, Lv); // 텍스트 이펙트 생성

            col.GetComponent<PoolObj>().ReleaseObject(); // 오브젝트 풀 반환
        }
        // 높은 레벨 동물과 충돌
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At) // 높은 레벨보다 약할 때
            {
                SoundManager.Instance.SoundPlay("Damage", playerController.DamageSound);

                // 공격받을 시 2초 시간 감소
                int minusTime = 2;
                GameManager.Instance.CurTime -= minusTime;

                // 이펙트 생성 및 크기 설정
                ValueText(TimerText, minusTime);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= playerController.playerstat.At)  // 높은 레벨보다 강할 때
            {
                SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);

                playerController.playerstat.At += prey.stat.At; // 공격력 증가
                GameManager.Instance.Score += Lv;

                // 이펙트 생성 및 크기 설정
                ValueText(PlusText, Lv);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.Score >= GameManager.Instance.goalScore[prey.stat.Lv])  // 높은 레벨보다 약할 때
            {
                //????????????????????
                print("다음 동물로 성장");
                SoundManager.Instance.SoundPlay("Upgrade", playerController.UpgradeSound);
                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        // 낮은 레벨 동물과 충돌
        else if (Lv <= playerController.playerstat.Lv)
        {
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);

            playerController.playerstat.At += prey.stat.At; // 공격력 증가
            GameManager.Instance.Score += Lv; // 점수 획득

            // 이펙트 생성 및 크기 설정
            ValueText(PlusText, Lv);
            Setsize(prey.stat.Lv, EatVFX);
            col.GetComponent<PoolObj>().ReleaseObject();
        }
    }

    // 오브젝트에 따른 사이즈 설정
    void Setsize(int index, string objText)
    {
        GameObject Effect = PoolManager.instance.GetGo(objText);
        float size = index * 0.1f;
        Effect.transform.localScale = new Vector3(size, size, size);
        Effect.transform.position = transform.position;
        Effect.GetComponent<VFXController>().ReleaseObj(); // 풀 오브젝트 반환
    }


    // 텍스트이펙트 생성
    void ValueText(string TextName, double Value)
    {
        // 이펙트 좌표 설정
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv / 4, playerController.playerstat.Lv / 4);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        // 이펙트 오브젝트 풀 활성화
        GameObject TextObj = PoolManager.instance.GetGo(TextName);

        // 이펙트 좌표, Text 설정
        TextObj.transform.position = spawnPos;
        TextObj.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);
        TextObj.GetComponent<TextVFX>().TextAnim();
    }
}
