using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Useable : Item
{
    public enum Type {BANDAGE};

    public Type type;

    public void OnUse(ControllableCharacter character)
    {
        if (type == Type.BANDAGE)
        {
            character.health += 5; 
        }
    }

}
