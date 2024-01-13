using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// GameManager class with simple save data
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Level shop UI")]
    public GameObject levelShop;
    [Header("Game save data")]
    public SaveData saveData;
    [HideInInspector]
    public ButtonInteractionData buttonData; 
    [HideInInspector]
    public int stackedEnemiesCount = 0; 

    #region Unity methods

    private void Start()
    {
        LoadData(); 
    }

    #endregion


    #region Level up shop UI

    /// <summary>
    /// Set active level shop ui.
    /// </summary>
    public void OpenLevelUpShopUI()
    {
        levelShop.SetActive(true);
    }

    #endregion

    #region Save data

    /// <summary>
    /// Add coins to player.
    /// </summary>
    /// <param name="coins"> Coins quantity. </param>
    public void AddCoins(int coins)
    {
        saveData.coins += coins;
    }

    /// <summary>
    /// Remove coins from player.
    /// </summary>
    /// <param name="coins"> Quantity to remove. </param>
    public void RemoveCoins(int coins)
    {
        saveData.coins -= coins;
    }

    /// <summary>
    /// Add level up to player.
    /// </summary>
    public void AddLevel()
    {
        saveData.playerLevel++;
    }

    /// <summary>
    /// Save method to convert and store game data in a JSON file.
    /// </summary>
    public void Save()
    {
        // Convert the SaveData object to a JSON string.
        string jsonData = JsonUtility.ToJson(saveData);

        // Define the path for the save file.
        string filePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        // Write the data to the file.
        File.WriteAllText(filePath, jsonData);
    }

    /// <summary>
    /// LoadData method to retrieve saved game data from a JSON file.
    /// </summary>
    public void LoadData()
    {
        // Define the path for the save file.
        string filePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        // Check if the file exists before attempting to load.
        if (File.Exists(filePath))
        {
            // Read data from the file.
            string jsonData = File.ReadAllText(filePath);

            // Convert the JSON string back to the SaveData class.
            saveData = JsonUtility.FromJson<SaveData>(jsonData);
        }
        else
        {
            // If the file does not exist, create a default SaveData instance and save it.
            saveData = new SaveData();
            Save();
        }
    }

    #endregion
}


/// <summary>
/// Game save data.
/// </summary>
[System.Serializable]
public class SaveData
{
    public int coins; 
    public int playerLevel; 
    public List<int> unlockedColors; 

    
    public SaveData()
    {
        coins = 0;
        playerLevel = 1;
        unlockedColors = new List<int>();
    }
}
