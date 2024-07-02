using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;


[System.Serializable]
public class PreyStat
{
    public int Lv = 1;
    public string Name = "";
    public long At = 0;
    public int Speed = 0;
}

public class PreyAnimal : MonoBehaviour
{
    public TextMeshProUGUI AtText;

    [HideInInspector] public Transform target;
    [HideInInspector]
    public PreyStat stat;
    public string AniCode;

    public float FindRadius = 50f; // 랜덤 이동 반경
    public float FindTimer = 3; // 목적지 변경 주기

    private NavMeshAgent agent;
    private float timer;


    // 컴포넌트 초기화
    public void componentInit()
    {
        tag = "PreyAni";
        target = GameManager.Instance.player.gameObject.transform;

        agent = GetComponent<NavMeshAgent>();

        agent.speed = stat.Speed * 0.3f;
        timer = FindTimer;
    }

    void Update()
    {
        if (GameManager.Instance.IsStart)
        {
            Movement();
            texta();
        }
    }


    // 공격력 텍스트 업데이트
    void texta()
    {
        long playerAt = target.gameObject.GetComponent<PlayerController>().playerstat.At;
        if (stat.At < playerAt)
        {
            AtText.color = Color.green;
        }
        else if (stat.At > playerAt)
        {
            AtText.color = Color.red;
        }
        else
        {
            AtText.color = Color.white;
        }
        AtText.text = GameManager.Instance.FormatNumber(stat.At);
    }

    // 자동 이동 구현
    private void Movement()
    {
        timer += Time.deltaTime;

        if (timer >= FindTimer || agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 newPos = RandomNavSphere(transform.position, FindRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    // 무작위 이동 좌표 탐색
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}