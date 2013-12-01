using UnityEditor;
using UnityEngine;
using System.Collections;

public class Sound2DImporter : AssetPostprocessor
{
    private void OnPreprocessAudio()
    {
        AudioImporter ai = assetImporter as AudioImporter;

        ai.threeD = false;
    }
}
