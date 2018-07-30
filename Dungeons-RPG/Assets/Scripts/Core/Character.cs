using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{

    public enum CHARACTER_BATTLE_STATE
    {
        IDLE = 0, // EL PERSONAJE ESTÁ EN ESTADO DE REPOSO
        CHARGING = 1, // EL PERSONAJE ESTÁ CARGANDO SU BARRA DE TURNO
        WAITING_ACTION = 2, // EL PERSONAJE ESTÁ ESPERANDO POR EL USUARIO / IA PARA REALIZAR UNA ACCIÓN
        WAITING_QUEUE = 3, // EL PERSONAJE HA SELECCIONADO UNA ACCIÓN Y ESTÁ ESPERANDO A SER INTRODUCIDO EN LA COLA DE TURNOS DE ACCIÓN
        QUEUED = 4, // EL PERSONAJE HA SIDO INTRODUCIDO EN LA COLA DE TURNOS DE ACCIÓN Y ESPERA A QUE SU ACCIÓN SEA EJECUTADA
        PERFORMING = 5, // EL PERSONAJE ESTÁ REALIZANDO SU ACCIÓN
        PERFORMED = 6, // EL PERSONAJE HA FINALIZADO SU ACCIÓN
        DEAD = 7, // EL PERSONAJE HA REDUCIDO SU VIDA A 0
    }

    public static int PROGRESS_TURN_BAR_MIN_VALUE = 0;
    public static int PROGRESS_TURN_BAR_MAX_VALUE = 100;
    public BattleRequest request;

    protected const string TXT_LIFE = "txtLife";
    protected const string TXT_NAME = "txtName";
    protected const string TXT_TURN = "txtTurn";

    //----Parameters-----
    public int attack; //Ataque
    public int defense; //Defensa
    public int speed; //Velocidad
    public int evasion; //Evasión
    public int life; //Vida Actual
    public int totalLife; //Vida total
    //-------------------
    private CHARACTER_BATTLE_STATE state; //Estado del personaje

    public BattleAction selectedAction;
    public float progressBarTurn;
    //Id único del personaje en la batalla
    public string battleGUID;

    public Object objectLock;
    //----Interfaz-----
    public Text txtName;
    public Text txtLife;
    public Text txtTurn;


    public Character(int attack, int defense, int speed, int evasion, int life)
    {
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.evasion = evasion;
        this.life = life;
        this.totalLife = life;
        this.progressBarTurn = 0;
        this.state = CHARACTER_BATTLE_STATE.IDLE;
    }

    // Use this for initialization
    protected virtual void Start()
    {
        this.totalLife = life;
        //Objeto de exclusión mutua
        this.objectLock = new Object();
        //Prepara los objetos de la interfaz gráfica
        foreach (Text txt in gameObject.GetComponentsInChildren<Text>())
        {
            switch (txt.name)
            {
                case Character.TXT_NAME:
                    this.txtName = txt;
                    break;

                case Character.TXT_LIFE:
                    this.txtLife = txt;
                    break;

                case Character.TXT_TURN:
                    this.txtTurn = txt;
                    break;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
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

    public abstract bool getCritical();
    public abstract int getCriticalAttack();

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
        this.progressBarTurn += (this.speed * deltaTime);
    }

    public void setState(CHARACTER_BATTLE_STATE newState)
    {
        lock(this.objectLock)
        {
            this.state = newState;
        }
    }

    public CHARACTER_BATTLE_STATE getState()
    {
        return this.state;
    }

    protected abstract void generateBasicAttack();

    public void updateProgressBar()
    {
        if (this.progressBarTurn > Character.PROGRESS_TURN_BAR_MAX_VALUE)
        {
            txtTurn.text = Character.PROGRESS_TURN_BAR_MAX_VALUE + "/" + Character.PROGRESS_TURN_BAR_MAX_VALUE;
        }
        else
        {
            txtTurn.text = Mathf.FloorToInt(this.progressBarTurn) + "/" + Character.PROGRESS_TURN_BAR_MAX_VALUE;
        }
    }

    public void updateLifeBar()
    {
        if(this.getState() == CHARACTER_BATTLE_STATE.DEAD)
        {
            this.txtLife.color = Color.red;
        }

        this.txtLife.text = this.life + " / " + this.totalLife;

    }
}
