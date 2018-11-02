using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Core.Skills
{
    public class FireBall : Skill
    {
        public FireBall() : base("FireBall", "Bola de fuego", ELEMENT_TYPE.FIRE, SCOPE.ONE_CHARACTER, 50, 95)
        {

        }
    }
}
