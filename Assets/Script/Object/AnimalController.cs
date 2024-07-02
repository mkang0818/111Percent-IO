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
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At);
        anim.SetBool("IsRun", true);
    }
    private void OnTriggerEnter(Collider col)
    {
        // 먹이 오브젝트와 충돌 시
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000);
            AnimalCollision(col.gameObject);
        }
    }

    // 동물 오브젝트 충돌 시 
    void AnimalCollision(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;
        int playerlv = playerController.playerstat.Lv;

        // 같은 종류 동물과 충돌 시
        if (Lv == playerController.playerstat.Lv && playerController.playerstat.At >= prey.stat.At)
        {
            // 아군으로 자리 이동
            print("공격력증가");
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);
            
            playerController.playerstat.At += prey.stat.At;

            GameManager.Instance.Score += Lv;
            ValueText(PlusText, Lv);
            prey.gameObject.tag = "Player";

            col.GetComponent<PoolObj>().ReleaseObject();
        }
        //강한 동물과 충돌 시
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("강한 동물한테 공격받음");
                SoundManager.Instance.SoundPlay("Damage", playerController.DamageSound);

                // 공격받을 시 2초 시간 감소
                int minusTime = 2;
                GameManager.Instance.CurTime -= minusTime;

                ValueText(TimerText, minusTime);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= playerController.playerstat.At)
            {
                print("강한 동물 공격 성공");
                SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);

                playerController.playerstat.At += prey.stat.At;
                GameManager.Instance.Score += Lv;

                ValueText(PlusText, Lv);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.Score >= GameManager.Instance.goalScore[prey.stat.Lv])
            {

                print("다음 동물로 성장");
                SoundManager.Instance.SoundPlay("Upgrade", playerController.UpgradeSound);
                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //약한 동물과 충돌 시
        else if (Lv <= playerController.playerstat.Lv)
        {
            print("공격력 증가");
            SoundManager.Instance.SoundPlay("Eat", playerController.EatSound);
            playerController.playerstat.At += prey.stat.At;

            print("점수 획득");
            GameManager.Instance.Score += Lv;

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
        Effect.GetComponent<VFXController>().ReleaseObj();
    }


    // 텍스트 생성
    void ValueText(string TextName, long Value)
    {
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv / 4, playerController.playerstat.Lv / 4);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject TextObj = PoolManager.instance.GetGo(TextName);
        TextObj.transform.position = spawnPos;
        TextObj.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);
        TextObj.GetComponent<TextVFX>().TextAnim();
    }
}
