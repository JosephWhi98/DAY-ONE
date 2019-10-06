using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class CombatLog : MonoBehaviour
{
    public List<TMP_Text> Updates = new List<TMP_Text>();
    public List<TMP_Text> TextBoxes = new List<TMP_Text>();
    public Transform[] UpdateSlots;

    public void PostUpdate(string message)
    {
        TMP_Text text = TextBoxes[0];
        TextBoxes.RemoveAt(0);
        text.text = message.ToUpper();
        text.gameObject.SetActive(true);
        Updates.Insert(0, text);

        int count = 0;

        foreach (TMP_Text t in Updates)
        {
            t.transform.position = UpdateSlots[count].position;
            count++;
        }

        if (Updates.Count >= 5)
        {
            Updates[Updates.Count - 1].gameObject.SetActive(false);
            TextBoxes.Add(Updates[Updates.Count - 1]);
            Updates.RemoveAt(Updates.Count - 1);
        }
    }
}
