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

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
