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
    public double At = 0;
    public int Speed = 0;
}

public class PreyAnimal : MonoBehaviour
{
    private NavMeshAgent agent;

    public TextMeshProUGUI AtText;
    [HideInInspector] public Transform target;
    [HideInInspector] public PreyStat stat;

    public string AniCode; // 코드를 통해 스프레드시트 스탯 부여

    public float FindRadius = 50f; // 랜덤 이동 반경
    public float FindTimer = 3; // 목적지 변경 주기
    private float timer;


    // 오브젝트 풀로 활성화되어 타겟 및 컴포넌트 초기화
    public void componentInit()
    {
        target = GameManager.Instance.player.gameObject.transform;

        agent = GetComponent<NavMeshAgent>();

        agent.speed = stat.Speed * 0.3f;
        timer = FindTimer;
    }

    void Update()
    {
        if (GameManager.Instance.IsStart)
        {
            Movement(); // 자동 이동 메서드
            AttackText(); // 공격력 텍스트 업데이트
        }
    }


    // 공격력 텍스트 업데이트
    void AttackText()
    {
        double playerAt = target.gameObject.GetComponent<PlayerController>().playerstat.At;
        
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