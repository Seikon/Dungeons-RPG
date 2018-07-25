using UnityEngine;
using System.Collections;

public abstract class Item
{
    public string name;

    public abstract void use(Character character);
}
