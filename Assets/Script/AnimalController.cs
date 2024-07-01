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
    Animator anim;
    public GameObject EatVFX;

    void Start()
    {
        // 부모 오브젝트 설정
        playerObject = transform.parent.gameObject;
        playerController = playerObject.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

        // 부모 오브젝트 스탯 초기화
    }
    private void Update()
    {
        AttackText.text = GameManager.Instance.FormatNumber(playerController.playerstat.At);

        anim.SetBool("IsRun", true);
    }
    private void OnTriggerEnter(Collider col)
    {
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


        // 같은 종류 동물과 충돌 시
        if (Lv == playerController.playerstat.Lv)
        {
            // 아군으로 자리 이동
            print("공격력증가");
            playerController.playerstat.At += prey.stat.At;
            GameManager.Instance.Score += prey.stat.At;
            ValueText(GameManager.Instance.PlusText, prey.stat.At);

            //따라다니기
            prey.state = State.Follow;
            prey.RoundVFX.SetActive(true);

            playerController.AllyList.Add(prey.gameObject);

            prey.gameObject.tag = "Player";

            int index = playerController.AllyList.Count + 1;
            // 인덱스 넘어감 예외처리
            if (FollowPos.Length > index) prey.target = FollowPos[index];
        }
        //강한 동물과 충돌 시
        else if (Lv >= playerController.playerstat.Lv)
        {
            if (prey.stat.At > playerController.playerstat.At)
            {
                print("강한 동물한테 공격받음");

                // 공격받을 시 2초 시간 감소
                int minusTime = 2;
                playerController.playerstat.TimeLimit -= minusTime;

                ValueText(GameManager.Instance.MinusText, minusTime);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= playerController.playerstat.At)
            {
                print("강한 동물 공격 성공");

                playerController.playerstat.At += prey.stat.At;
                GameManager.Instance.Score += prey.stat.At;

                ValueText(GameManager.Instance.PlusText, prey.stat.At);
                Setsize(prey.stat.Lv, EatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.Score >= GameManager.Instance.goalScore[prey.stat.Lv])
            {
                print("다음 동물로 성장");

                playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        //약한 동물과 충돌 시
        else if (Lv <= playerController.playerstat.Lv)
        {
            print("공격력 증가");

            float PlayerAt = playerController.playerstat.At;
            PlayerAt += prey.stat.At;

            print("점수 획득");
            GameManager.Instance.Score += prey.stat.At;

            ValueText(GameManager.Instance.PlusText, prey.stat.Lv);
            Setsize(prey.stat.Lv, EatVFX);
            col.GetComponent<PoolObj>().ReleaseObject();
        }
    }

    // 오브젝트에 따른 사이즈 설정
    void Setsize(int index,GameObject obj)
    {
        GameObject Effect = Instantiate(obj, transform.position,Quaternion.identity);
        Effect.transform.localScale *= GameManager.Instance.AniSize[index-1];
    }


    // 텍스트 생성
    void ValueText(GameObject VFXText, long Value)
    {
        Vector3 playerPos = playerController.transform.position;
        int Xrand = Random.Range(-playerController.playerstat.Lv / 2, playerController.playerstat.Lv / 2);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + playerController.playerstat.Lv, playerPos.z);

        GameObject damageVFX = Instantiate(VFXText, spawnPos, Quaternion.identity);
        damageVFX.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);

        //damageVFX.transform.localScale = new Vector3(playerController.playerstat.Lv, playerController.playerstat.Lv, playerController.playerstat.Lv);
        //Transform canvasTransform = VFXText.transform.Find("Canvas");
        //canvasTransform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.FormatNumber(Value);
    }
}
