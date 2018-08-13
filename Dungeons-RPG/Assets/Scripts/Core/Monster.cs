using UnityEngine;
using System.Collections;

public class Monster : Character
{

    public Monster(int attack, int defense, int speed, int evasion, int life) 
        : base(attack, defense, speed, evasion, life)
    {
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    
    void Update()
    {
        //Si le toca realizar la acción
        if (this.getState() == CHARACTER_BATTLE_STATE.WAITING_ACTION)
        {
            if(this.selectedAction == null)
            {
                //Selecciona acción de ataque
                this.selectedAction = new BattleAction(BattleAction.BATTLE_ACCTION_TYPE.BASIC_ATTACK, null);
                //Al ser un Monstruo la petición de acción será Lógica
                this.request = new BattleRequest(BattleRequest.STATE_BATTLE_REQUEST.SELECT_ALL_ENEMIES, BattleRequest.MODE_BATTLE_REQUEST.LOGIC);
            }
            else
            {
                //Si la petición ha sido atendida
                if(this.request.state == BattleRequest.STATE_BATTLE_REQUEST.ATTENDED)
                {
                    //Recupera los miembros del equipo rival y ataca atleatoriamente a uno de ellos
                    int min = 0;
                    int max = this.selectedAction.targets.Count - 1;
                    int result = Dice.generateRandomNumber(min, max);
                    Character target = this.selectedAction.targets[result];
                    //Pone la acción a preparada
                    this.selectedAction.target = this.selectedAction.targets[result];
                    this.selectedAction.actionState = BattleAction.BATTLE_ACTION_STATE.READY;
                    //Modifica su estado listo para entrar en la cola de acciones
                    this.setState(CHARACTER_BATTLE_STATE.WAITING_QUEUE);
                }
            }

        }
    }

    public override bool getCritical()
    {
        int result;
        bool isCritical = false;

        //Genera el número atleatorio 1-100
        result = Dice.generateRandomNumber();

        isCritical = result < Weapon.DEFAULT_CRITICAL_ATTACK_PROBABILITY;

        return isCritical;
    }

    public override int getCriticalAttack()
    {
        int criticalAttack = Mathf.FloorToInt(this.attack * Weapon.DEFAULT_CRITIAL_ATTACK_MODIFIER);
        return criticalAttack;
    }

    public override int getPowerBasicAttack()
    {
        return base.getPowerBasicAttack();
    }

    protected override void generateBasicAttack()
    {

    }



}
