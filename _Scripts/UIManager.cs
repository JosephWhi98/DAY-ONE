using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject infoBox;

    public TMP_Text nameText;
    public TMP_Text healthText;
    public TMP_Text energyText;

    public TMP_Text turnText;
    public TMP_Text turnTitleText;

    public UnityEngine.UI.Image itemImage;
    public UnityEngine.UI.Image weaponImage;


    public GameObject GameOver;

    public UnityEngine.UI.Image fade;

    private void Awake()
    {
        fade.color = Color.black;
    }

    private void Start()
    {
        FadeScreen(5f, Color.clear);
    }

    public void FadeScreen(float t, Color col)
    {
        StartCoroutine(FadeScreenRoutine(t, col));
    }

    IEnumerator FadeScreenRoutine(float time, Color col)
    {
        float t = 0;

        while (fade.color != col)
        {
            t += Time.deltaTime / time;
            fade.color = Color.Lerp(fade.color, col, t);
            yield return null;
        }
    }

    private void Update()
    {
        if (GameManager.instance.playerTurn)
            turnTitleText.text = "PLAYER TURN";
        else
            turnTitleText.text = "ENEMY TURN";

        turnText.text = "TURN " + GameManager.instance.turnCount;

        if (GameManager.instance.camController.currentlySelectedCharacter)
        {
            infoBox.SetActive(true);
            
            nameText.text = GameManager.instance.camController.currentlySelectedCharacter.characterName.ToUpper();
            energyText.text = "ENERGY: " + GameManager.instance.camController.currentlySelectedCharacter.energy;
            healthText.text = "HEALTH: "  + GameManager.instance.camController.currentlySelectedCharacter.health;
        }
        else
        {
            infoBox.SetActive(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("mainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetItemImage(Sprite sprite)
    {
        itemImage.sprite = sprite;

        if (sprite == null)
        {
            itemImage.gameObject.SetActive(false);
        }
        else
        {
            itemImage.gameObject.SetActive(true);
        }
    }

    public void SetWeaponImage(Sprite sprite)
    {
        weaponImage.sprite = sprite;

        if(sprite == null)
        {
            weaponImage.gameObject.SetActive(false);
        }
        else
        {
            weaponImage.gameObject.SetActive(true);
        }
    }

    public void UseItem()
    {
        if(GameManager.instance.camController.currentlySelectedCharacter.item)
            GameManager.instance.camController.currentlySelectedCharacter.UseItem();
    }

    public void DropItem()
    {
        if (GameManager.instance.camController.currentlySelectedCharacter.item)
            GameManager.instance.camController.currentlySelectedCharacter.DropItem();
    }

    public void DropWeapon()
    {
        if (GameManager.instance.camController.currentlySelectedCharacter.weapon)
            GameManager.instance.camController.currentlySelectedCharacter.DropWeapon();
    }
}
