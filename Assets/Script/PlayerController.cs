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
        CurrentAnimal.transform.localPosition = Vector3.zero; // �� ������Ʈ�� ��ġ�� Animal�� ��ġ�� ����
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
        //�ִϸ��̼�
        //print("���� ���� : " + playerstat.Lv);
        //print("���� �̸� : " + playerstat.Name);
        //print("���� ü�� : " + playerstat.CurHP);
        //print("���� ���ݷ� : " + playerstat.At);
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
        CurrentAnimal.transform.localPosition = Vector3.zero; // �� ������Ʈ�� ��ġ�� Animal�� ��ġ�� ����

        //���� �ʱ�ȭ
        string SheetTxt = GameManager.Instance.StatData;
        print(index+1);
        GameManager.Instance.PlayerInitStat(SheetTxt, (index+1).ToString());

        // ���׷��̵� ����Ʈ
        UpgradeVFX.transform.localScale = new Vector3(index, index, index);
        Instantiate(UpgradeVFX,transform);
    }
    void LvUp()
    {
        if (playerstat.CurExp >= playerstat.MaxExp)
        {
            //print("������");
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

        // �̵� ���� ���͸� ���
        Vector3 inputDirection = new Vector3(x, 0, y).normalized;

        // ���̽�ƽ�� ������ ��쳪 �Է°��� ���� ��
        if (inputDirection == Vector3.zero)
        {
            // ���� �������� ��� �̵�
            rigid.MovePosition(rigid.position + moveVec);
        }
        else
        {
            // ���ο� �̵� ���� ���� ���
            moveVec = inputDirection * playerstat.Speed * Time.deltaTime;

            // ĳ���͸� �̵� �������� �̵�
            rigid.MovePosition(rigid.position + moveVec);

            // ȸ�� ���� ���
            Quaternion dirQuat = Quaternion.LookRotation(moveVec);
            Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);

            // ĳ���� ȸ��
            rigid.MoveRotation(moveQuat);
        }
    }
}