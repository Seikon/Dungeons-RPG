using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    Battle battle;
    //Cola de turnos de acción
    public Queue<Character> acctionTurnQueue;
    
    //Lista de peticiones a la interfaz
    public enum BATTLE_REQUEST
    {
        NOTHING = 0,
        SELECT_ENEMY = 1
    }
    

    public BattleController(List<Character> teamLeft, 
                            List<Character> teamRight)
    {
    }


	void Start ()
    {
        //Crea los personajes
        Hero BrutusElPutus = GameObject.Find("BrutusElPutus").GetComponent<Hero>();
        Monster Skeleton = GameObject.Find("Skeleton").GetComponent<Monster>();
        Monster Skeleton1 = GameObject.Find("Skeleton1").GetComponent<Monster>();

        BrutusElPutus.battleGUID = Guid.NewGuid().ToString();
        Skeleton.battleGUID = Guid.NewGuid().ToString();
        Skeleton1.battleGUID = Guid.NewGuid().ToString();

        this.acctionTurnQueue = new Queue<Character>();

        List<Character> teamRight = new List<Character>();
        List<Character> teamLeft = new List<Character>();

        teamRight.Add(BrutusElPutus);
        teamLeft.Add(Skeleton);
        teamLeft.Add(Skeleton1);

        this.battle = new Battle(teamLeft, teamRight);

        BrutusElPutus.setState(Character.CHARACTER_STATE.CHARGING);
        Skeleton.setState(Character.CHARACTER_STATE.CHARGING);
        Skeleton1.setState(Character.CHARACTER_STATE.CHARGING);
    }
	
	void Update ()
    {
        //Comprueba una vez si hay algún personaje realizando alguna acción
        bool isPerforming = this.battle.checkCharacterPerforming();
        //Para cada personaje de batalla y según su estado, ejecuta las operaciones pertinentes
        foreach (Character battleCharacter in this.battle.battleCharacters)
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
                        battleCharacter.increaseProgressTurnBar(Time.deltaTime);
                        battleCharacter.updateProgressBar();
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
