using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class PreyStat
{
    public int Lv = 0;
    public string Name = "";
    public int HP = 0;
    public int At = 0;
    public int Speed = 0;
    public int MaxExp = 0;
    public int CurExp = 0;
}
public enum State
{
    Move,
    Find,
    Follow
}
public class PreyAnimal : MonoBehaviour
{
    Animator anim;
    Rigidbody rigid;

    [HideInInspector]
    public PreyStat stat;
    public string AniCode;

    public float wanderRadius = 10f; // ���� �̵� �ݰ�
    public float wanderTimer = 5f; // ������ ���� �ֱ�

    private NavMeshAgent agent;
    private float timer;
    [HideInInspector] public Transform target;

    public State state;
    void Start()
    {
        target = GameManager.Instance.player.gameObject.transform;
        state = State.Move;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stat.Speed;
        timer = wanderTimer;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //print("���հ� ���� : "+stat.Lv);
        //print(stat.Name);
        //print(stat.HP);
        //print(stat.At);
        //print(stat.MaxExp);
        //print(stat.CurExp);

        switch (state)
        {
            case State.Move:
                //print("�̵� ��");
                Movement();
                find();
                break;

            case State.Find:
                //print("�߰߹߰�");
                //agent.SetDestination(target.position);
                find();
                break;

            case State.Follow:
                follow();
                break;
        }
        
    }
    private void follow()
    {
        //print("���󰡱�");
        agent.isStopped = true;
        // Ÿ�� ���� ���
        Vector3 direction = (target.position - transform.position).normalized;

        // �̵� ���� ���
        Vector3 moveVector = direction * stat.Speed * Time.deltaTime;

        // ���ο� ��ġ ���
        Vector3 newPosition = transform.position + moveVector;

        // ������Ʈ�� ���ο� ��ġ�� �̵�
        transform.position = newPosition;

        // Ÿ�� �������� ȸ��
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8);

        rigid.isKinematic = true;
    }
    private void find()
    {
        float Dis = Vector3.Distance(target.position ,transform.position);
        if (Dis < 1)
        {
            state = State.Find;
        }
        else
        {
            state = State.Move;
        }
    }

    private void Movement()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position,1);
    }
}