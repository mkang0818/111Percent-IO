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
    Find
}
public class PreyAnimal : MonoBehaviour
{
    [HideInInspector]
    public PreyStat stat;
    public string AniCode;

    public float wanderRadius = 10f; // 랜덤 이동 반경
    public float wanderTimer = 5f; // 목적지 변경 주기

    private NavMeshAgent agent;
    private float timer;
    private Transform target;

    private State state;
    void Start()
    {
        target = GameManager.Instance.player.gameObject.transform;
        state = State.Move;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = stat.Speed;
        timer = wanderTimer;
    }

    void Update()
    {
        print("먹잇감 레벨 : "+stat.Lv);
        //print(stat.Name);
        //print(stat.HP);
        //print(stat.At);
        //print(stat.MaxExp);
        //print(stat.CurExp);
        find();
        switch (state)
        {
            case State.Move:
                //print("이동 중");
                Movement();
                break;

            case State.Find:
                //print("발견발견");
                agent.SetDestination(target.position);
                break;
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