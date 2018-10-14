using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Hero : Character
{
    public int magic; //Puntos de mágia (PM)
    public  Weapon weapon; //Arma

    public Button btnBasicAttack;
    public Button btnItem;

    public Text txtItem;
    public List<Text> txtListItems;

    public List<Item> bag;

    private const string  BTN_BASIC_ATTACK = "btnBasicAttack";
    private const string BTN_ITEM = "btnItem";
    private const string TXT_ITEM = "txtItem";

    public Hero(int attack, int defense, int speed, int evasion, int life, int magic) 
        : base(attack, defense, speed, evasion, life)
    {
        this.magic = magic;
        this.bag = new List<Item>();
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
        if (this.weapon == null)
        {
            isImpact = result < Battle.DEFAULT_PRECISION;
        }
        else
        {
            isImpact = result < this.weapon.precision;
        }

        return isImpact;
    }

    public override bool getCritical()
    {
        int result;
        bool isCritical = false;

        //Genera el número atleatorio 1-100
        result = Dice.generateRandomNumber();

        if(this.weapon == null)
        {
            isCritical = result < Weapon.DEFAULT_CRITICAL_ATTACK_PROBABILITY;
        }
        else
        {
            isCritical = result < this.weapon.criticalAttackProbability;
        }

        return isCritical;
    }

    public override int getCriticalAttack()
    {
        int criticalAttack;

        if (this.weapon == null)
        {
            criticalAttack = Mathf.FloorToInt(this.attack * Weapon.DEFAULT_CRITIAL_ATTACK_MODIFIER);
        }
        else
        {
            criticalAttack = Mathf.FloorToInt(this.attack * this.weapon.criticalAttackModifier);
        }

        return criticalAttack;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        this.txtListItems = new List<Text>();

        //Prepara los objetos de la interfaz gráfica

        foreach (Text txt in gameObject.GetComponentsInChildren<Text>())
        {
            switch (txt.name)
            {
                case TXT_ITEM:
                    this.txtItem = txt;
                    this.txtItem.gameObject.SetActive(false);
                    break;

            }
        }

        foreach (Button btn in this.gameObject.GetComponentsInChildren<Button>())
        {
            switch (btn.name)
            {
                case BTN_BASIC_ATTACK:
                    this.btnBasicAttack = btn;
                    //--Ataque básico--
                    this.btnBasicAttack.onClick.AddListener(this.generateBasicAttack);
                    this.btnBasicAttack.gameObject.SetActive(false);
                    break;

                case BTN_ITEM:
                    this.btnItem = btn;
                    //--Objeto
                    this.btnItem.onClick.AddListener(this.generateUseItem);
                    this.btnItem.gameObject.SetActive(false);
                    break;

            }
        }
    }

    protected override void Update()
    {
        switch(this.getState())
        {
            case CHARACTER_BATTLE_STATE.WAITING_ACTION:
                //Muestra los botones si no está pendiente de realizar ninguna acción
                if (this.request == null)
                {
                    this.btnBasicAttack.gameObject.SetActive(true);
                    this.btnItem.gameObject.SetActive(true);
                }

                //Si ya esta preparado
                if (this.checkActionFullFilled())
                {
                    //Marca la acción como lista y espera en la cola
                    this.selectedAction.actionState = BattleAction.BATTLE_ACTION_STATE.READY;
                    this.setState(CHARACTER_BATTLE_STATE.WAITING_QUEUE);
                }
                break;

            //Waiting an animation
            case Character.CHARACTER_BATTLE_STATE.START_PERFORM:
                this.animator.SetBool(Utils.Utils.ANIMATION_STATE_ATTACK, true);
                this.setState(CHARACTER_BATTLE_STATE.PERFORMING);
                break;
            //Check when animation has finished
            case CHARACTER_BATTLE_STATE.PERFORMING:
                if (this.animator.GetBool(Utils.Utils.ANIMATION_STATE_ATTACK) == false)
                {
                    this.setState(CHARACTER_BATTLE_STATE.PERFORMED);
                }
                break;
        }
         
        //Si le toca realizar la acción
        if (this.getState() != CHARACTER_BATTLE_STATE.WAITING_ACTION)
        {
            this.btnBasicAttack.gameObject.SetActive(false);
            this.btnItem.gameObject.SetActive(false);
        }
    }

    protected override void generateBasicAttack()
    {
        //Selecciona acción de ataque
        this.selectedAction = new BattleAction(BattleAction.BATTLE_ACCTION_TYPE.BASIC_ATTACK, null);
        //Al ser un Heroe la petición de acción será interactiva
        this.request = new BattleRequest(BattleRequest.STATE_BATTLE_REQUEST.SELECT_ENEMY, BattleRequest.MODE_BATTLE_REQUEST.INTERACTIVE);
        this.btnBasicAttack.gameObject.SetActive(false);
    }

    private void generateUseItem()
    {
        //Selecciona acción de uso de item
        this.selectedAction = new BattleAction(BattleAction.BATTLE_ACCTION_TYPE.USE_ITEM, null);
        //Al ser un heroe la petición será interactiva
        //Constará de dos partes:
        // 1- Selecionar el objeto
        // 2- Selecionar el personaje objetivo
        this.request = new BattleRequest(BattleRequest.STATE_BATTLE_REQUEST.SELECT_BAG_ITEM, BattleRequest.MODE_BATTLE_REQUEST.INTERACTIVE);
        //this.request = new BattleRequest(BattleRequest.STATE_BATTLE_REQUEST.SELECT_FRIEND, BattleRequest.MODE_BATTLE_REQUEST.INTERACTIVE);

        this.btnItem.gameObject.SetActive(false);
    }

    /// <summary>
    /// Usa el método para realizar una petición de selección al controlador de batalla
    /// </summary>
    /// <param name="requestType"></param>
    public void setRequest(BattleRequest request)
    {
        this.request = request;
    }

    private bool checkActionFullFilled()
    {
        bool isFullFilled = false;

        if(this.selectedAction != null)
        {
            switch (this.selectedAction.actionType)
            {
                //--Ataque básico--
                case BattleAction.BATTLE_ACCTION_TYPE.BASIC_ATTACK:
                    if (this.selectedAction.target != null && 
                        this.request.state == BattleRequest.STATE_BATTLE_REQUEST.ATTENDED)
                    {
                        isFullFilled = true;
                    }
                    break;
                //--Objeto--
                case BattleAction.BATTLE_ACCTION_TYPE.USE_ITEM:
                    if(this.selectedAction.target != null &&
                       this.selectedAction.itemTarget != null &&
                       this.request.state== BattleRequest.STATE_BATTLE_REQUEST.ATTENDED)
                    {
                        isFullFilled = true;
                    }
                    break;
            }
        }

        return isFullFilled;
    }

}
