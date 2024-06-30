using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public string[] EnemyText;
    public float[] spawnTime;
    public int[] SpawnPosSize;

    private int lv;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("spawnPrey");
    }

    // Update is called once per frame
    void Update()
    {
        lv = GameManager.Instance.PlayerLv - 1;
    }
    IEnumerator spawnPrey()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            // 동물 생성
            int rand = Random.Range(lv, lv + 2);
            //코끼리일때 배열 벗어남
            var preyObj = PoolManager.instance.GetGo(EnemyText[rand]);
            //print(preyObj);

            // 생성한 동물 스폰좌표 설정
            float Xpos = Random.Range(-SpawnPosSize[lv], SpawnPosSize[lv]);
            float Zpos = Random.Range(-SpawnPosSize[lv], SpawnPosSize[lv]);
            Vector3 spawnPos = new Vector3(preyObj.transform.position.x + Xpos, 0.1f, preyObj.transform.position.z + Zpos);
            preyObj.transform.position = spawnPos;

            // 생성한 동물 스탯 적용
            string SheetTxt = GameManager.Instance.StatData;
            PreyAnimal prey = preyObj.GetComponent<PreyAnimal>();
            GameManager.Instance.PreyInitStat(SheetTxt, prey.AniCode, prey);

            //print("생성");
            //print(EnemyText[rand] +":"+ spawnTime[rand]);
            yield return new WaitForSeconds(spawnTime[rand]);
        }
    }
}
