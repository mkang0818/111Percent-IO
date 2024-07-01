using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public string[] EnemyText;
    public float[] spawnTime;
    public int[] SpawnPosSize;

    private int lv;


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsStart)
        {
            lv = GameManager.Instance.PlayerLv - 1;
        }
    }
    public void SpawnStart()
    {
        StartCoroutine("spawnPrey");
    }
    IEnumerator spawnPrey()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            // ���� ����
            int rand = Random.Range(lv, lv + 2);
            //�ڳ����϶� �迭 ���
            var preyObj = PoolManager.instance.GetGo(EnemyText[rand]);
            //print(preyObj);

            // ������ ���� ������ǥ ����
            float Xpos = Random.Range(-SpawnPosSize[lv], SpawnPosSize[lv]);
            float Zpos = Random.Range(-SpawnPosSize[lv], SpawnPosSize[lv]);
            Vector3 spawnPos = new Vector3(preyObj.transform.position.x + Xpos, 0.1f, preyObj.transform.position.z + Zpos);
            preyObj.transform.position = spawnPos;

            // ������ ���� ���� ����
            string SheetTxt = GameManager.Instance.StatData;
            PreyAnimal prey = preyObj.GetComponent<PreyAnimal>();
            GameManager.Instance.PreyInitStat(SheetTxt, prey.AniCode, prey);

            //print("����");
            //print(EnemyText[rand] +":"+ spawnTime[rand]);
            yield return new WaitForSeconds(spawnTime[rand]);
        }
    }
}
