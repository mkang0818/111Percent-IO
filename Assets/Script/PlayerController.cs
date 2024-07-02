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
        CurrentAnimal.transform.localPosition = Vector3.zero; // 새 오브젝트의 위치를 Animal의 위치로 설정        
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

    // 맵 범위 밖 이동 제한
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

    // 레벨 업그레이드
    void LvUp()
    {
        if (GameManager.Instance.Score >= GameManager.Instance.goalScore[playerstat.Lv - 1])
        {
            //print("업그레이드");
            if(playerstat.Lv <= 15) Upgrade();
        }
    }
    // 다음 레벨 업그레이드
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
        GameManager.Instance.PlayerInitStat(SheetTxt, (index + 1).ToString());

        // 업그레이드 이펙트
        GameObject UpgradeVFX = PoolManager.instance.GetGo(upgradeVFX);
        UpgradeVFX.transform.localScale = new Vector3(index, index, index);
        UpgradeVFX.transform.position = transform.position;
        UpgradeVFX.GetComponent<VFXController>().ReleaseObj();
    }

    void Clear()
    {
        if (GameManager.Instance.Score >= GameManager.Instance.goalScore[15])
        {
            print("게임클리어");
            PlayerStart = false;
            GameManager.Instance.GameOver(false);
            ClearObj();
            Destroy(gameObject);
        }
    }

    // 게임 오버
    void GameOver()
    {
        if (playerstat.TimeLimit <= 0)
        {
            print("게임오버");
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

    // 플레이어 이동 구현
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