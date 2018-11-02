using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Personality : IBehaviour
{
    public BattleAction calculateAction(Character battleCharacter)
    {
        //La primera vez creará la acción de batalla y pedirá los oponentes para elaborar una estrategia
        if (battleCharacter.selectedAction == null)
        {
            battleCharacter.selectedAction = new BattleAction(BattleAction.BATTLE_ACCTION_TYPE.IDLE, null);
            battleCharacter.selectedAction.actionState = BattleAction.BATTLE_ACTION_STATE.NOT_READY;
        }

        if (battleCharacter.request == null)
        {
            battleCharacter.request = new BattleRequest(BattleRequest.STATE_BATTLE_REQUEST.SELECT_ALL_ENEMIES,
                                                   BattleRequest.MODE_BATTLE_REQUEST.LOGIC,
                                                   true);
        }
        else
        {
            if(battleCharacter.selectedAction.targets != null)
            {
                //Decide que debe hacer
                return this.elaborateStrategy(battleCharacter);
            }

        }

        return battleCharacter.selectedAction;
    }

    public abstract BattleAction elaborateStrategy(Character battleCharacter);

    protected Character selectCharacterLowerDefense(List<Character> candidateCharacters)
    {
        int minDefense = int.MaxValue;

        Character candidateCharacter = null;

        foreach (Character currentCharacter in candidateCharacters)
        {
            if (currentCharacter.defense < minDefense)
                candidateCharacter = currentCharacter;
        }

        return candidateCharacter;

    }

    protected Character selectCharacterLowerMagicalDefense(List<Character> candidateCharacters, Skill.ELEMENT_TYPE magicalType)
    {
        int minDefense = int.MaxValue;

        Character candidateCharacter = null;

        foreach (Character currentCharacter in candidateCharacters)
        {
            if (currentCharacter.getMagicalDefense(magicalType) < minDefense)
                candidateCharacter = currentCharacter;
        }

        return candidateCharacter;

    }
}
