using UnityEngine;
using UnityEditor;

public abstract class Skill
{
    public string name;
    public string niceName;
    public ELEMENT_TYPE type;
    public int damage;
    public int precision;

    public int criticalAttackProbability;
    public float criticalAttackModifier;

    public Skill(string name, string niceName, ELEMENT_TYPE type, int damage, int precision)
    {
        this.name = name;
        this.niceName = niceName;
        this.type = type;
        this.damage = damage;
        this.precision = precision;
    }

    public enum ELEMENT_TYPE
    {
        FIRE = 0,
        WATER = 1,
        ELECTRICITY= 2
    }
}