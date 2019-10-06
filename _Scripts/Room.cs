using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Pathfinding pathfinder;
    public Grid grid;
    public Transform exitTransform;
    public List<EnemyController> enemies = new List<EnemyController>();
    public ControllableCharacter newCharacter;

    public Transform[] spawns;

    public void InitializeRoom()
    {
        if (GameManager.instance.roomIndex != 0)
        {
            foreach (Node n in GameManager.instance.rooms[GameManager.instance.roomIndex - 1].grid.path)
            {
                if (n.drawer)
                {
                    GameManager.instance.rooms[GameManager.instance.roomIndex - 1].grid.pathPool.returnPathDrawer(n.drawer);
                }
            }
        }

        if (newCharacter)
        {
            GameManager.instance.ControllableCharacters.Add(newCharacter);
            GameManager.instance.combatLog.PostUpdate(newCharacter.characterName + " has joined your party!");
            newCharacter.transform.SetParent(null);
        }

        int count = 0;
        foreach (ControllableCharacter character in GameManager.instance.ControllableCharacters)
        {
            character.energy = character.energyPerTurn;
            character.transform.position = spawns[count].position;
            count++;
            character.pathfinder = pathfinder;
            character.grid = grid; 
        }

        GameManager.instance.Exit.transform.position = exitTransform.position;
        GameManager.instance.Exit.transform.rotation = exitTransform.rotation;
        GameManager.instance.Exit.SetActive(false);
        GameManager.instance.enemies = enemies;
        GameManager.instance.allEnemiesDead = false;

        

        gameObject.SetActive(true);
    }

    public void DisableRoom()
    {
        gameObject.SetActive(false);
    }
}
