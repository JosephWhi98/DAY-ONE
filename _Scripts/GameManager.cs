using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("General")]
    public CameraController camController;
    public UIManager uiManager; 


    [Header("Turn manager")]
    public bool playerTurn = true;
    public float turnCount;
    [SerializeField] int turnsPerDay; 


    [Header("Character Management")]
    public List<ControllableCharacter> ControllableCharacters = new List<ControllableCharacter>();
    public List<EnemyController> enemies = new List<EnemyController>();

    public UnityEngine.UI.Button endTurnButton;

    public bool allEnemiesDead;

    public GameObject Exit;
    public bool GameOver;

    public bool Won;
    public GameObject Victory; 

    public CombatLog combatLog;

    public Room[] rooms;
    public int roomIndex = 0;

    public bool started = false;
    public GameObject menu;
    public GameObject gameUI;

    public void StartGame()
    {
        started = true;
        menu.SetActive(false);
        gameUI.SetActive(true);
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this; 
        }

        roomIndex = 0; 
        Exit.SetActive(false);
        rooms[0].InitializeRoom();
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.N))
            NextRoom();

        if (ControllableCharacters.Count == 0)
        {
            GameOver = true;
            uiManager.GameOver.SetActive(true);
        }


        if (enemies.Count == 0)
        {
            allEnemiesDead = true;
            Exit.SetActive(true);
        }

        if(playerTurn && camController.currentlySelectedCharacter && camController.currentlySelectedCharacter.Action != null)
            endTurnButton.gameObject.SetActive(false);
        else if(playerTurn && (camController.currentlySelectedCharacter  && camController.currentlySelectedCharacter.Action == null))
            endTurnButton.gameObject.SetActive(true);
    }

    bool roomTransition;

    public void NextRoom()
    {
        if (!roomTransition && roomIndex + 1 < rooms.Length)
            StartCoroutine(nextRoomRoutine());
        else if (roomIndex + 1 >= rooms.Length)
        {
            Won = true; 
            Victory.SetActive(true);
        }
    }

    public IEnumerator nextRoomRoutine()
    {
        roomTransition = true;
        uiManager.FadeScreen(2f, Color.black);
        yield return new WaitForSeconds(2f);
        rooms[roomIndex].DisableRoom();
        roomIndex++;
        rooms[roomIndex].InitializeRoom();
        yield return new WaitForSeconds(0.5f);
        uiManager.FadeScreen(2f, Color.clear);
        roomTransition = false;
    }

    public void EndTurn()
    {
        camController.currentlySelectedCharacter = null;

        if (!GameOver)
        {
            if (!playerTurn || allEnemiesDead)
            {
                endTurnButton.gameObject.SetActive(true);
                StartPlayerTurn();
            }
            else
            {
                endTurnButton.gameObject.SetActive(false);
                StartEnemyTurn();
            }
        }
    }

    public void StartPlayerTurn()
    {
        foreach (ControllableCharacter character in ControllableCharacters)
        {
            character.pathChosen = false;
            character.hunger -= 1;
            character.energy = character.energyPerTurn;
        }

        turnCount++;

        playerTurn = true; 
    }

    public void StartEnemyTurn()
    {
        playerTurn = false; 
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator EnemyTurn()
    {
        foreach (EnemyController enemy in enemies)
        {
            enemy.energy = enemy.energyPerTurn;
            yield return StartCoroutine(enemy.PlayTurn());
        }

        yield return new WaitForSeconds(2f);

        EndTurn();
    }
}
