using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    Battle battle;
    //Cola de turnos de acción
    public Queue<Character> acctionTurnQueue;
    //Interfaz Gráfica


    public BattleController(List<Character> teamLeft, 
                            List<Character> teamRight)
    {
        this.battle = new Battle(teamLeft, teamRight);
        acctionTurnQueue = new Queue<Character>();
    }


	void Start ()
    {
		
	}
	
	void Update ()
    {
        //Comprueba una vez si hay algún personaje realizando alguna acción
        bool isPerforming = battle.checkCharacterPerforming();
        //Para cada personaje de batalla y según su estado, ejecuta las operaciones pertinentes
        foreach (var battleCharacter in battle.battleCharacters)
        {
            switch(battleCharacter.getState())
            {
                case Character.CHARACTER_STATE.IDLE:
                    break;

                case Character.CHARACTER_STATE.CHARGING:
                    //Si ha terminado de cargar, se pone al personaje al estado de esperando acción
                    if(battleCharacter.progressBarTurn >= Character.PROGRESS_TURN_BAR_MAX_VALUE)
                    {
                        battleCharacter.setState(Character.CHARACTER_STATE.WAITING_ACTION);
                    }
                    else
                    {
                        //Sino, sigue cargando la barra
                        battleCharacter.increaseProgressTurnBar();
                    }
                    break;

                case Character.CHARACTER_STATE.WAITING_QUEUE:
                    //Si está esperando para entrar en la cola se introduce
                    this.acctionTurnQueue.Enqueue(battleCharacter);
                    battleCharacter.setState(Character.CHARACTER_STATE.QUEUED);
                    break;

                case Character.CHARACTER_STATE.QUEUED:
                    //Ejecuta la acción siempre que no haya otro personaje realizando la acción
                    if(!isPerforming)
                    {
                        battle.executeAction(battleCharacter);
                        battleCharacter.setState(Character.CHARACTER_STATE.PERFORMING);
                        //Ahora habrá un personaje ejecutando una acción
                        isPerforming = true;
                    }
                    break;

                case Character.CHARACTER_STATE.PERFORMING:
                    break;

                case Character.CHARACTER_STATE.PERFORMED:
                    battleCharacter.progressBarTurn = Character.PROGRESS_TURN_BAR_MIN_VALUE;
                    battleCharacter.setState(Character.CHARACTER_STATE.CHARGING);
                    break;
            }
        }
	}
}
