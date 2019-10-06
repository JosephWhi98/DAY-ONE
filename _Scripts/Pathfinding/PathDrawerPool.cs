using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawerPool : MonoBehaviour
{
    public List<GameObject> PathDrawer = new List<GameObject>();
    public Material validPath;
    public Material invalidPath; 

    public GameObject requestPathDrawer(Vector3 position)
    {
        GameObject obj = null;

        if (PathDrawer.Count > 0)
        {
            obj = PathDrawer[0];
            PathDrawer.RemoveAt(0);

            obj.transform.position = position;
            obj.SetActive(true);
        }
        return obj;
    }

    public void returnPathDrawer(GameObject obj)
    {
        PathDrawer.Add(obj);
        obj.SetActive(false);
    }
}
