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

    // 사운드 클립
    public AudioClip upgradeSound;
    public AudioClip eatSound;
    public AudioClip damageSound;
    public GameObject upgradeVFX;
    string _upgradeVFX = "UpgradeVFX"; // 오브젝트 풀 이름

    // 맵 제한구역 좌표
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
        currentAnimal.transform.localPosition = Vector3.zero; // 새 오브젝트의 위치를 Animal의 위치로 설정        
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

    // 플레이어 이동 구현
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

    // 맵 범위 밖 이동 제한
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

    // 레벨 업그레이드
    void LvUp()
    {
        if (GameManager.Instance.curScore >= GameManager.Instance.goalScore[PlayerStat.lv - 1])
        {
            if (PlayerStat.lv <= 15) Upgrade();
        }
    }

    // 다음 레벨 업그레이드
    public void Upgrade()
    {
        // 현재 레벨의 오브젝트 삭제 후 다음 레벨의 오브젝트 생성
        Destroy(currentAnimal);
        int index = PlayerStat.lv;
        currentAnimal = Instantiate(animalPrefabs[index], transform);
        currentAnimal.transform.localPosition = Vector3.zero; // 새 오브젝트의 위치를 Animal의 위치로 설정

        //스탯 초기화
        string SheetTxt = GameManager.Instance.StatData;
        GameManager.Instance.PlayerInitStat(SheetTxt, (index + 1).ToString());

        // 업그레이드 이펙트
        GameObject UpgradeVFX = PoolManager.instance.GetGo(_upgradeVFX);
        UpgradeVFX.transform.localScale = new Vector3(index, index, index);
        UpgradeVFX.transform.position = transform.position;
        UpgradeVFX.GetComponent<VFXController>().ReleaseObj();

        // 업그레이드 혜택 추가시간 10초
        GameManager.Instance.curTime += 10;
    }

    // 게임 클리어
    void Clear()
    {
        if (GameManager.Instance.curScore >= GameManager.Instance.goalScore[15])
        {
            // 게임 초기화
            isStart = false;
            GameManager.Instance.GameOver(false);
            ClearObj(); // 맵 내 오브젝트 풀 반환
            Destroy(gameObject); // 플레이어 삭제
        }
    }

    // 게임 오버
    void GameOver()
    {
        if (GameManager.Instance.curTime <= 0)
        {
            // 게임 초기화
            isStart = false;
            GameManager.Instance.GameOver(true);
            ClearObj();
            Destroy(gameObject);
        }
    }

    // 게임 내 풀 오브젝트 비활성화
    void ClearObj()
    {
        GameObject[] ClearObjs = GameObject.FindGameObjectsWithTag("PreyAni");

        foreach (GameObject obj in ClearObjs)
        {
            if (obj.GetComponent<PoolObj>() != null) obj.GetComponent<PoolObj>().ReleaseObject();
        }
    }


}