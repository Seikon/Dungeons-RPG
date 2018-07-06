using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
/// <summary>
/// Clase para guardar / cargar la partida actual del jugador
/// Cada partida o irá almacenado en un bloque de guardado,
/// cada bloque de guardado tendrá un fichero asociado designado de la siguiente manera:
/// gameState + id (número de bloque 1,2,3..) + .gd 
/// Ejemplo: gameState1.gd
/// Ejemplo: gameState2.gd
/// Habrá un número máximo de bloques donde se podrá guardar las partidas, 
/// este límite viene definido por el campo MAXIMUM_BLOCKS
/// </summary>
public static class StateGameController
{
    public static List<GameState> blocks = new List<GameState>();
    public const int MAXIMUM_BLOCKS = 3;
    private const string SAVED_GAMES_PATH = "/gameState";

    /// <summary>
    /// Método para guardar la partida de manera persistente
    /// </summary>
    /// <param name="game">Estado de la partida actual, la que se va a almacenar</param>
    /// <param name="blockId">Número de bloque donde guardará la partida</param>
    /// <returns>Devuelve el Id de bloque ocupado por la partida</returns>
    public static int saveGame(GameState game, int blockId)
    {
            if (blockId > MAXIMUM_BLOCKS)
            {
                throw new Exception("El id de bloque proporcionado no es correcto");
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                // Para cada partida se asigna un fichero distinto
                // cuyo nombre viene compuesto por gameState + id (número de bloque 1,2,3..) + .gd Ejemplo: gameState1.gd
                FileStream file = File.Create(Application.persistentDataPath + SAVED_GAMES_PATH + blockId + ".gd");
                bf.Serialize(file, game);
                file.Close();

                return blockId;
            }
        }
    
    public static GameState loadGame(int blockId)
    {
        if(blockId > MAXIMUM_BLOCKS)
        {
            throw new Exception("El id de bloque proporcionado no es correcto");
        }
        else
        {
            if (File.Exists(Application.persistentDataPath + SAVED_GAMES_PATH + blockId + ".gd"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + SAVED_GAMES_PATH + blockId + ".gd", FileMode.Open);
                GameState savedGame = (GameState)bf.Deserialize(file);
                file.Close();
                return savedGame;
            }
            else
            {
                throw new Exception("La partida contenida en el bloque " + blockId + "no existe");
            }
        }
    }

    private static List<GameState> getSavedGames()
    {
        List<GameState> savedGames = new List<GameState>();

        for (int indGame = 0; indGame < MAXIMUM_BLOCKS; indGame++)
        {
            // Obtiene la partida cuando exista
            if (File.Exists(Application.persistentDataPath + SAVED_GAMES_PATH + indGame + ".gd"))
            {
                savedGames.Add(loadGame(indGame));
            }
        }

        return savedGames;
    }

}
