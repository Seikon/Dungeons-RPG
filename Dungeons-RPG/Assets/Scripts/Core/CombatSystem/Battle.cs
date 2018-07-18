using UnityEngine;
using System.Collections;

public class Battle : MonoBehaviour
{
    public static int DEFAULT_PRECISION = 95;
    private static float ATTACK_MODIFIER = 0.2f;
    /// <summary>
    /// Comprueba si un atacante ha golpeado a un objetivo
    /// </summary>
    /// <param name="attacker">El atacante (quien realiza el ataque)</param>
    /// <param name="target">El objetivo (quien recibe el ataque)</param>
    /// <returns></returns>
    private bool getImpact(Character attacker, Character target)
    {
        bool isImpact = false;
        //Primero comprueba si el atacante tiene exito en el ataque
        if (attacker.getHit())
        {
            //Habrá impacto siempre y cuando no consiga esquivar el golpe el objetivo
            isImpact = !target.getDodge();
        }

        return isImpact;
    }

    private int calculateDamage(Character attacker, Character target)
    {
        int minValue, maxValue;
        int resultAttack;
        int totalDamage;

        //Generamos el modificador máximo y mínimo para el ataque
        minValue = attacker.attack - (int) (attacker.attack * ATTACK_MODIFIER);
        maxValue = attacker.attack + (int) (attacker.attack * ATTACK_MODIFIER);
        //Obtenemos el valor resultado para el modificador de ataque
        resultAttack = Dice.generateRandomNumber(minValue, maxValue);
        //Resolvemos el ataque restandole el tanto por ciento de la defensa del objetivo
        //------------------------------------------
        //attack = attack - attack * (defense / 100)
        //------------------------------------------
        totalDamage = attacker.attack - (attacker.attack * (target.defense / 100));

        return totalDamage;
    }
    /// <summary>
    /// Resta vida a la victima de un ataque
    /// </summary>
    /// <param name="victim">El objeto Character al que se le extrae la vida</param>
    /// <param name="damage">el daño que recibe (directamente se extra de la vida)</param>
    /// <returns>Devuelve si la victima del ataque ha fallecido o no</returns>
    public bool substractLife(Character victim, int damage)
    {
        bool dead = false;

        victim.life = victim.life - damage;

        dead = victim.life < 0;

        return dead;

    }
    /// <summary>
    /// Un atacante realiza un ataque sobre un objetivo
    /// 1. Se cálcula si hay Hit
    /// 2. Se calcula el daño
    /// </summary>
    /// <param name="attacker">Atacante</param>
    /// <param name="target"> Objetivo</param>
    /// <returns>El daño total recibido</returns>
    public int? attack(Character attacker, Character target)
    {
        int? damage = null;

        //El atacante comprueba si acierta al objetivo
        if (this.getImpact(attacker, target))
        {
            //Una vez acertado calcula el daño
             damage = this.calculateDamage(attacker, target);
        }

        return damage;
    }
}
