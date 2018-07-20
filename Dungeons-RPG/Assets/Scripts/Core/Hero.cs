using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hero : Character
{
    public int magic; //Puntos de mágia (PM)
    public  Weapon weapon; //Arma

    public Hero(int attack, int defense, int speed, int evasion, int life, int magic) 
        : base(attack, defense, speed, evasion, life)
    {
        this.magic = magic;
    }
    // El ataque básico será: attack + damage del Arma (en caso de que tenga arma)
    
    public override int getPowerBasicAttack()
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
    
    public override bool getHit()
    {
        int result;
        bool isImpact = false;

        //Genera el número atleatorio
        result = Dice.generateRandomNumber();

        //Si lleva arma saca la precisión del propio arma,
        //Sino obtendrá la precisión por defecto
        if (this.weapon.Equals(null))
        {
            isImpact = result < Battle.DEFAULT_PRECISION;
        }
        else
        {
            isImpact = result < this.weapon.precision;
        }

        return isImpact;
    }

    // Use this for initialization
    void Start()
    {
        //Objeto de exclusión mutua
        base.objectLock = new Object();
    }

    private void Update()
    {
        //Si ha realizado la acción pasa a esperar en cola
        if (this.getState() == CHARACTER_STATE.WAITING_ACTION)
        {
            if (this.selectedAction != null)
            {
                this.setState(CHARACTER_STATE.WAITING_QUEUE);
            }
            else
            {
            }
        }
        else
        {
        }
    }

}
