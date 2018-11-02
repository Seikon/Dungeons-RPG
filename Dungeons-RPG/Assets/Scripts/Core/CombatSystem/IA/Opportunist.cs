using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Core.CombatSystem.IA
{
    /*
     * La IA Oportunista ataca al protagonista:
     * - Con la defensa mágica más baja para el tipo de echizo disponible seleccionado (33%)
     * - Con la defensa cuerpo a cuerpo más baja (66%)
     */
    public class Opportunist : Personality
    {

        public override BattleAction elaborateStrategy(Character battleCharacter)
        {
            int numDecision = Dice.generateRandomNumber();
            BattleAction battleAction;

            //Realizará un ataque físico
            if (numDecision > 33)
            {
                battleAction = this.selectBattleActionAttack(battleCharacter);
                battleAction.actionType = BattleAction.BATTLE_ACCTION_TYPE.BASIC_ATTACK;
            }
            //Realizará un ataque mágico
            else
            {
                battleAction = this.selectBattleActionSkill(battleCharacter);
                battleAction.actionType = BattleAction.BATTLE_ACCTION_TYPE.MAGIC_ATTACK;
            }

            battleAction.actionState = BattleAction.BATTLE_ACTION_STATE.READY;

            return battleAction;
        }

        /// <summary>
        /// Selecciona el/los hechizos disponibles sobre los oponentes que tinen la defensa más baja
        /// </summary>
        /// <param name="battleCharacter"></param>
        /// <returns>Devuelve la acción de batalla formalizada</returns>
        private BattleAction selectBattleActionSkill(Character battleCharacter)
        {
            int lastIndexSkill = battleCharacter.magics.Count - 1;

            //Selecciona un echizo atleatoriamente
            int indSelectionSkill = Dice.generateRandomNumber(0, lastIndexSkill);
            Skill skillSelected = battleCharacter.magics[indSelectionSkill];

            //Busca que rival tiene la defensa mágica más baja del tipo al que corresponde el hechizo seleccionado
            Skill.ELEMENT_TYPE element = skillSelected.type;

            List<Character> candidateEnemies = battleCharacter.selectedAction.targets;

            Character target = base.selectCharacterLowerMagicalDefense(candidateEnemies, element);


            //Prepara la acción y la devuelve
            BattleAction battleAction = new BattleAction(BattleAction.BATTLE_ACCTION_TYPE.MAGIC_ATTACK, target);
            battleAction.skillTarget = skillSelected;

            return battleAction;

        }

        /// <summary>
        /// Selecciona el/los hechizos disponibles sobre los oponentes que tinen la defensa más baja
        /// </summary>
        /// <param name="battleCharacter"></param>
        /// <returns>Devuelve la acción de batalla formalizada</returns>
        private BattleAction selectBattleActionAttack(Character battleCharacter)
        {
            //Selecciona el rival con menor defensa
            List<Character> candidateEnemies = battleCharacter.selectedAction.targets;

            Character target = base.selectCharacterLowerDefense(candidateEnemies);

            //Prepara la acción y la devuelve
            BattleAction battleAction = new BattleAction(BattleAction.BATTLE_ACCTION_TYPE.BASIC_ATTACK, target);

            return battleAction;

        }


    }
}
