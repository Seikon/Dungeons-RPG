using UnityEngine;
using System.Collections;
using Assets.Scripts.Core.CombatSystem.IA;

public class Monster : Character
{
    public IBehaviour IA = null;

    public Monster(int attack, int defense, int speed, int evasion, int life) 
        : base(attack, defense, speed, evasion, life)
    {
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    
    protected override void Update()
    {
        //Si le toca realizar la acción
        switch (this.getState())
        {
            case CHARACTER_BATTLE_STATE.WAITING_ACTION:
                this.selectedAction = IA.calculateAction(this);

                if (selectedAction.actionState == BattleAction.BATTLE_ACTION_STATE.READY)
                {
                    this.setState(CHARACTER_BATTLE_STATE.WAITING_QUEUE);
                }
                break;
            //Espera a la animación
            case Character.CHARACTER_BATTLE_STATE.START_PERFORM:
                base.startAnimation();
                break;
            //Comprueba cuando la animación ha terminado
            case CHARACTER_BATTLE_STATE.PERFORMING:
                base.controlAnimation();
                break;

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
