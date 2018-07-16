using UnityEngine;
using System.Collections;

public class Hero : Character
{
    public  Weapon weapon; //Arma

    public Hero(int attack, int defense, int speed, int evasion, int life) 
        : base(attack, defense, speed, evasion, life)
    {
    }
    override
    public int getPowerBasicAttack()
    {
        int totalDamage = 0;
        
        if(this.weapon.Equals(null))
        {
            totalDamage = base.getPowerBasicAttack();
        }
        else
        {
            totalDamage = this.attack + this.weapon.damage;
        }

        return totalDamage;

    }
}
