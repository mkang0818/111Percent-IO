using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public string[] PoolObjText; // Ǯ ������Ʈ �ؽ�Ʈ
    public float[] spawnTime;    // ���� �ð�
    public int[] SpawnPosSize;   // ���� �Ÿ�

    private int PlayerLv; // ���� �÷��̾� ����

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

    // ���� Ȱ��ȭ
    public void SpawnStart()
    {
        StartCoroutine(spawnPrey1());
        StartCoroutine(spawnPrey2());
    }

    // 1��° ���� ����
    IEnumerator spawnPrey1()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (!GameManager.Instance.IsStart) break;  // �������� �� ��Ȱ��ȭ

            spawnSetting(PlayerLv);

            yield return new WaitForSeconds(spawnTime[PlayerLv]); // �����ð���ŭ ���
        }
    }

    // 2��° ���� ����
    IEnumerator spawnPrey2()
    {
        yield return new WaitForSeconds(2);
        while (true)
        {
            if (!GameManager.Instance.IsStart) break; // �������� �� ��Ȱ��ȭ

            // ������ �迭 �� ����ó��
            if (PlayerLv + 1 < 15) spawnSetting(PlayerLv + 1);
            else break;

            yield return new WaitForSeconds(spawnTime[PlayerLv + 1]);
        }
    }

    // ���� ���� �� ����
    void spawnSetting(int index)
    {
        // ������Ʈ Ǯ Ȱ��ȭ
        var preyObj = PoolManager.instance.GetGo(PoolObjText[index]);

        // ���� ������Ʈ ���� ��ǥ ����
        float Xpos = Random.Range(-SpawnPosSize[index], SpawnPosSize[index]);
        float Zpos = Random.Range(-SpawnPosSize[index], SpawnPosSize[index]);
        Vector3 spawnPos = new Vector3(preyObj.transform.position.x + Xpos, 0.1f, preyObj.transform.position.z + Zpos);
        preyObj.transform.position = spawnPos;

        // ���� ������Ʈ ���� ����
        string SheetTxt = GameManager.Instance.StatData;
        PreyAnimal prey = preyObj.GetComponent<PreyAnimal>();
        GameManager.Instance.PreyInitStat(SheetTxt, prey.AniCode, prey);

        // ���� ������Ʈ �ʱ�ȭ
        preyObj.GetComponent<PreyAnimal>().componentInit();
    }
}
