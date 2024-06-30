using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Statinit
{
    public string Name = "";
    public int HP = 0;
    public int At = 0;
    public int MaxExp = 0;
    public int CurExp = 0;
}
[CreateAssetMenu]
public class Stat : ScriptableObject
{
    public Statinit[] stat;
}
