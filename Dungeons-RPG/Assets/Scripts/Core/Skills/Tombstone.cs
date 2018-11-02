using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Core.Skills
{
    public class Tombstone : Skill
    {
        public Tombstone() : base ("Tombstone", "Lápida", ELEMENT_TYPE.EARTH, SCOPE.ONE_CHARACTER, 25, 100)
        {
        
        }
    }
}
