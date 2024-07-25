using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AnimalStat
{
    public int lv = 1;
    public string name = "";
    public double attack = 0.011f;
    public int speed = 0;
}

public class PlayerController : MonoBehaviour
{
    Rigidbody rigid;
    Vector3 moveVec;
    GameObject currentAnimal;

    [HideInInspector] public AnimalStat PlayerStat;
    [HideInInspector] public Slider goalSlider;

    private FloatingJoystick joy;
    public GameObject[] animalPrefabs;

    public int[] upgradeTime;
    public bool isStart = false;

    // ���� Ŭ��
    public AudioClip upgradeSound;
    public AudioClip eatSound;
    public AudioClip damageSound;
    public GameObject upgradeVFX;
    string _upgradeVFX = "UpgradeVFX"; // ������Ʈ Ǯ �̸�

    // �� ���ѱ��� ��ǥ
    private float minX = -86.4f;
    private float maxX = 67f;
    private float minZ = -90.3f;
    private float maxZ = 70.9f;

    void Start()
    {
        joy = GameManager.Instance.Joystick;
        PlayerStat = new AnimalStat();
        rigid = GetComponent<Rigidbody>();

        currentAnimal = Instantiate(animalPrefabs[0], transform);
        currentAnimal.transform.localPosition = Vector3.zero; // �� ������Ʈ�� ��ġ�� Animal�� ��ġ�� ����        
    }

    void Update()
    {
        if (isStart)
        {
            GameManager.Instance.curTime -= Time.deltaTime;

            Movement();
            LvUp();
            GameOver();
            Clear();
            Doundary();

            goalSlider.value = (float)GameManager.Instance.curScore / (float)GameManager.Instance.goalScore[PlayerStat.lv - 1];
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
            moveVec = inputDirection * PlayerStat.speed * Time.deltaTime;
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

        if (CurPos.x < minX)
        {
            CurPos.x = minX;
        }
        else if (CurPos.x > maxX)
        {
            CurPos.x = maxX;
        }

        if (CurPos.z < minZ)
        {
            CurPos.z = minZ;
        }
        else if (CurPos.z > maxZ)
        {
            CurPos.z = maxZ;
        }
        transform.position = CurPos;
    }

    // ���� ���׷��̵�
    void LvUp()
    {
        if (GameManager.Instance.curScore >= GameManager.Instance.goalScore[PlayerStat.lv - 1])
        {
            if (PlayerStat.lv <= 15) Upgrade();
        }
    }

    // ���� ���� ���׷��̵�
    public void Upgrade()
    {
        // ���� ������ ������Ʈ ���� �� ���� ������ ������Ʈ ����
        Destroy(currentAnimal);
        int index = PlayerStat.lv;
        currentAnimal = Instantiate(animalPrefabs[index], transform);
        currentAnimal.transform.localPosition = Vector3.zero; // �� ������Ʈ�� ��ġ�� Animal�� ��ġ�� ����

        //���� �ʱ�ȭ
        string SheetTxt = GameManager.Instance.StatData;
        GameManager.Instance.PlayerInitStat(SheetTxt, (index + 1).ToString());

        // ���׷��̵� ����Ʈ
        GameObject UpgradeVFX = PoolManager.instance.GetGo(_upgradeVFX);
        UpgradeVFX.transform.localScale = new Vector3(index, index, index);
        UpgradeVFX.transform.position = transform.position;
        UpgradeVFX.GetComponent<VFXController>().ReleaseObj();

        // ���׷��̵� ���� �߰��ð� 10��
        GameManager.Instance.curTime += 10;
    }

    // ���� Ŭ����
    void Clear()
    {
        if (GameManager.Instance.curScore >= GameManager.Instance.goalScore[15])
        {
            // ���� �ʱ�ȭ
            isStart = false;
            GameManager.Instance.GameOver(false);
            ClearObj(); // �� �� ������Ʈ Ǯ ��ȯ
            Destroy(gameObject); // �÷��̾� ����
        }
    }

    // ���� ����
    void GameOver()
    {
        if (GameManager.Instance.curTime <= 0)
        {
            // ���� �ʱ�ȭ
            isStart = false;
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