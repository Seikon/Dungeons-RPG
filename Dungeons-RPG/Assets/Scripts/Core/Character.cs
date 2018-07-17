using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    public int attack; //Ataque
    public int defense; //Defensa
    public int speed; //Velocidad
    public int evasion; //Evasión
    public int life; //Vida

    public Character(int attack, int defense, int speed, int evasion, int life)
    {
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.evasion = evasion;
        this.life = life;
    }
    virtual
    public int getPowerBasicAttack()
    {
        return this.attack;
    }
    /// <summary>
    /// Comprueba si se realiza con exito un ataque
    /// </summary>
    /// <returns></returns>
    virtual
    public bool getHit()
    {
        //Genera el número atleatorio
        int result = Dice.generateRandomNumber();

        //Si el valor resultado es menor que la probabilidad por defecto, entonces habrá impacto
        //Elemplo :
        // valores posibles --> 0 - 100
        // probabilidad --> 95%
        // resultado --> 20
        // 20 está entre (0..95) (95%) Acierto
        // resultado --> 97
        // 97 está entre (0..95) (95%) Fallo
        return result < Battle.DEFAULT_PRECISION;
    }

    /// <summary>
    /// Comprueba si realiza con éxito una acción evasiva
    /// </summary>
    /// <returns></returns>
    virtual
    public bool getDodge()
    {
        int result = Dice.generateRandomNumber();

        return result < this.evasion;
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
