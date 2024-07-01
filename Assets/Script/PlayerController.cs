using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalStat
{
    public int Lv = 0;
    public string Name = "";
    public float MaxHP = 0;
    public float CurHP = 0;
    public long At = 0;
    public int Speed = 0;
    public int MaxExp = 0;
    public int CurExp = 0;
}

public class PlayerController : MonoBehaviour
{
    public float Range;

    public GameObject[] AnimalArr;

    [HideInInspector]
    public AnimalStat playerstat;

    private VariableJoystick joy;

    Rigidbody rigid;
    Animator anim;
    Vector3 moveVec;

    GameObject CurrentAnimal;
    [HideInInspector] public List<GameObject> AllyList;

    public GameObject UpgradeVFX;
    public GameObject AllyText;
    public GameObject DamageText;
    void Start()
    {
        joy = GameManager.Instance.Joystick;
        playerstat = new AnimalStat();
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

        CurrentAnimal = Instantiate(AnimalArr[0], transform);
        CurrentAnimal.transform.localPosition = Vector3.zero; // 새 오브젝트의 위치를 Animal의 위치로 설정
    }

    // Update is called once per frame
    void Update()
    {
        playerstat.CurHP -= Time.deltaTime / 2;

        Movement();
        LvUp();
        Dead();
    }
    private void LateUpdate()
    {
        //애니메이션
        //print("현재 레벨 : " + playerstat.Lv);
        //print("현재 이름 : " + playerstat.Name);
        //print("현재 체력 : " + playerstat.CurHP);
        //print("현재 공격력 : " + playerstat.At);
        //print(stat.MaxExp);
        //print(stat.CurExp);
    }
    public void Upgrade()
    {
        foreach (GameObject ally in AllyList)
        {
            Destroy(ally);
        }
        AllyList.Clear();
        Destroy(CurrentAnimal);
        int index = playerstat.Lv;
        CurrentAnimal = Instantiate(AnimalArr[index], transform);
        CurrentAnimal.transform.localPosition = Vector3.zero; // 새 오브젝트의 위치를 Animal의 위치로 설정

        //스탯 초기화
        string SheetTxt = GameManager.Instance.StatData;
        print(index+1);
        GameManager.Instance.PlayerInitStat(SheetTxt, (index+1).ToString());

        // 업그레이드 이펙트
        UpgradeVFX.transform.localScale = new Vector3(index, index, index);
        Instantiate(UpgradeVFX,transform);
    }
    void LvUp()
    {
        if (playerstat.CurExp >= playerstat.MaxExp)
        {
            //print("레벨업");
        }
    }
    void Dead()
    {
        if (playerstat.CurHP <= 0)
        {
            print("DEADEADEADADEDAEDADEADAEDAEDEA");
        }
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
}