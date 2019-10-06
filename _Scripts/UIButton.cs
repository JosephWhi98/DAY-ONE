using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
    public UnityEvent rightClickEvent;
    public UnityEvent leftClickEvent;
    bool hovering;

    public void Update()
    {
        if (hovering)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (GameManager.instance.camController.currentlySelectedCharacter != null && !GameManager.instance.camController.currentlySelectedCharacter.pathChosen && GameManager.instance.playerTurn)
                {
                    leftClickEvent.Invoke();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (GameManager.instance.camController.currentlySelectedCharacter != null && !GameManager.instance.camController.currentlySelectedCharacter.pathChosen && GameManager.instance.playerTurn)
                {
                    rightClickEvent.Invoke();
                }
            }
        }
    }


    public void MouseEnter() //TODO - Put this stuff in Clickable parent class??? Can be used for items etc. 
    {
        hovering = true;
        if (GameManager.instance.playerTurn && GameManager.instance.camController.currentlySelectedCharacter != null && GameManager.instance.camController.currentlySelectedCharacter.Action == null)
        {

        }
        GameManager.instance.camController.hoveringOverClickable = true;
    }

    public void MouseExit()
    {
        hovering = false;
        GameManager.instance.camController.mouseText.text = "";
        GameManager.instance.camController.hoveringOverClickable = false;
    }

}
