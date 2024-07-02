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
    public float TimeLimit = 0;
    public long At = 0;
    public int Speed = 0;
}
public enum State
{
    Move,
    Follow
}
public class PreyAnimal : MonoBehaviour
{
    public TextMeshProUGUI AtText;

    Animator anim;
    Rigidbody rigid;

    [HideInInspector]
    public PreyStat stat;
    public string AniCode;

    public float wanderRadius = 50f; // ���� �̵� �ݰ�
    public float wanderTimer = 3; // ������ ���� �ֱ�

    private NavMeshAgent agent;
    private float timer;
    [HideInInspector] public Transform target;

    public GameObject RoundVFX;
    public State state;
    private void Start()
    {
        //componentInit();
    }

    // ������Ʈ �ʱ�ȭ
    public void componentInit()
    {
        tag = "PreyAni";
        target = GameManager.Instance.player.gameObject.transform;

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        agent.speed = stat.Speed * 0.3f;
        state = State.Move;
        timer = wanderTimer;
    }

    void Update()
    {
        //print("���հ� ���� : "+stat.Lv);
        //print(stat.Name);
        //print(stat.HP);
        //print(stat.At);
        //print(stat.MaxExp);
        //print(stat.CurExp);

        if(GameManager.Instance.IsStart) StateMotion();
    }

    // ���� ����
    void StateMotion()
    {
        switch (state)
        {
            case State.Move:
                //print("�̵� ��");
                Movement();
                texta();
                break;

            case State.Follow:
                AtText.text = "";
                follow();
                break;
        }
    }

    // ���ݷ� �ؽ�Ʈ ������Ʈ
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

    // �÷��̾� 
    private void follow()
    {
        anim.SetBool("IsRun",true);

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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.5f);

        rigid.isKinematic = true;
    }

    // �ڵ� �̵� ����
    private void Movement()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer || agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
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