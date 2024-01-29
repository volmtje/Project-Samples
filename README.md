# Project Samples

In diesem Repository liegt ein Unity Project mit zwei Codeauszügen aus meinen letztzen Projekten.

1. Den Auszug unter dem Namen "*PolygonMap*", habe ich im Rahmen eines GameJams erstellt. Das Ziel war es den **Screen in eine beliebige Zahl von Polygonen zu unterteilen**, die im Weiteren als Karte für ein strategisches Battle Game dienen sollte.   
Für die [PolygonCalculator](Assets/Scripts/PolygonMap/PolygonCalculator.cs) Klasse existieren außerdem ein paar exemplarische [Tests](Assets/Tests/PolygonMapTests/PolygonCalculatorTests.cs). Für die Weitere Nutzung wären die allerdings noch ausbaubar.

1. Der zweite Teil mit dem Namen "*CreatureObjectGenereator*" stammt aus meinem aktuellen Bachelor Projekt. Ich habe ihn geschrieben um Zeit und Aufwand zu sparen, indem ich die **Erstellung und Verarbeitung von größeren Mengen an Scriptable Objects automatisiere**.

Beide Ausschnitte lassen sich über die jeweiligen Szenen im Projekt ausführen.

##### Anmerkung: Dieses Repo ist eher zweckdienlich erstellt und nicht repräsentativ für meinen Umgang mit GIT :)

## Polygon Map

#### Das Problem

Der Bildschirm sollte zufällig in eine beliebige Anzahl von Polygonen unterteilt und diese dann visuell abgebildet werden. Die größte Schwierigkeit lag dabei in der zufälligen geometrischen Konstruktion.

#### Mein Lösungsansatz

Ich habe mich nach einigem Rumprobieren und etwas Recherche schließlich am Voronoi-Diagramm orientiert und als Basis den Algorithmus von Fortune gewählt. 

Vom Leeren Screen zu den fertigen Mehses in der Szene habe ich folgenden Workflow erarbeitet:
1. Unterteilung des Screens mit dem Algorithmus von Fortune - [FortunesAlgorithm.cs](Assets/Scripts/PolygonMap/FortunesAlgorithm.cs)
1. Die Kanten des Voronigraphen auf den Bildschim anpassen - [FortuneEdgeExtractor.cs](Assets/Scripts/PolygonMap/FortuneEdgeExtractor.cs)
2. Aus den Kanten Polygone mit Eckpunkten berechnen - [PolygonCalculator.cs](Assets/Scripts/PolygonMap/PolygonCalculator.cs)
3. Meshes der Polygone generieren - [PolygonMeshGenerator.cs](Assets/Scripts/PolygonMap/PolygonMeshGenerator.cs)

Die Implementierung des Basisalgorithmus im Skript [FortunesAlgorithm.cs](Assets/Scripts/PolygonMap/FortunesAlgorithm.cs) selbst stammt aus Zeitgründen von [Wikipedia](https://de.m.wikipedia.org/wiki/Voronoi-Diagramm#Algorithmus_von_Fortune), den ich für meine Zwecke angepasst habe.

In der weiteren Umsetzung kann man die Verteilung der Punkte spezifizieren um die Varianz in der Fläche der Polygone gering zu halten. Für Bewegungsregeln zwischen den Feldern lässt sich recht einfach ein Graf erstellen, der benachbarte Polygone kennt.


## Scriptable Object Generator

#### Das Problem:   
Ich habe ca. 190 Kreaturen (etwas wie Sammelobjekte) mit Namen, ID, Bild, Bescheribung, Stats etc., was händisch unsinnig viel Arbeit zu managen ist, besonders, wenn ich z.B. einen Stat hinzufügen möchte. Zudem will ich die Kreaturen in thematische Enzyklopädien unterteilen, die ich zur Runtime abrufen, verändern und abspeichern können muss.

#### Mein Lösungsansatz:   
- Das **Erstellen der Assets** habe ich in einem Editorskript gelöst (siehe [CreatureScriptableObjectGenerator.cs](Assets/Scripts/CreatureObjectGenerator/CreatureScriptableObjectGenerator.cs)). Für textbasierte Daten pflege ich eine CSV Datei und die Sprites rufe ich über ID-Nummern auf.   
Um die erstellten Objecte im Spiel möglichst effizient abrufen zu können erstelle ich ein zusätzliches ScriptableObject mit allen Referenzen, das ich über die Resourcen aufrufen kann.

- Für das **Speichern der Enzyklopädien** habe ich mich für JSON-Datein entschieden um meine Datenstrukturen unabhängig vom Spiel speichern zu können. In einem weiteren Editorskript (siehe [EncyclopediaAssetGenerator.cs](Assets/Scripts/CreatureObjectGenerator/EncyclopediaAssetsGenerator.cs)) generiere ich die verschiedenen Dateien direkt zusammen mit den Kreaturen. So spare ich mir viel Ärger mit der Zuweisung in der Engine und zu vollen Szenen.

Beim Starten der zugehörigen Demoszene (`CreatureObjectGenerator.unity`) werden die beiden Skripte augeführt, anschließend lassen sich die generierten Assets in den Ordnern "*Assets/Creatures*" und im Resourcefolder finden.