using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public string[] PoolObjText;
    public float[] spawnTime;
    public int[] SpawnPosSize;

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
    IEnumerator spawnPrey1()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (!GameManager.Instance.IsStart) break;
            aa(lv);
            yield return new WaitForSeconds(spawnTime[lv]);
        }
    }
    IEnumerator spawnPrey2()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (!GameManager.Instance.IsStart) break;
            if (lv + 1 < 15) aa(lv + 1);
            else break;
            yield return new WaitForSeconds(spawnTime[lv+1]);
        }
    }
    void aa(int index)
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
