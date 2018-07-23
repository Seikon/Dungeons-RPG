using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Battle

{
    public static int DEFAULT_PRECISION = 95;
    private static float ATTACK_MODIFIER = 0.2f;
    //Los equipos que se enfrentan entre si
    public List<Character> teamRight;
    public List<Character> teamLeft;
    //Todos los personajes ordenados de derecha a izquierda
    public List<Character> battleCharacters;
    public Text txtLog;

    public Battle(List<Character> teamLeft,
                  List<Character> teamRight, Text txtLog)
    {
        this.teamLeft = teamLeft;
        this.teamRight = teamRight;
        this.txtLog = txtLog;

        this.battleCharacters = new List<Character>();
        //Fusiona los personajes en una lista
        //1. Equipo Derecha
        foreach (Character character in teamRight)
        {
            this.battleCharacters.Add(character);
        }
        //2. Equipo Izquierda
        foreach (Character character in teamLeft)
        {
            this.battleCharacters.Add(character);
        }
    }

    /// <summary>
    /// Comprueba si un atacante ha golpeado a un objetivo
    /// </summary>
    /// <param name="attacker">El atacante (quien realiza el ataque)</param>
    /// <param name="target">El objetivo (quien recibe el ataque)</param>
    /// <returns></returns>
    private bool getImpact(Character attacker, Character target)
    {
        bool isImpact = false;
        //Primero comprueba si el atacante tiene exito en el ataque
        if (attacker.getHit())
        {
            //Habrá impacto siempre y cuando no consiga esquivar el golpe el objetivo
            isImpact = !target.getDodge();
        }

        return isImpact;
    }

    private int calculateDamage(Character attacker, Character target)
    {
        int minValue, maxValue;
        float resultAttack;
        float totalDamage;

        //Generamos el modificador máximo y mínimo para el ataque
        minValue = attacker.attack - (int) (attacker.attack * ATTACK_MODIFIER);
        maxValue = attacker.attack + (int) (attacker.attack * ATTACK_MODIFIER);
        //Obtenemos el valor resultado para el modificador de ataque
        resultAttack = Dice.generateRandomNumber(minValue, maxValue);
        //Resolvemos el ataque restandole el tanto por ciento de la defensa del objetivo
        //------------------------------------------
        //attack = attack - attack * (defense / 100)
        //------------------------------------------
        totalDamage = resultAttack - (resultAttack * ((float) target.defense / 100));

        return Mathf.FloorToInt(totalDamage);
    }
    /// <summary>
    /// Resta vida a la victima de un ataque
    /// </summary>
    /// <param name="victim">El objeto Character al que se le extrae la vida</param>
    /// <param name="damage">el daño que recibe (directamente se extra de la vida)</param>
    /// <returns>Devuelve si la victima del ataque ha fallecido o no</returns>
    public bool substractLife(Character victim, int damage)
    {
        bool dead = false;

        victim.life = victim.life - damage;

        dead = victim.life < 0;

        return dead;

    }
    /// <summary>
    /// Un atacante realiza un ataque sobre un objetivo
    /// 1. Se cálcula si hay Hit
    /// 2. Se calcula el daño
    /// </summary>
    /// <param name="attacker">Atacante</param>
    /// <param name="target"> Objetivo</param>
    /// <returns>El daño total recibido</returns>
    public int attack(Character attacker, Character target)
    {
        int damage = 0;

        //El atacante comprueba si acierta al objetivo
        if (this.getImpact(attacker, target))
        {
            //Una vez acertado calcula el daño
             damage = this.calculateDamage(attacker, target);
        }

        return damage;
    }
    /// <summary>
    /// Comprueba si algún personaje está realizando alguna acción
    /// </summary>
    /// <returns></returns>
    public bool checkCharacterPerforming()
    {
        bool isPerforming = false;

        if(battleCharacters.Count > 0)
        {
            foreach (var character in battleCharacters)
            {
                if(character.getState() == Character.CHARACTER_BATTLE_STATE.PERFORMING)
                {
                    isPerforming = true;
                    break;
                }
            }
        }

        return isPerforming;
    }
    public void executeAction(Character character)
    {
        switch(character.selectedAction.actionType)
        {
            case BattleAction.BATTLE_ACCTION_TYPE.BASIC_ATTACK:
                //Ataca al objetivo con un ataque básico cuerpo a cuerpo
                this.resolveBasicAttack(character);
                break;
        }
    }

    private int resolveBasicAttack(Character battleCharacter)
    {
        int resultDamage = this.attack(battleCharacter, battleCharacter.selectedAction.target);

        if(resultDamage > 0)
        {
            //Actualiza la vida del objetivo
            battleCharacter.selectedAction.target.life -= resultDamage;

            txtLog.text += "\n" + battleCharacter.txtName.text + " atacó a " + battleCharacter.selectedAction.target.txtName.text + " quitandole " + resultDamage + " de daño";

            //Si el daño le provoca la muerte (life <=0)
            if (battleCharacter.selectedAction.target.life <= 0)
            {
                battleCharacter.selectedAction.target.life = 0;
                battleCharacter.selectedAction.target.setState(Character.CHARACTER_BATTLE_STATE.DEAD);
                txtLog.text += "\n" + battleCharacter.selectedAction.target.txtName.text + " ha muerto";
            }

            //Actualiza la interfaz gráfica de la barra de vida del personaje
            battleCharacter.selectedAction.target.updateLifeBar();
        }
        else
        {
            txtLog.text += "\n" + battleCharacter.txtName.text + " ha fallado su ataque sobre " + battleCharacter.selectedAction.target.txtName.text;
        }

        return resultDamage;
    }

}
