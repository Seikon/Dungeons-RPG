using UnityEngine;
using System.Collections;

public class Monster : Character
{

    public Monster(int attack, int defense, int speed, int evasion, int life) 
        : base(attack, defense, speed, evasion, life)
    {
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    
    void Update()
    {

    }

    public override int getPowerBasicAttack()
    {
        return base.getPowerBasicAttack();
    }

    protected override void generateBasicAttack()
    {
    }



}
