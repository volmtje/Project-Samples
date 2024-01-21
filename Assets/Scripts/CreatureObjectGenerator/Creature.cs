using System;
using UnityEngine;

//foreach different type of creature exists one of these creature scriptable objects
public class Creature : ScriptableObject
{
    public new string name;
    public int id;
    public string description;  // to be seen in detail view

    public Sprite artwork;      //image repreentation of the creature

    public Locations location;  // each creature is associated with exactly one location

    //more specs to be seen in detail view
    public int sizeMin;         
    public int sizeMax;
    public int strengthMin;
    public int strengthMax;
    public int agilityMin;
    public int agilityMax;
    public int charmMin;
    public int charmMax;

    public int rarity;          //used to colculate probibility of discovery



    //the locations at which creatures can be found
    [Serializable] public enum Locations   
    {
        Ocean,
        Lake,
        Sand,
        Plains,
        Forest,
        Mountains,
        MountainPeak,
        Vulcano,
        Cave
    }
}