using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EncyclopediaAssetsGenerator : MonoBehaviour
{
    void Start()
    {
        //load allcreatures object reference
        AssetDatabase.Refresh(); //make sure assetdatabase has recent changes
        AllCreatures allCreatures = AssetDatabase.LoadAssetAtPath<AllCreatures>("Assets/Resources/AllCreatures.asset"); // contains a list of all ceatures
        
        if (allCreatures == null)
        {
            Debug.LogError("missing allCreatures scriptable object in resources");
            return;
        }

        //setup dictionary for encyclopedias by location
        Dictionary<Creature.Locations, SerializableEncyclopedia> allEncyclos = new Dictionary<Creature.Locations, SerializableEncyclopedia>();

        SerializableEncyclopedia Plains = new SerializableEncyclopedia();
        SerializableEncyclopedia Forest = new SerializableEncyclopedia();
        SerializableEncyclopedia Mountains = new SerializableEncyclopedia();
        SerializableEncyclopedia MountainPeak = new SerializableEncyclopedia();
        SerializableEncyclopedia Ocean = new SerializableEncyclopedia();
        SerializableEncyclopedia Lake = new SerializableEncyclopedia();
        SerializableEncyclopedia Sand = new SerializableEncyclopedia();
        SerializableEncyclopedia Vulcano = new SerializableEncyclopedia();
        SerializableEncyclopedia Cave = new SerializableEncyclopedia();

        allEncyclos.Add(Creature.Locations.Plains, Plains);
        allEncyclos.Add(Creature.Locations.Forest, Forest);
        allEncyclos.Add(Creature.Locations.Mountains, Mountains);
        allEncyclos.Add(Creature.Locations.MountainPeak, MountainPeak);
        allEncyclos.Add(Creature.Locations.Ocean, Ocean);
        allEncyclos.Add(Creature.Locations.Sand, Sand);
        allEncyclos.Add(Creature.Locations.Vulcano, Vulcano);
        allEncyclos.Add(Creature.Locations.Cave, Cave);
        allEncyclos.Add(Creature.Locations.Lake, Lake);
        
        foreach(var e in allEncyclos)
        {
            e.Value.encyclopedia = new List<EncyclopediaEntry>();
        }

        //add creatures to the according dictionary (by location)
        foreach (Creature c in allCreatures.Creatures)
        {
            EncyclopediaEntry entry = new EncyclopediaEntry(c.id);
            allEncyclos[c.location].encyclopedia.Add(entry);
        }

        //save encyclopedias to resources
        foreach (var s in allEncyclos)
        {
            string jsonDataString = JsonUtility.ToJson(s.Value, false);
            TextAsset jsonEncyclopedia = new TextAsset(jsonDataString);

            string filename = s.Key.ToString() + ".asset";  //save by name of the location
            if (!AssetDatabase.IsValidFolder("Assets/Resources/encyclopedia"))
                AssetDatabase.CreateFolder("Assets/Resources", "encyclopedia");
            AssetDatabase.CreateAsset(jsonEncyclopedia, "Assets/Resources/encyclopedia/" + filename);
        }
    }
}
