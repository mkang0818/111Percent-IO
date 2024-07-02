using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AnimalStat
{
    public int Lv = 1;
    public string Name = "";
    public float TimeLimit = 0;
    public long At = 0;
    public int Speed = 0;
}

public class PlayerController : MonoBehaviour
{
    public GameObject[] AnimalArr;

    Rigidbody rigid;
    Vector3 moveVec;

    GameObject CurrentAnimal;
    [HideInInspector] public AnimalStat playerstat;
    [HideInInspector] public List<GameObject> AllyList;
    [HideInInspector] public Slider goalSlider;
    private FloatingJoystick joy;

    public GameObject UpgradeVFX;
    string upgradeVFX = "UpgradeVFX";

    public int[] UpgradeTime;
    public bool PlayerStart = false;

    private float MinX = -86.4f;
    private float MaxX = 67f;
    private float MinZ = -90.3f;
    private float MaxZ = 70.9f;

    void Start()
    {
        joy = GameManager.Instance.Joystick;
        playerstat = new AnimalStat();
        rigid = GetComponent<Rigidbody>();

        CurrentAnimal = Instantiate(AnimalArr[0], transform);
        CurrentAnimal.transform.localPosition = Vector3.zero; // �� ������Ʈ�� ��ġ�� Animal�� ��ġ�� ����        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerStart)
        {
            playerstat.TimeLimit -= Time.deltaTime;
            
            Doundary();
            Movement();
            LvUp();
            GameOver();
            Clear();

            goalSlider.value = (float)GameManager.Instance.Score / (float)GameManager.Instance.goalScore[playerstat.Lv - 1];
        }
    }

    // �� ���� �� �̵� ����
    void Doundary()
    {
        Vector3 CurPos = transform.position;

        if (CurPos.x < MinX)
        {
            CurPos.x = MinX;
        }
        else if (CurPos.x > MaxX)
        {
            CurPos.x = MaxX;
        }

        if (CurPos.z < MinZ)
        {
            CurPos.z = MinZ;
        }
        else if (CurPos.z > MaxZ)
        {
            CurPos.z = MaxZ;
        }
        transform.position = CurPos;
    }

    // ���� ���׷��̵�
    void LvUp()
    {
        if (GameManager.Instance.Score >= GameManager.Instance.goalScore[playerstat.Lv - 1])
        {
            //print("���׷��̵�");
            if(playerstat.Lv <= 15) Upgrade();
        }
    }
    // ���� ���� ���׷��̵�
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
        GameManager.Instance.PlayerInitStat(SheetTxt, (index + 1).ToString());

        // ���׷��̵� ����Ʈ
        GameObject UpgradeVFX = PoolManager.instance.GetGo(upgradeVFX);
        UpgradeVFX.transform.localScale = new Vector3(index, index, index);
        UpgradeVFX.transform.position = transform.position;
        UpgradeVFX.GetComponent<VFXController>().ReleaseObj();
    }

    void Clear()
    {
        if (GameManager.Instance.Score >= GameManager.Instance.goalScore[15])
        {
            print("����Ŭ����");
            PlayerStart = false;
            GameManager.Instance.GameOver(false);
            ClearObj();
            Destroy(gameObject);
        }
    }

    // ���� ����
    void GameOver()
    {
        if (playerstat.TimeLimit <= 0)
        {
            print("���ӿ���");
            PlayerStart = false;
            GameManager.Instance.GameOver(true);
            ClearObj();
            Destroy(gameObject);
        }
    }
    void ClearObj()
    {
        foreach (GameObject listObj in AllyList)
        {
            listObj.tag = "PreyAni";
        }

        GameObject[] ClearObjs = GameObject.FindGameObjectsWithTag("PreyAni");

        foreach (GameObject obj in ClearObjs){
            if(obj.GetComponent<PoolObj>()!= null) obj.GetComponent<PoolObj>().ReleaseObject();
        }


    }

    // �÷��̾� �̵� ����
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