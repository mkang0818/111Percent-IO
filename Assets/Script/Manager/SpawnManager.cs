using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public string[] PoolObjText; // 풀 오브젝트 텍스트
    public float[] spawnTime;    // 스폰 시간
    public int[] SpawnPosSize;   // 스폰 거리

    private int lv;

    private void Start()
    {
        GameManager.Instance.Spawnmanager = GetComponent<SpawnManager>();
    }

    void Update()
    {
        if (GameManager.Instance.IsStart)
        {
            lv = GameManager.Instance.PlayerLv - 1;
        }
    }

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
            if (!GameManager.Instance.IsStart) break;
            spawnSetting(lv);
            yield return new WaitForSeconds(spawnTime[lv]);
        }
    }

    // 2번째 동물 스폰
    IEnumerator spawnPrey2()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (!GameManager.Instance.IsStart) break;
            if (lv + 1 < 15) spawnSetting(lv + 1);
            else break;
            yield return new WaitForSeconds(spawnTime[lv+1]);
        }
    }
    void spawnSetting(int index)
    {
        var preyObj = PoolManager.instance.GetGo(PoolObjText[index]);

        // 생성한 동물 스폰좌표 설정
        float Xpos = Random.Range(-SpawnPosSize[index], SpawnPosSize[index]);
        float Zpos = Random.Range(-SpawnPosSize[index], SpawnPosSize[index]);
        Vector3 spawnPos = new Vector3(preyObj.transform.position.x + Xpos, 0.1f, preyObj.transform.position.z + Zpos);
        preyObj.transform.position = spawnPos;

        // 생성한 동물 스탯 적용
        string SheetTxt = GameManager.Instance.StatData;
        PreyAnimal prey = preyObj.GetComponent<PreyAnimal>();
        GameManager.Instance.PreyInitStat(SheetTxt, prey.AniCode, prey);

        preyObj.GetComponent<PreyAnimal>().componentInit();
    }
}
