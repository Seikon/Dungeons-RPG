﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    Battle battle;
    //Cola de turnos de acción
    public Queue<Character> acctionTurnQueue;
    //Comprueba si un character está selecionando una acción
    private bool isSelecting = false;

    List<Character> possibleSelections;

    Character selectedTarget;
    Item selectedItem;
    Skill selectedSkill;
    List<Character> selectedTargets;

    Text selectedTextItem;

    int selectedIndex;

    private enum TEAM
    {
        NOTHING = 0,
        RIGHT = 1,
        LEFT = 2
    }

    public BattleController(List<Character> teamLeft,
                            List<Character> teamRight)
    {
    }


    void Start()
    {
        //Crea los personajes
        Brutus BrutusElPutus = GameObject.Find("BrutusElPutus").GetComponent<Brutus>();
        Skeleton Skeleton = GameObject.Find("Skeleton").GetComponent<Skeleton>();
        Skeleton Skeleton1 = GameObject.Find("Skeleton1").GetComponent<Skeleton>();
        Skeleton Skeleton2 = GameObject.Find("Skeleton2").GetComponent<Skeleton>();

        BrutusElPutus.battleGUID = Guid.NewGuid().ToString();
        Skeleton.battleGUID = Guid.NewGuid().ToString();
        Skeleton1.battleGUID = Guid.NewGuid().ToString();
        Skeleton2.battleGUID = Guid.NewGuid().ToString();

        this.acctionTurnQueue = new Queue<Character>();

        List<Character> teamRight = new List<Character>();
        List<Character> teamLeft = new List<Character>();

        teamRight.Add(BrutusElPutus);
        teamLeft.Add(Skeleton);
        teamLeft.Add(Skeleton1);
        teamLeft.Add(Skeleton2);

        Text txtLog = GameObject.Find("SimuladorBatalla").GetComponentInChildren<Text>();

        this.battle = new Battle(teamLeft, teamRight, txtLog);

        Skeleton.updateLifeBar();
        Skeleton1.updateLifeBar();
        Skeleton2.updateLifeBar();
        BrutusElPutus.updateLifeBar();

        BrutusElPutus.bag = new List<Item>();

        BrutusElPutus.bag.Add(new Potion());
        BrutusElPutus.bag.Add(new Potion());
        BrutusElPutus.bag.Add(new Potion());

        BrutusElPutus.setState(Character.CHARACTER_BATTLE_STATE.CHARGING);
        Skeleton.setState(Character.CHARACTER_BATTLE_STATE.CHARGING);
        Skeleton1.setState(Character.CHARACTER_BATTLE_STATE.CHARGING);
        Skeleton2.setState(Character.CHARACTER_BATTLE_STATE.CHARGING);
    }

    void Update()
    {
        //Comprueba si la batalla ha finalizado
        if(!this.battle.checkBattleEnded())
        {
            //Para cada personaje de batalla y según su estado, ejecuta las operaciones pertinentes
            foreach (Character battleCharacter in this.battle.battleCharacters)
            {
                switch (battleCharacter.getState())
                {
                    case Character.CHARACTER_BATTLE_STATE.IDLE:
                        break;

                    case Character.CHARACTER_BATTLE_STATE.CHARGING:
                        //Si ha terminado de cargar, se pone al personaje al estado de esperando acción
                        if (battleCharacter.progressBarTurn >= Character.PROGRESS_TURN_BAR_MAX_VALUE)
                        {
                            battleCharacter.setState(Character.CHARACTER_BATTLE_STATE.WAITING_ACTION);
                        }
                        else
                        {
                            //Sino, sigue cargando la barra
                            battleCharacter.increaseProgressTurnBar(Time.deltaTime);
                            battleCharacter.updateProgressBar();
                        }
                        break;

                    case Character.CHARACTER_BATTLE_STATE.WAITING_ACTION:
                        if (battleCharacter.request != null)
                        {
                            //Si está esperando por una acción Y tiene una petición al controlador de batalla
                            if (battleCharacter.request.state != BattleRequest.STATE_BATTLE_REQUEST.NOTHING)
                            {
                                this.proccessBattleCharacterRequest(battleCharacter);
                            }
                        }

                        break;

                    case Character.CHARACTER_BATTLE_STATE.WAITING_QUEUE:
                        //Si está esperando para entrar en la cola se introduce
                        this.acctionTurnQueue.Enqueue(battleCharacter);
                        battleCharacter.setState(Character.CHARACTER_BATTLE_STATE.QUEUED);
                        break;
                    case Character.CHARACTER_BATTLE_STATE.PERFORMED:
                        battle.executeAction(battleCharacter);
                        battleCharacter.progressBarTurn = Character.PROGRESS_TURN_BAR_MIN_VALUE;
                        battleCharacter.selectedAction = null;
                        battleCharacter.request = null;
                        battleCharacter.setState(Character.CHARACTER_BATTLE_STATE.CHARGING);
                        break;
                }
                //Ejecuta las acciones pendientes en la cola de turnos
                this.proccesTurnActionQueue();
            }
        }
        else
        {
            this.enabled = false;
            this.battle.txtLog.text += "\n" + "La batalla ha finalizado";
        }
    }

    private void proccesTurnActionQueue()
    {
        bool isPerforming = this.battle.checkCharacterPerforming();
        Character battleCharacter;
        //Ejecuta la acción siempre que no haya otro personaje realizando la acción
        if (!isPerforming)
        {
            //Si la cola de turnos tiene acciones pendientes
            if(this.acctionTurnQueue.Count > 0)
            {
                battleCharacter = this.acctionTurnQueue.Dequeue();
                //Es posible que el personaje haya muerto mientras estaba en la cola de turnos de acción,
                // por lo que habrá que sacarle y, por supuesto, no ejecutar su acción
                if(battleCharacter.getState() != Character.CHARACTER_BATTLE_STATE.DEAD)
                {
                    //Ahora habrá un personaje ejecutando una acción
                    battleCharacter.setState(Character.CHARACTER_BATTLE_STATE.START_PERFORM);
                }
            }
        }
    }

    private void proccessBattleCharacterRequest(Character battleChracter)
    {
        switch (battleChracter.request.state)
        {
            case BattleRequest.STATE_BATTLE_REQUEST.NOTHING:
                break;

            case BattleRequest.STATE_BATTLE_REQUEST.SELECT_ENEMY:
                this.activateSelectEnemy(battleChracter);
                break;

            case BattleRequest.STATE_BATTLE_REQUEST.SELECT_ALL_ENEMIES:
                this.activateSelectEnemies(battleChracter);
                break;

            case BattleRequest.STATE_BATTLE_REQUEST.SELECT_FRIEND:
                this.activateSelectFriend(battleChracter);
                break;


            case BattleRequest.STATE_BATTLE_REQUEST.SELECT_BAG_ITEM:
                this.activateSelectItem(battleChracter);
                break;

            case BattleRequest.STATE_BATTLE_REQUEST.SELECT_SKILL:
                this.activateSelectSkill(battleChracter);
                break;

            default:
                throw new Exception("El tipo de solicitud de batalla no es válida o no se ha implementado aún");
        }
    }
    /// <summary>
    /// Activa la selección de enemigo
    /// </summary>
    private void activateSelectEnemy(Character battleCharacter)
    {
        if (!this.isSelecting)
        {
            this.possibleSelections = new List<Character>();
            //Selecionará como los posibles objetivos a los personajes del equipo contrario

            //Primero comprueba a que equipo pertenece
            TEAM belongedTeam = getBattleCharacterTeam(battleCharacter);

            if (belongedTeam == TEAM.RIGHT)
            {
                this.possibleSelections = this.battle.teamLeft;
                this.isSelecting = true;
            }
            else if (belongedTeam == TEAM.LEFT)
            {
                this.possibleSelections = this.battle.teamRight;
                this.isSelecting = true;
            }
            else
            {
                throw new Exception("El personaje no pertenece a ningún equipo, por lo que no se puede seleccionar enemigo");
            }
            //Filtra por los personajes que están muertos
            this.possibleSelections = this.battle.filterDeadCharacters(this.possibleSelections);

            this.selectedTarget = this.possibleSelections[0];
            this.selectedIndex = 0;
            this.selectedTarget.txtName.color = Color.yellow;
            this.selectedTarget.txtLife.color = Color.yellow;
            this.selectedTarget.txtTurn.color = Color.yellow;
        }

        //Captura los eventos de selección de objetivo
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (this.selectedIndex > 0)
            {
                this.selectedTarget.txtName.color = Color.white;
                this.selectedTarget.txtLife.color = Color.white;
                this.selectedTarget.txtTurn.color = Color.white;

                selectedIndex--;

                this.selectedTarget = this.possibleSelections[selectedIndex];
                this.selectedTarget.txtName.color = Color.yellow;
                this.selectedTarget.txtLife.color = Color.yellow;
                this.selectedTarget.txtTurn.color = Color.yellow;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (this.selectedIndex < this.possibleSelections.Count - 1)
            {
                this.selectedTarget.txtName.color = Color.white;
                this.selectedTarget.txtLife.color = Color.white;
                this.selectedTarget.txtTurn.color = Color.white;

                this.selectedIndex++;

                this.selectedTarget = this.possibleSelections[selectedIndex];
                this.selectedTarget.txtName.color = Color.yellow;
                this.selectedTarget.txtLife.color = Color.yellow;
                this.selectedTarget.txtTurn.color = Color.yellow;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (!battleCharacter.request.firstTime)
            {

                this.isSelecting = false;
                battleCharacter.selectedAction.target = this.selectedTarget;

                this.selectedTarget.txtName.color = Color.white;
                this.selectedTarget.txtLife.color = Color.white;
                this.selectedTarget.txtTurn.color = Color.white;

                battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.ATTENDED;
            }
            else
            {
                battleCharacter.request.firstTime = false;
            }
        }
    }

    private void activateSelectEnemies(Character battleCharacter)
    {
        switch (battleCharacter.request.mode)
        {
            case BattleRequest.MODE_BATTLE_REQUEST.INTERACTIVE:
                break;

            case BattleRequest.MODE_BATTLE_REQUEST.LOGIC:
                TEAM teamBattleCharacter = this.getBattleCharacterTeam(battleCharacter);

                if (teamBattleCharacter == TEAM.LEFT)
                {
                    this.selectedTargets = this.battle.filterDeadCharacters(this.battle.teamRight);
                }
                else
                {
                    this.selectedTargets = this.battle.filterDeadCharacters(this.battle.teamRight);
                }

                battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.ATTENDED;
                battleCharacter.selectedAction.targets = this.selectedTargets;
                break;
        }
    }
    private void activateSelectFriend(Character battleCharacter)
    {
        if(!this.isSelecting)
        {
            this.possibleSelections = new List<Character>();
            //Selecionará como los posibles objetivos a los personajes del equipo contrario

            //Primero comprueba a que equipo pertenece
            TEAM belongedTeam = this.getBattleCharacterTeam(battleCharacter);

            if (belongedTeam == TEAM.RIGHT)
            {
                this.possibleSelections = this.battle.teamRight;
                this.isSelecting = true;
            }
            else if (belongedTeam == TEAM.LEFT)
            {
                this.possibleSelections = this.battle.teamLeft;
                this.isSelecting = true;
            }
            else
            {
                throw new Exception("El personaje no pertenece a ningún equipo, por lo que no se puede seleccionar enemigo");
            }
            //Filtra por los personajes que están muertos
            this.possibleSelections = this.battle.filterDeadCharacters(this.possibleSelections);

            this.selectedTarget = this.possibleSelections[0];
            this.selectedIndex = 0;
            this.selectedTarget.txtName.color = Color.yellow;
            this.selectedTarget.txtLife.color = Color.yellow;
            this.selectedTarget.txtTurn.color = Color.yellow;

        }

        //Captura los eventos de selección de objetivo
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (this.selectedIndex > 0)
            {
                this.selectedTarget.txtName.color = Color.white;
                this.selectedTarget.txtLife.color = Color.white;
                this.selectedTarget.txtTurn.color = Color.white;

                selectedIndex--;

                this.selectedTarget = this.possibleSelections[selectedIndex];
                this.selectedTarget.txtName.color = Color.yellow;
                this.selectedTarget.txtLife.color = Color.yellow;
                this.selectedTarget.txtTurn.color = Color.yellow;
            }
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (this.selectedIndex < this.possibleSelections.Count - 1)
            {
                this.selectedTarget.txtName.color = Color.white;
                this.selectedTarget.txtLife.color = Color.white;
                this.selectedTarget.txtTurn.color = Color.white;

                this.selectedIndex++;

                this.selectedTarget = this.possibleSelections[selectedIndex];
                this.selectedTarget.txtName.color = Color.yellow;
                this.selectedTarget.txtLife.color = Color.yellow;
                this.selectedTarget.txtTurn.color = Color.yellow;
            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (!battleCharacter.request.firstTime)
            {
                this.isSelecting = false;
                battleCharacter.selectedAction.target = this.selectedTarget;

                this.selectedTarget.txtName.color = Color.white;
                this.selectedTarget.txtLife.color = Color.white;
                this.selectedTarget.txtTurn.color = Color.white;

                battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.ATTENDED;
            }
            else
            {
                battleCharacter.request.firstTime = false;
            }
        }
    }

    private void activateSelectItem(Character battleCharacter)
    {
        Hero battleHero = (Hero) battleCharacter;

        if(!this.isSelecting)
        {
            battleHero.txtbagItems = new List<Text>();
            battleHero.txtBag.gameObject.SetActive(true);

            for(int indItem = 0; indItem < battleHero.bag.Count; indItem++)
            {
                //-----Creación del componente-----
                GameObject itemText = new GameObject("Item" + indItem);
                Text itemTempComp = itemText.AddComponent<Text>();
                //-----Personalización del componente-----
                itemTempComp.text = battleHero.bag[indItem].name;
                itemTempComp.font = battleHero.txtName.font;
                itemTempComp.alignment = TextAnchor.MiddleCenter;
                itemTempComp.fontSize = battleHero.txtName.fontSize;
                itemTempComp.color = battleHero.txtBag.color;
                itemText.transform.SetParent(battleHero.txtBag.transform);
                //Posición relativa a su padre
                itemText.GetComponent<RectTransform>().localPosition = new Vector2(-15, (indItem + 1) * -15);

                battleHero.txtbagItems.Add(itemTempComp);

            }

            this.selectedIndex = 0;
            this.selectedItem = battleHero.bag[0];
            battleHero.txtbagItems[0].color = Color.yellow;

            this.isSelecting = true;
        }

        //Captura los eventos de selección de objetivo
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (this.selectedIndex > 0)
            {
                battleHero.txtbagItems[this.selectedIndex].color = Color.white;

                this.selectedIndex--;

                this.selectedItem = battleHero.bag[selectedIndex];
                battleHero.txtbagItems[this.selectedIndex].color = Color.yellow;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (this.selectedIndex < battleHero.bag.Count - 1)
            {
                battleHero.txtbagItems[this.selectedIndex].color = Color.white;

                this.selectedIndex++;

                this.selectedItem = battleHero.bag[selectedIndex];
                battleHero.txtbagItems[this.selectedIndex].color = Color.yellow;

            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (!battleCharacter.request.firstTime)
            {
                this.isSelecting = false;

                battleCharacter.selectedAction.itemTarget = this.selectedItem;
                battleCharacter.selectedAction.selectedIndex = this.selectedIndex;
                battleHero.txtbagItems[this.selectedIndex].color = Color.white;

                battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.SELECT_FRIEND;

                foreach (Transform child in battleHero.txtBag.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                battleHero.txtBag.gameObject.SetActive(false);
            }
            else
            {
                battleCharacter.request.firstTime = false;
            }

        }

    }

    private void activateSelectSkill(Character battleCharacter)
    {
        Hero battleHero = (Hero)battleCharacter;

        if (!this.isSelecting)
        {
            battleHero.txtSkillItems = new List<Text>();
            battleHero.txtSkillsList.gameObject.SetActive(true);

            for (int indItem = 0; indItem < battleHero.magics.Count; indItem++)
            {
                //-----Creación del componente-----
                GameObject itemText = new GameObject("Skill" + indItem);

                Text itemTempComp = itemText.AddComponent<Text>();
                //-----Personalización del componente-----
                itemTempComp.text = battleHero.magics[indItem].niceName;
                itemTempComp.font = battleHero.txtName.font;
                itemTempComp.alignment = TextAnchor.MiddleCenter;
                itemTempComp.fontSize = battleHero.txtName.fontSize;
                itemTempComp.color = battleHero.txtMagicAttack.color;
                itemText.transform.SetParent(battleHero.txtSkillsList.transform);
                //Posición relativa a su padre
                itemText.GetComponent<RectTransform>().localPosition = new Vector2(-15, (indItem + 1) * -15);
                
                battleHero.txtSkillItems.Add(itemTempComp);

                itemText.SetActive(true);

            }

            this.selectedIndex = 0;
            this.selectedSkill = battleHero.magics[0];
            battleHero.txtSkillItems[0].color = Color.yellow;

            this.isSelecting = true;
        }

        //Captura los eventos de selección de objetivo
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (this.selectedIndex > 0)
            {
                battleHero.txtSkillItems[this.selectedIndex].color = Color.white;

                this.selectedIndex--;

                this.selectedSkill = battleHero.magics[selectedIndex];
                battleHero.txtSkillItems[this.selectedIndex].color = Color.yellow;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (this.selectedIndex < battleHero.magics.Count - 1)
            {
                battleHero.txtSkillItems[this.selectedIndex].color = Color.white;

                this.selectedIndex++;

                this.selectedSkill = battleHero.magics[selectedIndex];
                battleHero.txtSkillItems[this.selectedIndex].color = Color.yellow;

            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (!battleCharacter.request.firstTime)
            {
                this.isSelecting = false;

                battleCharacter.selectedAction.skillTarget = this.selectedSkill;
                battleCharacter.selectedAction.selectedIndex = this.selectedIndex;
                battleHero.txtSkillItems[this.selectedIndex].color = Color.white;

                battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.SELECT_ENEMY;

                foreach (Transform child in battleHero.txtSkillsList.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                battleHero.txtSkillsList.gameObject.SetActive(false);
            }
            else
            {
                battleCharacter.request.firstTime = false;
            }

        }
    }

    private TEAM getBattleCharacterTeam(Character battleCharacter)
    {
        TEAM belongedTeam = TEAM.NOTHING;

        foreach (Character characterCompare in this.battle.teamLeft)
        {
            if(battleCharacter.battleGUID.Equals(characterCompare.battleGUID))
            {
                belongedTeam = TEAM.LEFT;
                break;
            }
        }

        if(belongedTeam == TEAM.NOTHING)
        {
            foreach (Character characterCompare in this.battle.teamRight)
            {
                if (battleCharacter.battleGUID.Equals(characterCompare.battleGUID))
                {
                    belongedTeam = TEAM.RIGHT;
                    break;
                }
            }
        }

        return belongedTeam;
    }

}
