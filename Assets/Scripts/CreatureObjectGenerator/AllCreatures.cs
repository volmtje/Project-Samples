using UnityEngine;


// this class is used as a container for all creature objects, to be accessible via resources
public class AllCreatures : ScriptableObject
{
    public Creature[] Creatures;

    ///<summary>retruns the creature object with the given id</summary>
    public Creature GetCreaturebyID(int id)
    {
        return Creatures[id];
    }
}
