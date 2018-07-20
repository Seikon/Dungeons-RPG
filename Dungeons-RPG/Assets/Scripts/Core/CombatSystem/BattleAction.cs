using UnityEngine;
using UnityEditor;

public class BattleAction
{
    public enum BATTLE_ACCTION_TYPE
    {
        IDLE = 0,
        BASIC_ATTACK = 1
    }

    public enum BATTLE_ACTION_STATE
    {
        NOT_READY = 0,
        READY = 1
    }

    public BATTLE_ACCTION_TYPE actionType;
    public BATTLE_ACTION_STATE actionState;
    public Character target;

    public BattleAction(BATTLE_ACCTION_TYPE actionType, Character target)
    {
        this.actionType = actionType;
        this.target = target;

        this.actionState = BATTLE_ACTION_STATE.NOT_READY;
    }

}