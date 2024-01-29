using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public class CreatureScriptableObjectGenerator : MonoBehaviour
{
    [SerializeField] TextAsset CSVFile;     // contains the data for all creatures
    List<Creature> creatures = new List<Creature>();

    char lineSeperator = '\n';  // each line contains data for one ceature
    char fieldSeperator = ';';  // NOTE: fields 0:id, 1:name, 2:description, 3:location, 4/5:size, 6/7:strength, 8/9:agility, 10/11:charm, 12:rarity

    void Awake()
    {
        //split lines
        string[] creatureData = CSVFile.text.Split(lineSeperator);

        foreach (string alldata in creatureData)
        {
            //split fields
            string[] data = alldata.Split(fieldSeperator);

            if (!IsCreatureDataValid(data)) //skip for invalid/incomplete
                continue;

            //setup creature object
            Creature c = ScriptableObject.CreateInstance<Creature>();
            c.id = int.Parse(data[0]);
            c.artwork = GetCreatureSprite(data[0]);
            c.name = data[1];
            c.description = data[2];
            c.location = (Creature.Locations)Enum.Parse(typeof(Creature.Locations), data[3]);
            c.sizeMin = int.Parse(data[4]);
            c.sizeMax = int.Parse(data[5]);
            c.strengthMin = int.Parse(data[6]);
            c.strengthMax = int.Parse(data[7]);
            c.agilityMin = int.Parse(data[8]);
            c.agilityMax = int.Parse(data[9]);
            c.charmMin = int.Parse(data[10]);
            c.charmMax = int.Parse(data[11]);
            c.rarity = int.Parse(data[12]);

            //add creature to asset files
            string id = data[0];
            if (c.id < 10)
                id = "00" + id;
            else if (c.id < 100)
                id = "0" + id;

            if (!AssetDatabase.IsValidFolder("Assets/Creatures/"))
                AssetDatabase.CreateFolder("Assets", "Creatures");
            AssetDatabase.CreateAsset(c, "Assets/Creatures/" + id + ".asset");
            creatures.Add(c);
            Debug.Log("created creature asset " + id);
        }

        //save allCreatures to resources for in-game access
        AllCreatures allCreatures = ScriptableObject.CreateInstance<AllCreatures>();
        allCreatures.Creatures = creatures.ToArray();
        AssetDatabase.CreateAsset(allCreatures, "Assets/Resources/AllCreatures.asset");
        AssetDatabase.SaveAssets();
    }

    // Checks if all the data for a creature is valid
    private bool IsCreatureDataValid(string[] data)
    {

        if (data.Length < 13)   //skip for incomplete creature entries
        {
            Debug.Log($"CSV Field seperation is incorrect");
            return false;
        }

        foreach (string s in data)  //skip incomplete creature entries
            if (String.IsNullOrEmpty(s))
                return false;

        if (GetCreatureSprite(data[0]) == null) //skip for missing sprite
        {
            Debug.LogError($"Invalid sprite for creature number {data[0]}");
            return false;
        }

        Creature.Locations creatureLocation;
        if (!Enum.TryParse(data[3], out creatureLocation)) //skip for invalid location name
        {
            Debug.LogError($"Invalid location name in creature number {data[0]}");
            return false;
        }

        return true;
    }

    // Gets the sprite artwork from files
    private Sprite GetCreatureSprite(string id)
    {
        int intID = int.Parse(id);
        if (intID < 10)
            id = "00" + id;
        else if (intID < 100)
            id = "0" + id;
        Sprite artwork = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Creatures/" + id + ".png");
        return artwork;
    }
}
