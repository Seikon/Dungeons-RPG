using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    Battle battle;
    //Cola de turnos de acción
    public Queue<Character> acctionTurnQueue;
    //Interfaz Gráfica
    Dictionary<int, Text> progressBarsUI;
    Dictionary<int, Text> lifeBarsUI;

    public BattleController(List<Character> teamLeft, 
                            List<Character> teamRight)
    {
    }


	void Start ()
    {
        //Asigna los elementos de la interfaz gráfica
        Text txtLog = GameObject.Find("txtLog").GetComponent<Text>();

        Text txtBrutusProgress = GameObject.Find("txtBrutusProgreso").GetComponent<Text>();
        Text txtEsqueletoProgress = GameObject.Find("txtEsqueletoProgreso").GetComponent<Text>();

        Text txtBrutusLife = GameObject.Find("txtBrutusVida").GetComponent<Text>();
        Text txtEsqueletoLife = GameObject.Find("txtEsqueletoVida").GetComponent<Text>();
        //Crea los personajes
        Hero BrutusElPutus = GameObject.Find("BrutusElPutus").GetComponent<Hero>();
        Monster Skeleton = GameObject.Find("Esqueleto").GetComponent<Monster>();

        BrutusElPutus.id = 1;
        Skeleton.id = 2;

        progressBarsUI = new Dictionary<int, Text>();
        lifeBarsUI = new Dictionary<int, Text>();

        this.acctionTurnQueue = new Queue<Character>();

        List<Character> teamRight = new List<Character>();
        List<Character> teamLeft = new List<Character>();

        teamRight.Add(BrutusElPutus);
        teamLeft.Add(Skeleton);

        this.battle = new Battle(teamLeft, teamRight);

        progressBarsUI.Add(BrutusElPutus.id, txtBrutusProgress);
        progressBarsUI.Add(Skeleton.id, txtEsqueletoProgress);

        lifeBarsUI.Add(BrutusElPutus.id, txtBrutusLife);
        lifeBarsUI.Add(Skeleton.id, txtEsqueletoLife);

        BrutusElPutus.setState(Character.CHARACTER_STATE.CHARGING);
        Skeleton.setState(Character.CHARACTER_STATE.CHARGING);

        GameObject.Find("btnBrutusAtaque").SetActive(false);
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
                        this.updateProgressBar(battleCharacter);
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
    private void updateProgressBar(Character character)
    {
        if(character.progressBarTurn > Character.PROGRESS_TURN_BAR_MAX_VALUE)
        {
            progressBarsUI[character.id].text = Character.PROGRESS_TURN_BAR_MAX_VALUE + "/" + Character.PROGRESS_TURN_BAR_MAX_VALUE;
        }
        else
        {
            progressBarsUI[character.id].text = character.progressBarTurn + "/" + Character.PROGRESS_TURN_BAR_MAX_VALUE;
        }
    }
}
