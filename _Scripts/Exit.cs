using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameManager.instance.camController.mouseText.text = "";
            GameManager.instance.camController.pathfindingTarget.transform.position = transform.position;
            GameManager.instance.camController.currentlySelectedCharacter.movingToExit = true;
            GameManager.instance.camController.currentlySelectedCharacter.pathChosen = true;
        }
    }

    private void OnMouseEnter() //TODO - Put this stuff in Clickable parent class??? Can be used for items etc. 
    {
        if (GameManager.instance.playerTurn && GameManager.instance.camController.currentlySelectedCharacter != null && GameManager.instance.camController.currentlySelectedCharacter.Action == null)
        {
            GameManager.instance.camController.mouseText.text = "CONTINUE";
        }
        GameManager.instance.camController.hoveringOverClickable = true;
    }

    private void OnMouseExit()
    {
        GameManager.instance.camController.mouseText.text = "";
        GameManager.instance.camController.hoveringOverClickable = false;
    }
}
