using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Parsable class to contain a list of encyclopedia eintries
[Serializable]
class SerializableEncyclopedia
{
    public List<EncyclopediaEntry> encyclopedia;
}

// A serializable class for encyclopedia info
[Serializable]
public class EncyclopediaEntry
{
    public int creatureId;     // id of the creature associated with this entry

    public bool encountered;    // is true if the associated creature has been dicovered in game (false by default)
    public int encounters;      // the number of times, the associated creature has been encountered in game

    public EncyclopediaEntry(int id)
    {
        creatureId = id;
        encountered = false;
        encounters = 0;
    }
}
