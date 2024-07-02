using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AnimalStat
{
    public int Lv = 1;
    public string Name = "";
    public double At = 0.011f;
    public int Speed = 0;
}

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public AnimalStat playerstat;
    [HideInInspector] public Slider goalSlider;
    private FloatingJoystick joy;
    public GameObject[] AnimalArr;

    Rigidbody rigid;
    Vector3 moveVec;
    GameObject CurrentAnimal;

    public int[] UpgradeTime;
    public bool PlayerStart = false;

    // ���� Ŭ��
    public AudioClip UpgradeSound;
    public AudioClip EatSound;
    public AudioClip DamageSound;
    public GameObject UpgradeVFX;

    // ������Ʈ Ǯ �̸�
    string upgradeVFX = "UpgradeVFX";


    // �� ���ѱ��� ��ǥ
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

    void Update()
    {
        if (PlayerStart)
        {
            GameManager.Instance.CurTime -= Time.deltaTime;

            Movement();
            LvUp();
            GameOver();
            Clear();
            Doundary();

            goalSlider.value = (float)GameManager.Instance.Score / (float)GameManager.Instance.goalScore[playerstat.Lv - 1];
        }
    }

    // �÷��̾� �̵� ����
    void Movement()
    {
        float x = joy.Horizontal;
        float y = joy.Vertical;

        Vector3 inputDirection = new Vector3(x, 0, y).normalized;

        if (inputDirection == Vector3.zero)
        {
            rigid.MovePosition(rigid.position + moveVec);
        }
        else
        {
            moveVec = inputDirection * playerstat.Speed * Time.deltaTime;
            rigid.MovePosition(rigid.position + moveVec);

            Quaternion dirQuat = Quaternion.LookRotation(moveVec);
            Quaternion moveQuat = Quaternion.Slerp(rigid.rotation, dirQuat, 0.3f);

            rigid.MoveRotation(moveQuat);
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
            if (playerstat.Lv <= 15) Upgrade();
        }
    }

    // ���� ���� ���׷��̵�
    public void Upgrade()
    {
        // ���� ������ ������Ʈ ���� �� ���� ������ ������Ʈ ����
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

        // ���׷��̵� ���� �߰��ð� 10��
        GameManager.Instance.CurTime += 10;
    }

    // ���� Ŭ����
    void Clear()
    {
        if (GameManager.Instance.Score >= GameManager.Instance.goalScore[15])
        {
            // ���� �ʱ�ȭ
            PlayerStart = false;
            GameManager.Instance.GameOver(false);
            ClearObj(); // �� �� ������Ʈ Ǯ ��ȯ
            Destroy(gameObject); // �÷��̾� ����
        }
    }

    // ���� ����
    void GameOver()
    {
        if (GameManager.Instance.CurTime <= 0)
        {
            // ���� �ʱ�ȭ
            PlayerStart = false;
            GameManager.Instance.GameOver(true);
            ClearObj();
            Destroy(gameObject);
        }
    }

    // ���� �� Ǯ ������Ʈ ��Ȱ��ȭ
    void ClearObj()
    {
        GameObject[] ClearObjs = GameObject.FindGameObjectsWithTag("PreyAni");

        foreach (GameObject obj in ClearObjs)
        {
            if (obj.GetComponent<PoolObj>() != null) obj.GetComponent<PoolObj>().ReleaseObject();
        }
    }


}