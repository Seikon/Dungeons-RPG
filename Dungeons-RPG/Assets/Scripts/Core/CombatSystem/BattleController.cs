using System;
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
        Hero BrutusElPutus = GameObject.Find("BrutusElPutus").GetComponent<Hero>();
        Monster Skeleton = GameObject.Find("Skeleton").GetComponent<Monster>();
        Monster Skeleton1 = GameObject.Find("Skeleton1").GetComponent<Monster>();
        Monster Skeleton2 = GameObject.Find("Skeleton2").GetComponent<Monster>();

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
            //Comprueba una vez si hay algún personaje realizando alguna acción
            bool isPerforming = this.battle.checkCharacterPerforming();
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

                    case Character.CHARACTER_BATTLE_STATE.QUEUED:
                        //Ejecuta la acción siempre que no haya otro personaje realizando la acción
                        if (!isPerforming)
                        {
                            isPerforming = true;
                            battleCharacter.setState(Character.CHARACTER_BATTLE_STATE.PERFORMING);
                            battle.executeAction(battleCharacter);
                            battleCharacter.selectedAction = null;
                            battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.NOTHING;
                            battleCharacter.setState(Character.CHARACTER_BATTLE_STATE.PERFORMED);
                            //Ahora habrá un personaje ejecutando una acción
                        }
                        break;

                    case Character.CHARACTER_BATTLE_STATE.PERFORMING:
                        break;

                    case Character.CHARACTER_BATTLE_STATE.PERFORMED:
                        battleCharacter.request = null;
                        battleCharacter.progressBarTurn = Character.PROGRESS_TURN_BAR_MIN_VALUE;
                        battleCharacter.setState(Character.CHARACTER_BATTLE_STATE.CHARGING);
                        break;
                }
            }
        }
        else
        {
            this.enabled = false;
            this.battle.txtLog.text += "\n" + "La batalla ha finalizado";
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
            this.isSelecting = false;
            battleCharacter.selectedAction.target = this.selectedTarget;

            this.selectedTarget.txtName.color = Color.white;
            this.selectedTarget.txtLife.color = Color.white;
            this.selectedTarget.txtTurn.color = Color.white;

            battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.ATTENDED;
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
            this.isSelecting = false;
            battleCharacter.selectedAction.target = this.selectedTarget;

            this.selectedTarget.txtName.color = Color.white;
            this.selectedTarget.txtLife.color = Color.white;
            this.selectedTarget.txtTurn.color = Color.white;

            battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.ATTENDED;
        }
    }

    private void activateSelectItem(Character battleCharacter)
    {
        Hero battleHero = (Hero) battleCharacter;

        if(!this.isSelecting)
        {
            battleHero.txtListItems = new List<Text>();
            battleHero.txtItem.gameObject.SetActive(true);

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
                itemTempComp.color = battleHero.txtItem.color;
                itemText.transform.SetParent(battleHero.txtItem.transform);
                //Posición relativa a su padre
                itemText.GetComponent<RectTransform>().localPosition = new Vector2(-15, (indItem + 1) * -15);

                battleHero.txtListItems.Add(itemTempComp);

            }

            this.selectedIndex = 0;
            this.selectedItem = battleHero.bag[0];
            battleHero.txtListItems[0].color = Color.yellow;

            this.isSelecting = true;
        }

        //Captura los eventos de selección de objetivo
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (this.selectedIndex > 0)
            {
                battleHero.txtListItems[this.selectedIndex].color = Color.white;

                this.selectedIndex--;

                this.selectedItem = battleHero.bag[selectedIndex];
                battleHero.txtListItems[this.selectedIndex].color = Color.yellow;
            }
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (this.selectedIndex < battleHero.bag.Count - 1)
            {
                battleHero.txtListItems[this.selectedIndex].color = Color.white;

                this.selectedIndex++;

                this.selectedItem = battleHero.bag[selectedIndex];
                battleHero.txtListItems[this.selectedIndex].color = Color.yellow;

            }
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            this.isSelecting = false;

            battleCharacter.selectedAction.itemTarget = this.selectedItem;
            battleCharacter.selectedAction.selectedIndex = this.selectedIndex;
            battleHero.txtListItems[this.selectedIndex].color = Color.white;

            battleCharacter.request.state = BattleRequest.STATE_BATTLE_REQUEST.ATTENDED;

            foreach (Transform child in battleHero.txtItem.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            battleHero.txtItem.gameObject.SetActive(false);
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
