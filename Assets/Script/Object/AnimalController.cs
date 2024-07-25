using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimalController : MonoBehaviour
{
    public TextMeshProUGUI attackText;

    // 참조할 부모 오브젝트 (플레이어 오브젝트)
    private GameObject playerObject;
    private PlayerController _playerController;

    Animator anim;
    string eatVFX = "EatVFX";
    string plusText = "PlusText";
    string timerText = "TimerText";

    void Start()
    {
        // 부모 오브젝트 설정
        playerObject = transform.parent.gameObject;
        _playerController = playerObject.GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        attackText.text = GameManager.Instance.FormatNumber(_playerController.PlayerStat.attack); // 공격력 텍스트 업데이트
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
        if (Lv == _playerController.PlayerStat.lv)
        {
            SoundManager.Instance.SoundPlay("Eat", _playerController.eatSound);

            _playerController.PlayerStat.attack += prey.stat.At; // 공격력 증가
            GameManager.Instance.curScore += Lv; // 점수 획득
            ValueText(plusText, Lv); // 텍스트 이펙트 생성

            col.GetComponent<PoolObj>().ReleaseObject(); // 오브젝트 풀 반환
        }
        // 높은 레벨 동물과 충돌
        else if (Lv >= _playerController.PlayerStat.lv)
        {
            if (prey.stat.At > _playerController.PlayerStat.attack) // 높은 레벨보다 약할 때
            {
                SoundManager.Instance.SoundPlay("Damage", _playerController.damageSound);

                // 공격받을 시 2초 시간 감소
                int minusTime = 2;
                GameManager.Instance.curTime -= minusTime;

                // 이펙트 생성 및 크기 설정
                ValueText(timerText, minusTime);
                Setsize(prey.stat.Lv, eatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (prey.stat.At <= _playerController.PlayerStat.attack)  // 높은 레벨보다 강할 때
            {
                SoundManager.Instance.SoundPlay("Eat", _playerController.eatSound);

                _playerController.PlayerStat.attack += prey.stat.At; // 공격력 증가
                GameManager.Instance.curScore += Lv;

                // 이펙트 생성 및 크기 설정
                ValueText(plusText, Lv);
                Setsize(prey.stat.Lv, eatVFX);
                col.GetComponent<PoolObj>().ReleaseObject();
            }
            else if (GameManager.Instance.curScore >= GameManager.Instance.goalScore[prey.stat.Lv])  // 높은 레벨보다 약할 때
            {
                //????????????????????
                print("다음 동물로 성장");
                SoundManager.Instance.SoundPlay("Upgrade", _playerController.upgradeSound);
                _playerController.Upgrade();
                Destroy(col.gameObject);
            }
        }
        // 낮은 레벨 동물과 충돌
        else if (Lv <= _playerController.PlayerStat.lv)
        {
            SoundManager.Instance.SoundPlay("Eat", _playerController.eatSound);

            _playerController.PlayerStat.attack += prey.stat.At; // 공격력 증가
            GameManager.Instance.curScore += Lv; // 점수 획득

            // 이펙트 생성 및 크기 설정
            ValueText(plusText, Lv);
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
        Vector3 playerPos = _playerController.transform.position;
        int Xrand = Random.Range(-_playerController.PlayerStat.lv / 4, _playerController.PlayerStat.lv / 4);
        Vector3 spawnPos = new Vector3(playerPos.x + Xrand, playerPos.y + _playerController.PlayerStat.lv, playerPos.z);

        // 이펙트 오브젝트 풀 활성화
        GameObject TextObj = PoolManager.instance.GetGo(TextName);

        // 이펙트 좌표, Text 설정
        TextObj.transform.position = spawnPos;
        TextObj.GetComponent<TextVFX>().Value = GameManager.Instance.FormatNumber(Value);
        TextObj.GetComponent<TextVFX>().TextAnim();
    }
}
