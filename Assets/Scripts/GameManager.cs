using System.Collections;
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

    //[Header("Drop Treasure Controller")]
    //public DropTreasure DropTreasureController;

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
    private Camera _camera;

    private int _enemiesKilled = 0;
    public int EnemiesKilled => _enemiesKilled;

    private float _timePassedInTheGame;
    public float TimePassedInTheGame => _timePassedInTheGame;

    private bool _equipmentSetUp;
    private bool _playerManuallyAddedToLevel = false;

    #region Singleton
    private void Awake()
    {
        Instance = this;

        _enemiesKilled = 0;
        _timePassedInTheGame = 0.0f;

        GeneratePlayer();

        Time.timeScale = 1.0f;

        _camera = Camera.main;
    }
    #endregion

    private void Start()
    {
        _mainCameraFollowTarget = MainCamera.GetComponent<FollowTarget>();
        if (_playerObject != null)
            _mainCameraFollowTarget.SetTargetToFollow(_playerObject.transform);
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
        PlayerStats.Instance.UpdateToLevelAndExperience(PlayerData.CurrentPlayerLevel, PlayerData.CurrentExperience);

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
            PlayerStats.Instance.CurrentHealth = PlayerStats.Instance.TotalHealth.GetValue();
        else
            PlayerStats.Instance.CurrentHealth = PlayerData.CurrentHealth;

        if (PlayerData.CurrentMagic == 0.0f)
            PlayerStats.Instance.CurrentMagic = PlayerStats.Instance.TotalMagic.GetValue();
        else
            PlayerStats.Instance.CurrentMagic = PlayerData.CurrentMagic;

        PlayerStats.Instance.UpdateHealthSlider();
        PlayerStats.Instance.UpdateMagicSlider();
    }

    public void GeneratePlayer()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");

        if (_playerObject == null)
        {
            _playerManuallyAddedToLevel = false;

            LoadPlayerData();

            foreach (GameObject playerObject in PlayerPrefabs)
            {
                if (playerObject.gameObject.name == PlayerData.PlayerObjectName)
                {
                    _currentPlayerPrefab = playerObject;
                    break;
                }
            }

            _playerObject = SpawnPlayer(PlayerSpawnTransform);

            _timePassedInTheGame = PlayerData.GameTime;
            _enemiesKilled = PlayerData.EnemiesKilled;
        }
        else
        {
            _playerManuallyAddedToLevel = true;
        }
    }    

    public void SetupEquipment()
    {
        if (EquipmentManager.Instance.CurrentEquipment.Length == 0)
            _equipmentSetUp = false;

        if (PlayerData != null)
        {
            foreach (string itemName in PlayerData.EquippedItems)
            {
                foreach (Item item in Treasure)
                {
                    if (item.ItemName == itemName)
                    {
                        EquipmentManager.Instance.Equip(item as Equipment);
                    }
                }
            }
        }
    }

    public void SetupInventory()
    {
        if (PlayerInventory.Instance._items.Count != 0)
        {
            PlayerInventory.Instance.RemoveAllItems();
        }

        if (PlayerData != null)
        {
            foreach (string itemName in PlayerData.InventoryItems)
            {
                //Debug.Log(itemName);
                foreach (Item item in Treasure)
                {
                    if (item.ItemName == itemName)
                    {
                        PlayerInventory.Instance.AddItem(item);
                    }
                }
            }
        }

        if (Inventory.Instance._items.Count != 0)
        {
            Inventory.Instance.RemoveAllItems();
        }

        if (PlayerData != null)
        {
            foreach (string itemName in PlayerData.InventoryItems)
            {
                foreach (Item item in Treasure)
                {
                    if (item.ItemName == itemName)
                    {
                        Inventory.Instance.AddItem(item);
                    }
                }
            }
        }
    }

    public void SetupLearnedSpellsAndAbilities()
    {
        if (PlayerData.LearnedSpells != null)
        {
            foreach (string learnedSpell in PlayerData.LearnedSpells)
            {
                foreach (GameObject playerSpell in PlayerController.Instance.AllPlayerSpells)
                {
                    if (learnedSpell == playerSpell.name)
                    {
                        if (PlayerController.Instance.LearnedSpells.Contains(playerSpell))
                            continue;

                        PlayerController.Instance.LearnedSpells.Add(playerSpell);
                    }
                }
            }
        }
    }

    public GameObject SpawnPlayer(Transform spawnPosition)
    {
        if (_currentPlayerPrefab == null)
        {
            _currentPlayerPrefab = PlayerPrefabs[1];

            if (_currentPlayerPrefab == null || spawnPosition == null)
            {
                Debug.LogWarning("Either PlayerPrefab or SpawnPosition missing");
                return null;
            }

            GameObject player = Instantiate(PlayerPrefabs[0], spawnPosition.position, Quaternion.identity);
            return player;
        }
        else
        {
            GameObject player = Instantiate(_currentPlayerPrefab, spawnPosition.position, Quaternion.identity);
            return player;
        }
    }

    public void SavePlayerData()
    {
        PlayerData playerSaveData = new PlayerData(_currentPlayerPrefab, PlayerSpawnTransform, PlayerStats.Instance, PlayerInventory.Instance, EquipmentManager.Instance, PlayerController.Instance, Instance);

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
