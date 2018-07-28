﻿using UnityEngine;
using UnityEditor;

public class BattleRequest
{
    public enum STATE_BATTLE_REQUEST
    {
        NOTHING = 0,
        SELECT_ENEMY = 1,
        SELECT_ALL_ENEMIES = 2,
        SELECT_FRIEND = 3,
        SELECT_BAG_ITEM = 4,
        ATTENDED = 3
    }

    public enum MODE_BATTLE_REQUEST
    {
        LOGIC = 0,
        INTERACTIVE = 1
    }

    public STATE_BATTLE_REQUEST state;
    public MODE_BATTLE_REQUEST mode;

    public BattleRequest(STATE_BATTLE_REQUEST state, MODE_BATTLE_REQUEST mode)
    {
        this.state = state;
        this.mode = mode;
    }
}