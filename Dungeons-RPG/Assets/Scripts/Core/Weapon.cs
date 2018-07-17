using UnityEngine;
using System.Collections;

public class Weapon
{
    public int damage;
    public int precision;

    public Weapon(int damage, int precision)
    {
        this.damage = damage;
        this.precision = precision;
    }
}
