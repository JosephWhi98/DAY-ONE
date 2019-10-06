using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableCharacter : MonoBehaviour
{

    [Header("Pathfinding")]
    public bool pathChosen;
    [SerializeField] Transform target; 
    [SerializeField] public Pathfinding pathfinder;
    [SerializeField] public Grid grid;
    [SerializeField] float walkSpeed = 1f;
    bool movingToNode;
    public bool movingToAttack;
    public EnemyController enemyToAttack;
    public bool movingToPickup;
    public Item itemToPickup;
    public bool movingToExit;

    [Header("General Character Info")]
    public string characterName;
    public int energyPerTurn = 15; //This may vary for different characters??
    public int energy = 15;
    public int health = 10;
    public int hunger = 10;
    public int damage = 2;

    public int attackRange = 3;

    public Animator animator;


    public Weapon weapon;
    public Useable item; 

    public Coroutine Action;

    int defaultRange;
    int defaultHealth;
    int defaultDamage;

    [Header("AUDIO")]
    public AudioSource audioSource;
    public AudioClip footstep;
    public AudioClip hurt;
    public AudioClip pickUp;

    [Header("Held Weapons")]
    public GameObject spikedBatObject;
    public GameObject branchObject;
    public GameObject fireAxe;


    private void Start()
    {
        Action = null;
        defaultRange = attackRange;
        defaultHealth = health;
        defaultDamage = damage; 
    }


    private void Update()
    {
        energy = Mathf.Clamp(energy, 0, energyPerTurn);

        if (health > defaultHealth)
            health = defaultHealth;

        if (energy > energyPerTurn)
            energy = energyPerTurn; 

        if (energy == 0)
            pathChosen = false;

        if (pathChosen && !movingToNode && grid.path.Count > 0 && energy > 0 && Action == null)
        {
            if (movingToAttack)
            {
                if (Vector3.Distance(transform.position, target.position) > attackRange)
                {
                    Action = StartCoroutine(WalkToNode(grid.path[0]));
                }
                else
                {
                    Action = StartCoroutine(AttackEnemy());
                }
            }
            else if (movingToPickup)
            {
                if (Vector3.Distance(transform.position, target.position) > attackRange)
                {
                    Action = StartCoroutine(WalkToNode(grid.path[0]));
                }
                else
                {
                    PickupItem();
                    movingToPickup = false;
                    pathChosen = false;
                }
            }
            else if (movingToExit)
            {
                if (Vector3.Distance(transform.position, GameManager.instance.Exit.transform.position) > attackRange)
                {
                    Action = StartCoroutine(WalkToNode(grid.path[0]));
                }
                else
                {
                    movingToExit = false;
                    pathChosen = false;
                    GameManager.instance.NextRoom();
                }
            }
            else
            {

                Action = StartCoroutine(WalkToNode(grid.path[0]));
            }
        }

        if (health <= 0)
        {
            GameManager.instance.ControllableCharacters.Remove(this);
            GameManager.instance.combatLog.PostUpdate(characterName + " has died a painful death!");
            Destroy(gameObject);
        }

    }

    public void UseItem()
    {
        item.OnUse(this);
        Destroy(item.gameObject);
        item = null;
        GameManager.instance.uiManager.SetItemImage(null);
        audioSource.clip = pickUp;
        audioSource.Play();
    }

    public void DropWeapon()
    {
        attackRange = defaultRange;
        damage = defaultDamage;
        weapon.transform.position = this.transform.position;
        weapon.gameObject.SetActive(true);
        weapon.transform.SetParent(GameManager.instance.rooms[GameManager.instance.roomIndex].transform);
        GameManager.instance.combatLog.PostUpdate(characterName + " dropped " + weapon.itemName);
        weapon = null;
        audioSource.clip = pickUp;
        audioSource.Play();
        GameManager.instance.uiManager.SetWeaponImage(null);

        spikedBatObject.SetActive(false);
        branchObject.SetActive(false);
        fireAxe.SetActive(false);
    }

    public void DropItem()
    {
        item.transform.position = this.transform.position;
        item.gameObject.SetActive(true);
        item.transform.SetParent(GameManager.instance.rooms[GameManager.instance.roomIndex].transform);
        GameManager.instance.combatLog.PostUpdate(characterName + " dropped " + item.itemName);
        item = null;
        audioSource.clip = pickUp;
        audioSource.Play();
        GameManager.instance.uiManager.SetItemImage(null);
    }

    void PickupItem()
    {
        if (itemToPickup.GetType() == typeof(Weapon))
        {
            if (!weapon)
            {
                audioSource.clip = pickUp;
                audioSource.Play();
                weapon = itemToPickup.GetComponent<Weapon>();
                itemToPickup.gameObject.SetActive(false);
                itemToPickup.transform.parent = transform;

                damage = weapon.damage;
                attackRange = weapon.range;
                GameManager.instance.uiManager.SetWeaponImage(weapon.icon);

                GameManager.instance.combatLog.PostUpdate(characterName + " picked up " + weapon.itemName);

                spikedBatObject.SetActive(weapon.type == Weapon.Types.SPIKEDBAT);
                branchObject.SetActive(weapon.type == Weapon.Types.BRANCH);
                fireAxe.SetActive(weapon.type == Weapon.Types.FIREAXE);


                return;
            }
        }
        else if (itemToPickup.GetType() == typeof(Useable))
        {
            if (!item)
            {
                audioSource.clip = pickUp;
                audioSource.Play();
                item = itemToPickup.GetComponent<Useable>();
                itemToPickup.gameObject.SetActive(false);
                itemToPickup.transform.parent = transform;
                GameManager.instance.uiManager.SetItemImage(item.icon);

                GameManager.instance.combatLog.PostUpdate(characterName + " picked up " + item.itemName);
                return;
            }
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

    private void OnMouseEnter() //TODO - Put this stuff in Clickable parent class??? Can be used for items etc. 
    {
        if (GameManager.instance.started)
        {
            if (GameManager.instance.playerTurn && GameManager.instance.camController.currentlySelectedCharacter == null)
            {
                GameManager.instance.camController.mouseText.text = "SELECT";
            }
            GameManager.instance.camController.hoveringOverClickable = true;
        }
    }

    private void OnMouseExit()
    {
        GameManager.instance.camController.mouseText.text = "";
        GameManager.instance.camController.hoveringOverClickable = false;
    }


    private void OnMouseOver()
    {
        if (GameManager.instance.started)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && GameManager.instance.playerTurn)
            {
                if (GameManager.instance.camController.currentlySelectedCharacter == null || !GameManager.instance.camController.currentlySelectedCharacter.pathChosen)
                {
                    GameManager.instance.camController.mouseText.text = "";
                    GameManager.instance.camController.currentlySelectedCharacter = this;
                    pathfinder.seeker = transform;
                    pathfinder.target = GameManager.instance.camController.pathfindingTarget;

                    if (weapon)
                        GameManager.instance.uiManager.SetWeaponImage(weapon.icon);
                    else
                        GameManager.instance.uiManager.SetWeaponImage(null);

                    if (item)
                        GameManager.instance.uiManager.SetItemImage(item.icon);
                    else
                        GameManager.instance.uiManager.SetItemImage(null);
                }
            }
        }
    }

    public IEnumerator AttackEnemy()
    {
        if (enemyToAttack)
        {
            transform.LookAt(enemyToAttack.transform);
            enemyToAttack.transform.LookAt(transform);
            animator.SetTrigger("Attack");
            energy -= 2;

            enemyToAttack.audioSource.clip = enemyToAttack.hurt;
            enemyToAttack.audioSource.Play();

            yield return new WaitForSeconds(1f);

            int damage = (int)(this.damage * Random.Range(0.8f, 2f));
            enemyToAttack.health -= damage;

            GameManager.instance.combatLog.PostUpdate(characterName + " hit " + enemyToAttack.characterName + " for " + damage + " damage");

            movingToAttack = false;
            enemyToAttack = null;
            pathChosen = false;
        }
        Action = null; 
    }


    IEnumerator WalkToNode(Node n)
    {
        if(!GameManager.instance.allEnemiesDead)
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
        Action = null;
    }
    
}
