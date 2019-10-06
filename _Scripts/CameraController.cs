using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class CameraController : MonoBehaviour
{
    public ControllableCharacter currentlySelectedCharacter; 
    public Transform pathfindingTarget;
    bool followingMouse;
    public bool hoveringOverClickable;
    public bool hittingObsticle;

    public TMP_Text mouseText;

    private void Start()
    {
        mouseText.text = "";
    }

    void Update()
    {
        mouseText.transform.position = Input.mousePosition + new Vector3(30, -25, 0);

        if (GameManager.instance.playerTurn && !GameManager.instance.Won)
        {
            Plane plane = new Plane(Vector3.up, 0);

            if (currentlySelectedCharacter)
                currentlySelectedCharacter.pathfinder.FindPath(currentlySelectedCharacter.transform.position, pathfindingTarget.position);

            if (currentlySelectedCharacter && !currentlySelectedCharacter.pathChosen)
            {
                followingMouse = true;
            }
            else
            {
                followingMouse = false;
            }


            float dist;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out dist))
            {
                Vector3 point = ray.GetPoint(dist);

                if (currentlySelectedCharacter && !currentlySelectedCharacter.grid.NodeFromWorldPoint(point).walkable)
                    hittingObsticle = true;
                else
                    hittingObsticle = false;



                if (followingMouse)
                {
                    pathfindingTarget.transform.position = point;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && !hoveringOverClickable && currentlySelectedCharacter  && currentlySelectedCharacter.pathChosen == false && currentlySelectedCharacter.Action == null && currentlySelectedCharacter.grid.pathValid)
                {
                    pathfindingTarget.transform.position = point;
                    currentlySelectedCharacter.pathChosen = true;
                }

            }
        }
    }
}
