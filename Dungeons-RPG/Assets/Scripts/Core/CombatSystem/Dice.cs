﻿using UnityEngine;
using UnityEditor;

public class Dice
{
    private static int MIN = 1;
    private static int MAX = 100;

    public static int generateRandomNumber()
    {
        return Random.Range(Dice.MIN, Dice.MAX);
    }
    public static int generateRandomNumber(int min, int max)
    {
        return Random.Range(min, max);
    }
}