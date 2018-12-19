using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem {

    public string keyItemName;
    public int taken;

    public KeyItem(string newItemName, int newTaken)
    {
        this.keyItemName = newItemName;
        this.taken = newTaken;
    }

}
