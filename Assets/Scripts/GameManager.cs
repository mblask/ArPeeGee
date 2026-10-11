using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public delegate void OnPlayerVictory();
    public OnPlayerVictory onPlayerVictory;

    public PlayerData PlayerData;

    [Header("Player Spawning Settings")]
    public List<GameObject> PlayerPrefabs = new List<GameObject>();
    public Transform PlayerSpawnTransform;

    [Header("Game Camera")]
    public GameObject MainCamera;

    [Header("Game Cursors")]
    public Texture2D DefaultCursor;
    public Texture2D InteractableCursor;
    public Vector2 hotspot = new Vector2(10.0f, 10.0f);

    [Header("Item Name Panel")]
    public ItemNamePanel ItemNamePanel;
    public float ItemNameOffset;

    [Header("All Game Items")]
    public List<Item> Treasure = new List<Item>();

    private GameObject _currentPlayerPrefab;
    private GameObject _playerObject;
    private FollowTarget _mainCameraFollowTarget;
    private Inventory _inventory;

    private int _enemiesKilled = 0;
    public int EnemiesKilled => _enemiesKilled;

    private float _timePassedInTheGame;
    public float TimePassedInTheGame => _timePassedInTheGame;

    private bool _equipmentSetUp;
    private bool _playerManuallyAddedToLevel = false;

    private PlayerController _playerController;
    private PlayerEquipment _playerEquipment;
    private PlayerStats _playerStats;

    #region Singleton
    private void Awake()
    {
        Instance = this;

        _enemiesKilled = 0;
        _timePassedInTheGame = 0.0f;

        GeneratePlayer();

        Time.timeScale = 1.0f;
    }
    #endregion

    private void Start()
    {
        _mainCameraFollowTarget = MainCamera.GetComponent<FollowTarget>();
        _inventory = Inventory.Instance;
        if (_playerObject != null)
            _mainCameraFollowTarget.SetTargetToFollow(_playerObject.transform);
    }

    private void OnEnable()
    {
        _playerController = PlayerController.Instance;
        _playerEquipment = PlayerEquipment.Instance;
        _playerStats = PlayerStats.Instance;
    }

    private void Update()
    {
        if (!_equipmentSetUp && PlayerData.EquippedItems != null && _playerManuallyAddedToLevel == false)
        {
            SetupPlayer();
        }

        _timePassedInTheGame += Time.deltaTime;
    }

    private void SetupPlayer()
    {
        if (_playerStats == null)
            _playerStats = PlayerStats.Instance;

        _playerStats.UpdateToLevelAndExperience(PlayerData.CurrentPlayerLevel, PlayerData.CurrentExperience);

        if (PlayerData.InventoryItems != null)
            SetupInventory();

        if (PlayerData.LearnedSpells != null)
            SetupLearnedSpellsAndAbilities();

        if (PlayerData.EquippedItems != null)
            SetupEquipment();

        SetupCurrentHealthAndMagic();

        SavePlayerData();

        _equipmentSetUp = true;
    }

    private void SetupCurrentHealthAndMagic()
    {
        if (PlayerData.CurrentHealth == 0.0f)
            _playerStats.CurrentHealth = _playerStats.TotalHealth.GetValue();
        else
            _playerStats.CurrentHealth = PlayerData.CurrentHealth;

        if (PlayerData.CurrentMagic == 0.0f)
            _playerStats.CurrentMagic = _playerStats.TotalMagic.GetValue();
        else
            _playerStats.CurrentMagic = PlayerData.CurrentMagic;

        _playerStats.UpdateHealthSlider();
        _playerStats.UpdateMagicSlider();
    }

    public void GeneratePlayer()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");

        if (_playerObject != null)
        {
            _playerManuallyAddedToLevel = true;
            return;
        }

        _playerManuallyAddedToLevel = false;

        LoadPlayerData();

        foreach (GameObject playerObject in PlayerPrefabs)
        {
            if (playerObject.gameObject.name != PlayerData.PlayerObjectName)
                continue;

            _currentPlayerPrefab = playerObject;
            break;
        }

        _playerObject = SpawnPlayer(PlayerSpawnTransform);

        _timePassedInTheGame = PlayerData.GameTime;
        _enemiesKilled = PlayerData.EnemiesKilled;
    }    

    public void SetupEquipment()
    {
        if (_playerEquipment == null)
            _playerEquipment = PlayerEquipment.Instance;

        if (_playerEquipment.GetEquipSlots() == null || _playerEquipment.GetEquipSlots().Count == 0)
            _equipmentSetUp = false;

        if (PlayerData == null)
            return;

        foreach (string itemName in PlayerData.EquippedItems)
        {
            Item item = GetItemFromTreasure(itemName);
            if (item == null)
                continue;

            Equipment equipmentItem = item as Equipment;
            if (equipmentItem == null)
            {
                Debug.LogError($"SetupEquipment, item {item.ItemName} is not Equipment item");
                continue;
            }

            _playerEquipment.EquipItem(equipmentItem);
            //foreach (Item item in Treasure)
            //{
            //    if (item.ItemName != itemName)
            //        continue;
            //
            //    Equipment equipmentItem = item as Equipment;
            //    if (equipmentItem == null)
            //    {
            //        Debug.LogError($"SetupEquipment, item {item.ItemName} is not Equipment item");
            //        continue;
            //    }
            //
            //    _playerEquipment.EquipItem(equipmentItem);
            //}
        }
    }

    public void SetupInventory()
    {
        if (_inventory == null)
            _inventory = Inventory.Instance;

        if (_inventory.GetItems().Count != 0)
            _inventory.RemoveAllItems();

        if (PlayerData != null)
        {
            foreach (string itemName in PlayerData.InventoryItems)
            {
                Item item = GetItemFromTreasure(itemName);
                if (item == null)
                    continue;

                _inventory.AddItem(item);
                //foreach (Item item in Treasure)
                //{
                //    if (item.ItemName != itemName)
                //        continue;
                //    
                //    _inventory.AddItem(item);
                //}
            }
        }

        /*if (_inventory.GetItems().Count != 0)
            _inventory.RemoveAllItems();

        if (PlayerData != null)
        {
            foreach (string itemName in PlayerData.InventoryItems)
            {
                Item item = GetItemFromTreasure(itemName);
                if (item == null)
                    continue;

                _inventory.AddItem(item);
                //foreach (Item item in Treasure)
                //{
                //    if (item.ItemName != itemName)
                //        continue;
                //    
                //    _inventory.AddItem(item);
                //}
            }
        }*/
    }

    private Item GetItemFromTreasure(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
            return null;

        foreach (Item item in Treasure)
        {
            if (item.ItemName != itemName)
                continue;

            return item;
        }

        return null;
    }

    public void SetupLearnedSpellsAndAbilities()
    {
        if (PlayerData.LearnedSpells == null)
            return;

        foreach (string learnedSpell in PlayerData.LearnedSpells)
        {
            foreach (GameObject playerSpell in _playerController.AllPlayerSpells)
            {
                if (learnedSpell != playerSpell.name)
                    continue;

                if (_playerController.LearnedSpells.Contains(playerSpell))
                    continue;

                _playerController.LearnedSpells.Add(playerSpell);
            }
        }
    }

    public GameObject SpawnPlayer(Transform spawnPosition)
    {
        GameObject player;
        if (_currentPlayerPrefab != null)
        {
            player = Instantiate(_currentPlayerPrefab, spawnPosition.position, Quaternion.identity);
            return player;
        }

        _currentPlayerPrefab = PlayerPrefabs[1];

        if (_currentPlayerPrefab == null || spawnPosition == null)
        {
            Debug.LogWarning("Either PlayerPrefab or SpawnPosition missing");
            return null;
        }

        player = Instantiate(PlayerPrefabs[0], spawnPosition.position, Quaternion.identity);
        return player;
    }

    public void SavePlayerData()
    {
        PlayerData playerSaveData = new PlayerData(_currentPlayerPrefab, PlayerSpawnTransform, PlayerStats.Instance, Inventory.Instance, PlayerEquipment.Instance, PlayerController.Instance, Instance);

        SaveSystem.SavePlayer(playerSaveData);
    }

    public void LoadPlayerData()
    {
        PlayerData = SaveSystem.LoadPlayer();
    }

    public void PlayerVictory()
    {
        if (onPlayerVictory != null)
            onPlayerVictory.Invoke();
    }

    public void SetCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Default:
                Cursor.SetCursor(DefaultCursor, hotspot, CursorMode.Auto);
                break;
            case CursorType.Interactable:
                Cursor.SetCursor(InteractableCursor, hotspot, CursorMode.Auto);
                break;
        }
    }

    public void UpdateEnemiesKilled()
    {
        _enemiesKilled++;
    }
}
