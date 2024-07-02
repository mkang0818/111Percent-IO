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

    public float wanderRadius = 50f; // 랜덤 이동 반경
    public float wanderTimer = 3; // 목적지 변경 주기

    private NavMeshAgent agent;
    private float timer;
    [HideInInspector] public Transform target;

    public GameObject RoundVFX;
    public State state;
    private void Start()
    {
        //componentInit();
    }

    // 컴포넌트 초기화
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
        //print("먹잇감 레벨 : "+stat.Lv);
        //print(stat.Name);
        //print(stat.HP);
        //print(stat.At);
        //print(stat.MaxExp);
        //print(stat.CurExp);

        if(GameManager.Instance.IsStart) StateMotion();
    }

    // 상태 구현
    void StateMotion()
    {
        switch (state)
        {
            case State.Move:
                //print("이동 중");
                Movement();
                texta();
                break;

            case State.Follow:
                AtText.text = "";
                follow();
                break;
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

    // 플레이어 
    private void follow()
    {
        anim.SetBool("IsRun",true);

        agent.isStopped = true;
        // 타겟 방향 계산
        Vector3 direction = (target.position - transform.position).normalized;

        // 이동 벡터 계산
        Vector3 moveVector = direction * stat.Speed * Time.deltaTime;

        // 새로운 위치 계산
        Vector3 newPosition = transform.position + moveVector;

        // 오브젝트를 새로운 위치로 이동
        transform.position = newPosition;

        // 타겟 방향으로 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5.5f);

        rigid.isKinematic = true;
    }

    // 자동 이동 구현
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