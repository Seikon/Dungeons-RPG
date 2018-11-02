using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IBehaviour
{
    BattleAction calculateAction(Character character);

    BattleAction elaborateStrategy(Character character);
}
