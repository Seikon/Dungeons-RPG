using UnityEngine;
using System.Collections;

public class Skeleton : Monster
{

    public Skeleton(int attack, int defense, int speed, int evasion, int life) 
        : base(attack, defense, speed, evasion, life)
    {
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    
    protected override void Update()
    {
        base.Update();
    }

    public override bool getCritical()
    {
        int result;
        bool isCritical = false;

        //Genera el número atleatorio 1-100
        result = Dice.generateRandomNumber();

        isCritical = result < Weapon.DEFAULT_CRITICAL_ATTACK_PROBABILITY;

        return isCritical;
    }

    public override int getCriticalAttack()
    {
        int criticalAttack = Mathf.FloorToInt(this.attack * Weapon.DEFAULT_CRITIAL_ATTACK_MODIFIER);
        return criticalAttack;
    }

    public override int getPowerBasicAttack()
    {
        return base.getPowerBasicAttack();
    }

    protected override void generateBasicAttack()
    {

    }



}
