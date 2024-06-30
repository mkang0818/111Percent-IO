using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalStat
{
    public int Lv = 0;
    public string Name = "";
    public int HP = 0;
    public int At = 0;
    public int Speed = 0;
    public int MaxExp = 0;
    public int CurExp = 0;
}

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public AnimalStat playerstat;

    private VariableJoystick joy;
    public float speed;

    Rigidbody rigid;
    Animator anim;
    Vector3 moveVec;

    void Start()
    {
        joy = GameManager.Instance.Joystick;
        playerstat = new AnimalStat();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    private void LateUpdate()
    {
        //애니메이션
        print("현재 레벨 : " + playerstat.Lv);
        print("현재 이름 : " + playerstat.Name);
        print("현재 체력 : " + playerstat.HP);
        print("현재 공격력 : " + playerstat.At);
        //print(stat.MaxExp);
        //print(stat.CurExp);
    }
    void Movement()
    {
        float x = joy.Horizontal;
        float y = joy.Vertical;

        // 이동 방향 벡터를 계산
        Vector3 inputDirection = new Vector3(x, 0, y).normalized;

        // 조이스틱이 떼어진 경우나 입력값이 없을 때
        if (inputDirection == Vector3.zero)
        {
            // 기존 방향으로 계속 이동
            rigid.MovePosition(rigid.position + moveVec);
        }
        else
        {
            // 새로운 이동 방향 벡터 계산
            moveVec = inputDirection * playerstat.Speed * Time.deltaTime;

            // 캐릭터를 이동 방향으로 이동
            rigid.MovePosition(rigid.position + moveVec);

            // 회전 방향 계산
            Quaternion dirQuat = Quaternion.LookRotation(moveVec);
            Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);

            // 캐릭터 회전
            rigid.MoveRotation(moveQuat);
        }
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("PreyAni"))
        {
            Vibration.Vibrate(1000);
            print("충돌");
            aa(col.gameObject);
        }
    }

    void aa(GameObject col)
    {
        PreyAnimal prey = col.gameObject.GetComponent<PreyAnimal>();
        int Lv = prey.stat.Lv;

        // 같은 종류 동물과 충돌 시
        if (Lv == playerstat.Lv)
        {
            // 아군으로 자리 이동
            print("공격력증가");
            playerstat.At += prey.stat.At;
        }
        //다른 종류 동물과 충돌 시
        else
        {
            if (prey.stat.At > playerstat.At)
            {
                print("체력 감소");
            }
            else if (prey.stat.At < playerstat.At)
            {
                print("경험치 증가");
            }
        }

        Destroy(col.gameObject);
    }
}