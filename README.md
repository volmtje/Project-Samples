# Project Samples

In diesem Repository liegt ein Unity Project mit zwei Codeauszügen aus meinen letztzen Projekten.

1. Den Auszug unter dem Namen "*PolygonMap*", habe ich im Rahmen eines GameJams erstellt. Das Ziel war es den **Screen in eine beliebige Zahl von Polygonen zu unterteilen**, die im Weiteren als Karte für einen strategisches Battle Game dient. Für ein bisschen mehr Kontext habe ich zusätzlich für die [PolygonCalculator](Assets/Scripts/PolygonMap/PolygonCalculator.cs) Klasse ein paar exemplarische [Tests](Assets/Tests/PolygonMapTests/PolygonCalculatorTests.cs) geschrieben.

2. Der zweite Teil mit dem Namen "*CreatureObjectGenereator*" stammt aus meinem aktuellen Bachelor Projekt. Ich habe ihn um Zeit und Aufwand zu spahren für die **automatisierte Erstellung und Verarbeitung von größeren Mengen an Scriptable Objects** geschrieben, die ich regelmäßig überarbeiten muss.

Beide Ausschnitte lassen sich über die jeweiligen Szenen ausführen.

Anmerkung: Bitte nehmt dieses Repo nicht als Maßstab für meinen Umgang mit GIT -  Es ist eher zweckdienlich erstellt :)

## Polygon Map

#### Das Problem

Der Bildschirm sollte zufällig in eine beliebige Anzahl von Polygonen unterteilt werden und diese dann visuell abgebildet werden. Die größte Schwierigkeit lag dabei in der zufälligen geometrischen Konstruktion.

#### Mein Lösungsansatz

Ich habe mich nach einigem Rumprobieren und etwas Recherche schließlich am Voronoi-Diagram orientiert und als Basis den Algorithmus von Fortune gewählt. Die Implementierung des Algorithmus (siehe [FortunesAlgorith.cs](Assets/Scripts/PolygonMap/FortunesAlgorithm.cs)) selbst stammtaus Zeitgründen von [Wikipedia](https://de.m.wikipedia.org/wiki/Voronoi-Diagramm#Algorithmus_von_Fortune) und enthält nur leichte Veränderungen, für meine Zwecke.

Die vom Algorithmus erstellten Kanten und Punkte habe ich Schrittweise so Verarbeitet, dass sich das Format schließlich zum erstellen von Meshes geeignet hat:
1. Die Kanten des Voronigraphen auf den Bildschim anpassen - [FortuneEdgeExtractor.cs](Assets/Scripts/PolygonMap/FortuneEdgeExtractor.cs)
2. Aus den Kanten Polygone mit Eckpunkten berechnen - [PolygonCalculator.cs](Assets/Scripts/PolygonMap/PolygonCalculator.cs)
3. Meshes generieren - [PolygonMeshGenerator.cs](Assets/Scripts/PolygonMap/PolygonMeshGenerator.cs)

In der weiteren Umsetzung kann man die Verteilung der Punkte spezifizieren um die Varianz in der Fläche der Polygone gering zu halten. Für Bewegungsregeln zwischen den Feldern lässt sich recht einfach ein Graf erstellen, der benachbarte Polygone kennt.


## Scriptable Object Generator

#### Das Problem:   
Ich habe ca 190 Kreaturen (etwas wie Sammelobjekte) mit Namen, ID, Bild, Bescheribung, Stats etc., was händisch unsinnig viel Arbeit zu managen ist, besonders, wenn ich zB einen Stat hinzufügen möchte. Zedem will ich die Kreaturen in thematische Enzyklopadien unterteilen, die ich zur Runtime abrufen, verändern und abspeichern können muss.

#### Mein Lösungsansatz:   
- Das **Erstellen der Assets** habe ich in einem Editorskript gelöst (siehe [CreatureScriptableObjectGenerator.cs](Assets/Scripts/CreatureObjectGenerator/CreatureScriptableObjectGenerator.cs)). Für Textbasierte Daten führe ich eine CSV Datei und die Sprites rufe ich über ID-Nummern auf.   
Um die Erstellten Objecte im Spiel möglichst effizient abrufen zu können erstelle ich ein zusätzliches ScriptableObject mit allen Referenzen, was ich über die Resourcen aufrufen kann.

- Für das **Speichern der Enzyklopädien** habe ich mich für JSON-datein entschieden um meine Datenstrukturen unabhängig vom Spiel speichern zu können. In einem weiteren Editorskript (siehe [EncyclopediaAssetGenerator.cs](Assets/Scripts/CreatureObjectGenerator/EncyclopediaAssetsGenerator.cs)) generiere ich die verschiedenen Dateien direkt zusammen mit den Kreaturen. So spare ich mir viel Ärger mit der Zuweisung in der Engine und zu vollen Szenen.

Beim Starten der zugehörigen Szene werden die beiden Skripte augeführt, anschließend lassen sich die generierten assets in den Ordnern "*Assets/Creatures*" und im Resourcefolder finden.