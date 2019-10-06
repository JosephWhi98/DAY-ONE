using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string tooltip = "Placeholder tool-tip";
    public Sprite icon;
    public bool isWeapon;

    public string itemName;

    private void OnMouseOver()
    {
        if (GameManager.instance.started)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (GameManager.instance.camController.currentlySelectedCharacter != null && !GameManager.instance.camController.currentlySelectedCharacter.pathChosen)
                {
                    if (GameManager.instance.camController.currentlySelectedCharacter.energy >= GameManager.instance.camController.currentlySelectedCharacter.grid.path.Count)
                    {
                        GameManager.instance.camController.mouseText.text = "";
                        GameManager.instance.camController.pathfindingTarget.transform.position = transform.position;
                        GameManager.instance.camController.currentlySelectedCharacter.movingToPickup = true;
                        GameManager.instance.camController.currentlySelectedCharacter.pathChosen = true;
                        GameManager.instance.camController.currentlySelectedCharacter.itemToPickup = this;
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
                GameManager.instance.camController.mouseText.text = "PICK UP";
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
}
