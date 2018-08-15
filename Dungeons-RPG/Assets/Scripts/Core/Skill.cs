using UnityEngine;
using UnityEditor;

public class Skill
{
    public ELEMENT_TYPE type;
    public int damage;
    public int precision;

    public enum ELEMENT_TYPE
    {
        FIRE = 0,
        WATER = 1,
        ELECTRICITY= 2
    }
}