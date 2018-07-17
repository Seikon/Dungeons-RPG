using UnityEngine;
using System.Collections;

public class Battle : MonoBehaviour
{
    public static int DEFAULT_PRECISION = 95;    
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
}
