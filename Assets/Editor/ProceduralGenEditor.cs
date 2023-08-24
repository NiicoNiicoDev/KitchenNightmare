using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(SCR_ProceduralGeneration))]
public class ProceduralGenEditor : Editor
{
    private Procedural_Gen_Settings scriptableProceduralSettings;
    SCR_ProceduralGeneration gen;

    public override void OnInspectorGUI()
    {
        gen = (SCR_ProceduralGeneration)target;
        scriptableProceduralSettings = gen.scriptableObject;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            gen.ResetGeneration();
        }

        EditorGUILayout.LabelField("\nSeed Settings\n", EditorStyles.boldLabel);
        gen.seed = EditorGUILayout.IntField("Seed", scriptableProceduralSettings.seed);
        gen.randomSeed = EditorGUILayout.Toggle("Random Seed", scriptableProceduralSettings.useRandomSeed);
        gen.useTestedSeeds = EditorGUILayout.Toggle("Use Tested Seeds", scriptableProceduralSettings.useTestedSeeds);
        gen.numberOfNewSeeds = EditorGUILayout.IntSlider("Number of Seeds", gen.numberOfNewSeeds, 1, 100000);

        if (GUILayout.Button("Re-Generate Tested Seeds"))
        {
            gen.randomSeed = false;
            gen.useTestedSeeds = false;
            gen.killOnFail = false;

            string newSeeds = null;
            int successes = 0;
            for (int i = 1; i <= gen.numberOfNewSeeds; i++)
            {
                gen.StartTestingSeeds(i);
                if (gen.sucessfulGeneration)
                {
                    if (newSeeds == null)
                    {
                        newSeeds += i.ToString();
                    }
                    else
                    {
                        newSeeds += "\n" + i.ToString();
                    }
                    successes++;
                    Debug.Log(successes);
                }
                gen.ResetFields();
            }
            gen.numberOfNewSeeds = successes;

            string path = "Assets/Resources/Seeds.txt";
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(newSeeds);
                    writer.Close();
                }
            }
            catch(System.UnauthorizedAccessException)
            {
                throw new System.ArgumentException("Make sure you have the seeds text file checked out!");
            }

            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(path);
            gen.testedSeedAsset = Resources.Load<TextAsset>("Seeds");
        }

        if (gen.useTestedSeeds)
        {
            gen.randomSeed = false;
        }
        if (gen.seed <= 0)
        {
            gen.seed = 1;
        }
    }
    public void OnInspectorUpdate()
    {
        if (Selection.activeTransform)
        {
            Selection.activeTransform.localScale = new Vector3(gen.numberOfNewSeeds, gen.numberOfNewSeeds, gen.numberOfNewSeeds);
        }
    }
}
