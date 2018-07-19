﻿using UnityEngine;
using System.Collections;

public class Monster : Character
{

    public Monster(int attack, int defense, int speed, int evasion, int life) 
        : base(attack, defense, speed, evasion, life)
    {
    }

    public override int getPowerBasicAttack()
    {
        return base.getPowerBasicAttack();
    }

}
