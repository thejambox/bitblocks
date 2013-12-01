using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseProjectSync : ScriptableObject
{
    public static string baseProjectPath
    {
        get
        {
            return Path.GetFullPath(string.Format("{0}/../../../base/Unity/", Application.dataPath));
        }
    }

    public static string projectPath
    {
        get
        {
            return Path.GetFullPath(string.Format("{0}/../", Application.dataPath));
        }
    }

    [MenuItem("Assets/BaseProject/Copy to Base", false, 2000)]
    public static void CopyToBase()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogError("[BASEPROJECT]: ERROR: No files or folders were selected.");
            return;
        }

        List<string> paths = GetPathsFromSelection();

        foreach (string localPath in paths)
        {
            string sourcePath = Path.Combine(projectPath, localPath);
            string destPath = Path.Combine(baseProjectPath, localPath);

            BaseProjectCopy(sourcePath, destPath);
        }

        Debug.Log("[BASEPROJECT]: Copy to Base Complete.");
    }

    [MenuItem("Assets/BaseProject/Update from Base", false, 2001)]
    public static void UpdateFromBase()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogError("[BASEPROJECT]: ERROR: No files or folders were selected.");
            return;
        }

        List<string> paths = GetPathsFromSelection();

        foreach (string localPath in paths)
        {
            string sourcePath = Path.Combine(baseProjectPath, localPath);
            string destPath = Path.Combine(projectPath, localPath);

            BaseProjectCopy(sourcePath, destPath);
        }

        Debug.Log("[BASEPROJECT]: Update from Base Complete.");
    }

    private static void BaseProjectCopy(string sourcePath, string destPath)
    {
        bool sourceIsDir = Directory.Exists(sourcePath);
        bool sourceIsFile = File.Exists(sourcePath);

        bool destIsDir = Directory.Exists(destPath);

        if (sourceIsDir)
        {
            if (!destIsDir)
            {
                FileTools.CopyAll(sourcePath, destPath);
            }
            else
            {
                DirectoryInfo destDir = new DirectoryInfo(destPath);

                int option = OverwriteFolderDialog(destPath);

                if (option == 0)
                {
                    FileTools.MakeFilesWritable(destDir);
                    FileTools.CopyAll(sourcePath, destPath);
                }
                else if (option == 1)
                {
                    FileTools.MakeFilesWritable(destDir);
                    destDir.Delete(true);
                    FileTools.CopyAll(sourcePath, destPath);
                }
                else if (option == 2)
                {
                    Debug.LogWarning("[BASEPROJECT]: Skipped " + sourcePath);
                    return;
                }
            }
        }
        else if (sourceIsFile)
        {
            if (IsSafeFileCopy(sourcePath, destPath) || OverwriteFileDialog(destPath))
            {
                FileTools.CopyTo(sourcePath, destPath);
            }
            else
            {
                Debug.LogWarning("[BASEPROJECT]: Skipped " + sourcePath);
                return;
            }
        }
        else
        {
            Debug.LogError("[BASEPROJECT]: ERROR: Missing " + sourcePath);
            return;
        }
    }

    private static List<string> GetPathsFromSelection()
    {
        List<string> paths = new List<string>();

        foreach (Object obj in Selection.objects)
            paths.Add(AssetDatabase.GetAssetPath(obj));

        // sort folders by length, longest first.
        paths.Sort((a, b) => (a.CompareTo(b) * -1));

        // check for children folders
        List<string> childrenPaths = new List<string>();
        foreach (string path in paths)
        {
            if (!Directory.Exists(path))
                continue;

            foreach (string otherPath in paths)
            {
                if (!Directory.Exists(otherPath))
                {
                    continue;
                }
                if (path == otherPath)
                {
                    continue;
                }
                else if (otherPath.StartsWith(path))
                {
                    childrenPaths.Add(otherPath);
                    continue;
                }
            }
        }

        // remove children paths from the paths
        foreach (string childPath in childrenPaths)
        {
            SubFolderIgnoredDialog(childPath);
            paths.Remove(childPath);
        }

        return paths;
    }

    // we determine that the file is safe to copy if the source file is older than the dest file,
    // or if the destination file doesn't exist.
    private static bool IsSafeFileCopy(string source, string dest)
    {
        FileInfo sourceFile = new FileInfo(source);
        FileInfo destFile = new FileInfo(dest);

        if (!destFile.Exists)
            return true;
        else if (sourceFile.LastWriteTime >= destFile.LastWriteTime)
            return true;

        return false;
    }

    private static bool OverwriteFileDialog(string path)
    {
#if UNITY_EDITOR
        string filename = Path.GetFileName(path);
        string title = string.Format("Overwrite {0}?", filename);
        string message = "The destination file is newer than the source file. Are you sure?";
        string ok = "Confirm";
        string cancel = "Cancel";

        return EditorUtility.DisplayDialog(title, message, ok, cancel);
#else 
        return false;
#endif
    }

    private static int OverwriteFolderDialog(string path)
    {
#if UNITY_EDITOR
        string localPath = string.Empty;

        string destType = string.Empty;

        if (path.StartsWith(projectPath))
        {
            destType = "Project";
            localPath = path.Substring(projectPath.Length);
        }
        else
        {
            destType = "Base";
            localPath = path.Substring(baseProjectPath.Length);
        }

        string title = string.Format("Overwrite {0} {1}?", destType, localPath);
        string message = "This destination folder exists. Please choose an action.";
        string ok = "Append and Overwrite";
        string cancel = "Replace";
        string alt = "Cancel";

        return EditorUtility.DisplayDialogComplex(title, message, ok, cancel, alt);
#else
        return 2;
#endif
    }

    private static bool SubFolderIgnoredDialog(string localPath)
    {
#if UNITY_EDITOR
        string title = string.Format("Ignoring {0}", localPath);
        string message = "When copying multiple folders, subfolders are not allowed.";
        string ok = "OK";

        return EditorUtility.DisplayDialog(title, message, ok);
#else 
        return false;
#endif
    }
}