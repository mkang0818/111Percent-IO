using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public string[] PoolObjText; // 풀 오브젝트 텍스트
    public float[] spawnTime;    // 스폰 시간
    public int[] SpawnPosSize;   // 스폰 거리

    private int PlayerLv; // 현재 플레이어 레벨

    private void Start()
    {
        GameManager.Instance.Spawnmanager = GetComponent<SpawnManager>();
    }

    void Update()
    {
        if (GameManager.Instance.IsStart)
        {
            PlayerLv = GameManager.Instance.PlayerLv - 1;
        }
    }

    // 스폰 활성화
    public void SpawnStart()
    {
        StartCoroutine(spawnPrey1());
        StartCoroutine(spawnPrey2());
    }

    // 1번째 동물 스폰
    IEnumerator spawnPrey1()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (!GameManager.Instance.IsStart) break;  // 게임종료 시 비활성화

            spawnSetting(PlayerLv);

            yield return new WaitForSeconds(spawnTime[PlayerLv]); // 스폰시간만큼 대기
        }
    }

    // 2번째 동물 스폰
    IEnumerator spawnPrey2()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (!GameManager.Instance.IsStart) break; // 게임종료 시 비활성화

            // 마지막 배열 수 예외처리
            if (PlayerLv + 1 < 15) spawnSetting(PlayerLv + 1);
            else break;

            yield return new WaitForSeconds(spawnTime[PlayerLv + 1]);
        }
    }

    // 스폰 생성 및 설정
    void spawnSetting(int index)
    {
        // 오브젝트 풀 활성화
        var preyObj = PoolManager.instance.GetGo(PoolObjText[index]);

        // 생성 오브젝트 스폰 좌표 설정
        float Xpos = Random.Range(-SpawnPosSize[index], SpawnPosSize[index]);
        float Zpos = Random.Range(-SpawnPosSize[index], SpawnPosSize[index]);
        Vector3 spawnPos = new Vector3(preyObj.transform.position.x + Xpos, 0.1f, preyObj.transform.position.z + Zpos);
        preyObj.transform.position = spawnPos;

        // 생성 오브젝트 스탯 적용
        string SheetTxt = GameManager.Instance.StatData;
        PreyAnimal prey = preyObj.GetComponent<PreyAnimal>();
        GameManager.Instance.PreyInitStat(SheetTxt, prey.AniCode, prey);

        // 생성 오브젝트 초기화
        preyObj.GetComponent<PreyAnimal>().componentInit();
    }
}
