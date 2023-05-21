using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BackupManager
{
    public static void BackupSOString(ScriptableObject scriptableObject, string json)
    {
        string filePath = AssetDatabase.GetAssetPath(scriptableObject);
        string fileName = scriptableObject.name;
        string folderPath = System.IO.Path.GetDirectoryName(filePath);
        string backupFolderPath = Path.Combine(folderPath, fileName + "_BackupsFolder");

        if (Directory.Exists(backupFolderPath))
        {
            string currentTime = DateTime.Now.ToString("dd.MM.yyyy - hh.mm.ss tt");
            string backupFilePath = Path.Combine(backupFolderPath, fileName + "_" + currentTime + ".jsonStringBackup");
            string[] backupFiles = Directory.GetFiles(backupFolderPath, "*.jsonStringBackup");
            if (backupFiles.Length > 0)
            {
                Array.Sort(backupFiles, (a, b) => File.GetCreationTime(b).CompareTo(File.GetCreationTime(a)));
                string lastSavedFileLocation = backupFiles[0];
                string lastSavedJson = System.IO.File.ReadAllText(lastSavedFileLocation);
                if (lastSavedJson == json)
                {
                    Debug.LogWarning("No new Changes in Files will not create dupplicate File");
                    return;
                }
                // Use the lastSavedJson as needed
            }
            else
            {
                // No backup files found in the directory
            }
            System.IO.File.WriteAllText(backupFilePath, json);
            Debug.Log("File Backup Created" + backupFilePath + " \n" + currentTime);
        }
        else
        {
            Debug.LogError("Backup Folder Does not Exist" + backupFolderPath);
        }
    }
    public static string ReloadLatestBackup(ScriptableObject scriptableObject)
    {
        string filePath = AssetDatabase.GetAssetPath(scriptableObject);
        string fileName = scriptableObject.name;
        string folderPath = System.IO.Path.GetDirectoryName(filePath);
        string backupFolderPath = Path.Combine(folderPath, fileName + "_BackupsFolder");
        string[] backupFiles = Directory.GetFiles(backupFolderPath, "*.jsonStringBackup");
        if (backupFiles.Length > 0)
        {
            Array.Sort(backupFiles, (a, b) => File.GetCreationTime(b).CompareTo(File.GetCreationTime(a)));
            string lastSavedFileLocation = backupFiles[0];
            string lastSavedJson = System.IO.File.ReadAllText(lastSavedFileLocation);
            Debug.Log("Reloaded File" + backupFiles[0]);
            return lastSavedJson;
        }
        Debug.Log("failed to Reload File");
        return "";
    }
}
