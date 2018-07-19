using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    public enum CHARACTER_STATE
    {
        IDLE = 0, // EL PERSONAJE ESTÁ EN ESTADO DE REPOSO
        CHARGING = 1, // EL PERSONAJE ESTÁ CARGANDO SU BARRA DE TURNO
        WAITING_ACTION = 2, // EL PERSONAJE ESTÁ ESPERANDO POR EL USUARIO / IA PARA REALIZAR UNA ACCIÓN
        WAITING_QUEUE = 3, // EL PERSONAJE HA SELECCIONADO UNA ACCIÓN Y ESTÁ ESPERANDO A SER INTRODUCIDO EN LA COLA DE TURNOS DE ACCIÓN
        QUEUED = 4, // EL PERSONAJE HA SIDO INTRODUCIDO EN LA COLA DE TURNOS DE ACCIÓN Y ESPERA A QUE SU ACCIÓN SEA EJECUTADA
        PERFORMING = 5, // EL PERSONAJE ESTÁ REALIZANDO SU ACCIÓN
        PERFORMED = 6, // EL PERSONAJE HA FINALIZADO SU ACCIÓN
    }

    public static int PROGRESS_TURN_BAR_MIN_VALUE = 0;
    public static int PROGRESS_TURN_BAR_MAX_VALUE = 100;

    //----Parameters-----
    public int attack; //Ataque
    public int defense; //Defensa
    public int speed; //Velocidad
    public int evasion; //Evasión
    public int life; //Vida
    //-------------------
    private CHARACTER_STATE state; //Estado del personaje

    public BattleAction selectedAction;
    public int progressBarTurn;
    public int id;

    private Object objectLock;

    public Character(int attack, int defense, int speed, int evasion, int life)
    {
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.evasion = evasion;
        this.life = life;
        this.progressBarTurn = 0;
        this.state = CHARACTER_STATE.IDLE;

    }
    
    public virtual int getPowerBasicAttack()
    {
        return this.attack;
    }
    /// <summary>
    /// Comprueba si se realiza con exito un ataque
    /// </summary>
    /// <returns></returns>
    
    public virtual bool getHit()
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
    
    public virtual bool getDodge()
    {
        int result = Dice.generateRandomNumber();

        return result < this.evasion;
    }

    public void increaseProgressTurnBar(float deltaTime)
    {
        this.progressBarTurn += Mathf.FloorToInt(this.speed * deltaTime);
    }

    public void setState(CHARACTER_STATE newState)
    {
        lock(objectLock)
        {
            this.state = newState;
        }
    }

    public CHARACTER_STATE getState()
    {
        return this.state;
    }

	// Use this for initialization
	void Start ()
    {
        //Objeto de exclusión mutua
        this.objectLock = new Object();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Si ha realizado la acción pasa a esperar en cola
		if(this.getState() == CHARACTER_STATE.WAITING_ACTION && this.selectedAction != null)
        {
            this.setState(CHARACTER_STATE.WAITING_QUEUE);
        }
	}
}
