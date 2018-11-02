using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.Core.Skills;
public class Brutus : Hero
{

    public Brutus(int attack, int defense, int speed, int evasion, int life, int magic)
        : base(attack, defense, speed, evasion, life, magic)
    {
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        this.magics.Add(new FireBall());
    }

    protected override void Update()
    {
        base.Update();
    }

}
