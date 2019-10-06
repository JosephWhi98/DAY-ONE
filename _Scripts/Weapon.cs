using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public enum Types{BRANCH, SPIKEDBAT, FIREAXE}
    public Types type;
    public int damage;
    public int range;
}
