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

    public string AniCode; // �ڵ带 ���� ���������Ʈ ���� �ο�

    public float FindRadius = 50f; // ���� �̵� �ݰ�
    public float FindTimer = 3; // ������ ���� �ֱ�
    private float timer;


    // ������Ʈ Ǯ�� Ȱ��ȭ�Ǿ� Ÿ�� �� ������Ʈ �ʱ�ȭ
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
            Movement(); // �ڵ� �̵� �޼���
            AttackText(); // ���ݷ� �ؽ�Ʈ ������Ʈ
        }
    }


    // ���ݷ� �ؽ�Ʈ ������Ʈ
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

    // �ڵ� �̵� ����
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

    // ������ �̵� ��ǥ Ž��
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}