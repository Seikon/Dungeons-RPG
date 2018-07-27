using UnityEngine;
using System.Collections;

public abstract class Item
{
    public string name;
    protected string ITEM_NAME = "";

    public abstract void use(Character character);
}
