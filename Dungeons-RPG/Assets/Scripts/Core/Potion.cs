using UnityEngine;
using System.Collections;

public class Potion : Item
{
    private const int HEALING_POWER = 50;

    public override void use( Character character)
    {
        if(character.life + HEALING_POWER > character.life)
        {
            character.life = character.totalLife;
        }
        else
        {
            character.life += HEALING_POWER;
        }
    }
}
