using UnityEngine;
using System.Collections;

public class Weapon
{
    public int damage;
    public int precision;

    public int criticalAttackProbability;
    public float criticalAttackModifier;

    public const int DEFAULT_CRITICAL_ATTACK_PROBABILITY = 10;//%
    public const float DEFAULT_CRITIAL_ATTACK_MODIFIER = 1.5f;

    public Weapon(int damage, int precision,
                  int criticalAttackProbability, float criticalAttackModifier)
    {
        this.damage = damage;
        this.precision = precision;
        this.criticalAttackProbability = criticalAttackProbability;
        this.criticalAttackModifier = criticalAttackModifier;
    }
}
