using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Hero : Character
{
    public int magic; //Puntos de mágia (PM)
    public  Weapon weapon; //Arma
    public Button btnBasicAttack;
    public BattleController.BATTLE_REQUEST stateTargetRequest;

    private const string  BTN_BASIC_ATTACK = "btnBasicAttack";

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
    protected override void Start()
    {
        base.Start();

        //Prepara los objetos de la interfaz gráfica
        foreach (Button btn in gameObject.GetComponentsInChildren<Button>())
        {
            switch (btn.name)
            {
                case BTN_BASIC_ATTACK:
                    this.btnBasicAttack = btn;
                    //--Ataque básico--
                    this.btnBasicAttack.onClick.AddListener(this.generateBasicAttack);
                    break;

            }
        }
    }

    private void Update()
    {
        //Si ha realizado la acción pasa a esperar en cola
        if (this.getState() == CHARACTER_STATE.WAITING_ACTION)
        {
            if (this.selectedAction != null)
            {
                //La acción está preparada por lo que se meterá en la cola de acciones
                if(this.selectedAction.actionState == BattleAction.BATTLE_ACTION_STATE.READY)
                {
                    this.setState(CHARACTER_STATE.WAITING_QUEUE);
                }
            }
            else
            {
            }
        }
        else
        {
        }
    }

    protected override void generateBasicAttack()
    {
        this.selectedAction = new BattleAction(BattleAction.BATTLE_ACCTION_TYPE.BASIC_ATTACK, null);
        this.setTargetRequest(BattleController.BATTLE_REQUEST.SELECT_ENEMY);
    }
    /// <summary>
    /// Usa el método para realizar una petición de selección al controlador de batalla
    /// </summary>
    /// <param name="requestType"></param>
    public void setTargetRequest(BattleController.BATTLE_REQUEST requestType)
    {
        this.stateTargetRequest = requestType;
    }
    

}
