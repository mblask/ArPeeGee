using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public string PlayerObjectName;
    public int CurrentLevel;
    public float CurrentHealth = 0.0f;
    public float CurrentMagic = 0.0f;
    public int CurrentPlayerLevel;
    public float CurrentExperience;
    public float[] CurrentLevelSpawnPoint;
    public string[] InventoryItems;
    public string[] EquippedItems;
    public string[] LearnedSpells;
    public float GameTime;
    public int EnemiesKilled;


    public PlayerData(GameObject playerPrefab, Transform playerSpawnPoint, PlayerStats playerStats, Inventory inventory, EquipmentManager equipment, PlayerController controller, GameManager gameManager)
    {
        PlayerObjectName = playerPrefab.gameObject.name;

        if (playerSpawnPoint != null)
        {
            CurrentLevelSpawnPoint = new float[3];
            CurrentLevelSpawnPoint[0] = playerSpawnPoint.transform.position.x;
            CurrentLevelSpawnPoint[1] = playerSpawnPoint.transform.position.y;
            CurrentLevelSpawnPoint[2] = playerSpawnPoint.transform.position.z;
        }
        
        if (playerStats != null)
        {
            CurrentLevel = SceneManager.GetActiveScene().buildIndex;

            CurrentPlayerLevel = playerStats.CurrentLevel;
            CurrentExperience = playerStats.CurrentExperience;
            CurrentHealth = playerStats.CurrentHealth;
            CurrentMagic = playerStats.CurrentMagic;
        }
        else
        {
            CurrentPlayerLevel = 1;
            CurrentExperience = 0.0f;
            CurrentHealth = 0.0f;
            CurrentMagic = 0.0f;
        }

        if (inventory != null)
        {
            int itemCount = inventory._items.Count;
            InventoryItems = new string[itemCount];

            for (int i = 0; i < itemCount; i++)
            {
                InventoryItems[i] = inventory._items[i].ItemName;
            }
        }

        if (equipment != null)
        {
            int equippedItemsCount = equipment.CurrentEquipment.Length;
            EquippedItems = new string[equippedItemsCount];

            for (int i = 0; i < equippedItemsCount; i++)
            {
                if (equipment.CurrentEquipment[i] == null)
                    EquippedItems[i] = "";
                else
                    EquippedItems[i] = equipment.CurrentEquipment[i].ItemName;
            }
        }

        if (controller != null)
        {
            int learnedSpellsCount = controller.LearnedSpells.Count;
            LearnedSpells = new string[learnedSpellsCount];

            for (int i = 0; i < learnedSpellsCount; i++)
            {
                if (controller.LearnedSpells[i] == null)
                    LearnedSpells[i] = "";
                else
                    LearnedSpells[i] = controller.LearnedSpells[i].name;
            }
        }

        if (gameManager != null)
        {
            GameTime = gameManager.TimePassedInTheGame;
            EnemiesKilled = gameManager.EnemiesKilled;
        }
    }
}
