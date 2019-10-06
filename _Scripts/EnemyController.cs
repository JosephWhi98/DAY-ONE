using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Pathfinding")]
    public bool pathChosen;
    [SerializeField] Pathfinding pathfinder;
    [SerializeField] public Grid grid;
    [SerializeField] float walkSpeed = 1f;
    bool movingToNode;

    [Header("General Character Info")]
    public string characterName;
    public int energyPerTurn = 5; //This may vary for different characters??
    public int energy = 5;
    public int health = 10;
    public float attackRange = 3;
    public float damage;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip footstep;
    public AudioClip hurt;

    ControllableCharacter target; 

    private void Update()
    {
        energy = Mathf.Clamp(energy, 0, energyPerTurn);

        if (health <= 0)
        {
            GameManager.instance.enemies.Remove(this);

            if (GameManager.instance.enemies.Count == 0)
            {
                foreach (ControllableCharacter character in GameManager.instance.ControllableCharacters)
                {
                    character.energy = character.energyPerTurn;
                }
            }

            Destroy(gameObject);
        }
    }


    public IEnumerator PlayTurn()
    {
        float minDist = 100000; 
        foreach (ControllableCharacter character in GameManager.instance.ControllableCharacters)
        {
            float i = Vector3.Distance(transform.position, character.transform.position);

            if (i < minDist)
            {
                minDist = i; 
                target = character;
            }
        }

        pathfinder.seeker = transform; 
        pathfinder.target = target.transform; 

        while (energy > 0)
        {
            pathfinder.FindPath(transform.position, target.transform.position);
            Debug.Log(Vector3.Distance(transform.position, target.transform.position));
            if (Vector3.Distance(transform.position, target.transform.position) < attackRange)
            {
                StartCoroutine(Attack());
            }
            else
            {
                if (grid.path.Count > 0)
                    yield return StartCoroutine(WalkToNode(grid.path[0]));
            }
            


            yield return null;
        }
    }

    void SetFloorPosition()
    {
        RaycastHit hit;

        Vector3 rayPos = transform.position;
        rayPos.y += 1;
        if (Physics.Raycast(rayPos, -transform.up, out hit, 2))
        {
            if (hit.collider.gameObject.layer == 9)
            {
                Vector3 newPos = transform.position;
                newPos.y = hit.point.y + 0.1f;

                transform.position = newPos;
            }
        }


    }

    private void OnMouseOver()
    {
        if (GameManager.instance.started)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (GameManager.instance.camController.currentlySelectedCharacter != null && !GameManager.instance.camController.currentlySelectedCharacter.pathChosen)
                {
                    if (GameManager.instance.camController.currentlySelectedCharacter.energy - 2 >= GameManager.instance.camController.currentlySelectedCharacter.grid.path.Count)
                    {
                        GameManager.instance.camController.mouseText.text = "";
                        GameManager.instance.camController.pathfindingTarget.transform.position = transform.position;
                        GameManager.instance.camController.currentlySelectedCharacter.movingToAttack = true;
                        GameManager.instance.camController.currentlySelectedCharacter.pathChosen = true;
                        GameManager.instance.camController.currentlySelectedCharacter.enemyToAttack = this;
                    }
                }
            }
        }
    }

    private void OnMouseEnter() //TODO - Put this stuff in Clickable parent class??? Can be used for items etc. 
    {
        if (GameManager.instance.started)
        {
            if (GameManager.instance.playerTurn && GameManager.instance.camController.currentlySelectedCharacter != null && GameManager.instance.camController.currentlySelectedCharacter.Action == null)
            {
                GameManager.instance.camController.mouseText.text = "ATTACK";
            }
            GameManager.instance.camController.hoveringOverClickable = true;
        }
    }

    private void OnMouseExit()
    {
        GameManager.instance.camController.mouseText.text = "";
        GameManager.instance.camController.hoveringOverClickable = false;
    }

    private void OnDestroy()
    {
        OnMouseExit();
    }

    IEnumerator Attack()
    {
        energy = 0;
        transform.LookAt(target.transform);
        target.transform.LookAt(transform);
        target.animator.SetTrigger("Hit");

        target.audioSource.clip = target.hurt;
        target.audioSource.Play();

        yield return new WaitForSeconds(1.5f);

        int hitDamage = (int)(damage * Random.Range(0.5f,1.5f));
        target.health -= hitDamage;

        GameManager.instance.combatLog.PostUpdate(characterName + " hit " + target.characterName + " for " + hitDamage + " damage");
    }

    IEnumerator WalkToNode(Node n)
    {
        energy -= 1;

        audioSource.clip = footstep;
        audioSource.pitch = Random.Range(1f, 1.1f);
        audioSource.Play();

        movingToNode = true;
        Vector3 targetPosition = n.worldPosition;
        targetPosition.y = transform.position.y;
        transform.LookAt(targetPosition);
        float t = 0;
        while (transform.position != targetPosition)
        {
            t += Time.deltaTime / walkSpeed;

            transform.position = Vector3.Lerp(transform.position, targetPosition, t);

            yield return null;
        }

        if (grid.path.Count == 0)
        {
            pathChosen = false;
        }


        SetFloorPosition();

        movingToNode = false;
    }

}
