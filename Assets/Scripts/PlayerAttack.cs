using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private static PlayerAttack _instance;
    public static PlayerAttack Instance
    {
        get
        {
            return _instance;
        }
    }

    public delegate void OnNewSpellLearned(bool value);
    public OnNewSpellLearned onNewSpellLearned;

    public LayerMask EnemyLayerMask;
    public bool IsRanged;
    public List<GameObject> AllPlayerSpells = new List<GameObject>();

    [Header("Read-only")]
    [SerializeField] private List<GameObject> _learnedSpells = new List<GameObject>();
    public List<GameObject> LearnedSpells
    {
        get
        {
            return _learnedSpells;
        }
    }

    [SerializeField] private int _currentSpellIndex = 0;
    [SerializeField] private AudioClip _projectileCastingAudioClip;


    private float _rightClickRaycastDistance = 0.01f;
    private Camera _camera;
    private Rigidbody2D _rigidbody2d;
    private PlayerController _playerController;
    private PlayerInteraction _playerInteraction;
    private Transform _weaponSlotTransform;
    private WeaponSlot _weaponSlot;
    private Animator _weaponSlotAnimator;

    [SerializeField] private float _nextAttackTime = 0.0f;
    [SerializeField] private float _obstacleDistance = 0.4f;
    [SerializeField] private float _enemyAttackDistance = 0.7f;

    [SerializeField] private Collider2D _currentEnemy = null;

    //private GameObject _currentMeelePanel;
    private CurrentSpellPanel _currentSpellPanel;
    //private Image _currentMeelePanelIcon;
    private Image _currentSpellPanelIcon;

    private void Awake()
    {
        _instance = this;

        _camera = Camera.main;
        _rigidbody2d = GetComponent<Rigidbody2D>();
        _weaponSlotTransform = transform.Find("PlayerSprite").Find("WeaponSlot");
        _weaponSlot = _weaponSlotTransform.GetComponent<WeaponSlot>();
        _weaponSlotAnimator = _weaponSlotTransform.GetComponent<Animator>();
    }

    private void Start()
    {
        _playerController = PlayerController.Instance;
        _playerInteraction = PlayerInteraction.Instance;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (_currentEnemy != null)
            {
                if (!_currentEnemy.GetComponent<EnemyController>().IsDead)
                {
                    if (IsRanged && !_playerController.IsMoving() && PlayerStats.Instance.CurrentMagic >= _learnedSpells[_currentSpellIndex].GetComponent<Spell>().BaseSpellCost)
                    {
                        CastSpell(_currentEnemy.transform.position);
                    }
                    else
                    {
                        float distance = Vector2.Distance(transform.position, _currentEnemy.transform.position);
                        if (distance > _enemyAttackDistance)
                        {
                            _playerController.ShouldMove(true);
                            _playerController.SetDestination(_currentEnemy.transform.position);
                            //_shouldMove = true;
                            //_destination = _currentEnemy.transform.position;
                        }
                        else
                        {
                            _playerController.StopMovement();
                            //_shouldMove = false;
                            //_destination = transform.position;
                            if (Time.time >= _nextAttackTime && !_playerController.IsMoving())
                                PlayerAttacks();
                        }
                    }
                }
            }
            else
            {
                Vector3 mouseInputPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D enemyHit = Physics2D.Raycast(mouseInputPosition, Vector2.up, _rightClickRaycastDistance, EnemyLayerMask);

                if (enemyHit)
                {
                    _currentEnemy = enemyHit.collider;
                }
                else
                {
                    _playerController.StopMovement();
                    //_shouldMove = false;
                    //_destination = transform.position;
                    _playerInteraction.RemoveFocus();

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (IsRanged && PlayerStats.Instance.CurrentMagic >= _learnedSpells[_currentSpellIndex].GetComponent<Spell>().BaseSpellCost)
                            CastSpell(mouseInputPosition);
                        else
                            PlayerAttacks();
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
            _currentEnemy = null;
    }

    private void PlayerAttacks()
    {
        _weaponSlotAnimator.SetBool("IsMoving", _playerController.IsMoving());

        Vector2 mouseClickPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseClickPoint - _rigidbody2d.position).normalized;
        _playerController.FlipSprite(direction.x);

        if (!_weaponSlot.IsEquipped())
            return;

        if (Time.time >= _nextAttackTime)
        {
            _weaponSlot.WeaponSlotAttack();

            _nextAttackTime = Time.time + 1.0f / PlayerStats.Instance.TotalAttackRate.GetValue();
        }
    }
    
    public void CastSpell(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        _playerController.FlipSprite(direction.x);

        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + 1.0f / PlayerStats.Instance.TotalCastingRate.GetValue();
            _weaponSlotAnimator.SetTrigger("Attack");

            SpellType spellType = _learnedSpells[_currentSpellIndex].GetComponent<Spell>().SpellType;

            switch (spellType)
            {
                case SpellType.Projectile:
                    float directionAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    GameObject spellClone = Instantiate(_learnedSpells[_currentSpellIndex], _weaponSlot.transform.position, Quaternion.Euler(0.0f, 0.0f, directionAngle));
                    Projectile projectile = spellClone.GetComponent<Projectile>();
                    projectile.SetCasterTag(gameObject.tag);
                    PlayerStats.Instance.UpdateMagic(-projectile.BaseSpellCost);
                    AudioManager.Instance?.SFXAudioSource.PlayOneShot(_projectileCastingAudioClip);
                    break;

                case SpellType.AreaOfEffect:
                    AreaOfEffectSpell aoeSpell = _learnedSpells[_currentSpellIndex].GetComponent<AreaOfEffectSpell>();
                    aoeSpell.SetCasterTag(gameObject.tag);
                    aoeSpell.SetOriginPosition(transform);
                    aoeSpell.DealAreaDamage();
                    PlayerStats.Instance.UpdateMagic(-aoeSpell.BaseSpellCost);
                    break;
            }
        }
    }

    public bool LearnSpell(Spell spell)
    {
        if (spell == null)
        {
            Debug.LogWarning("The spell you want to learn is null");
            return false;
        }

        foreach (GameObject learnedSpell in _learnedSpells)
        {
            if (learnedSpell.GetComponent<Spell>().SpellName == spell.SpellName)
            {
                //Debug.Log("You already know that spell");
                if (onNewSpellLearned != null)
                    onNewSpellLearned.Invoke(false);
                return false;
            }
        }

        foreach (GameObject spellObject in AllPlayerSpells)
        {
            if (spellObject.GetComponent<Spell>().SpellName == spell.SpellName)
            {
                _learnedSpells.Add(spellObject);
                if (onNewSpellLearned != null)
                    onNewSpellLearned.Invoke(true);

                return true;
            }
        }

        return false;
    }

    public void ChangeActiveSpell()
    {
        if (AllPlayerSpells.Count == 0 || _learnedSpells.Count == 0)
            return;

        _currentSpellIndex++;

        if (_currentSpellIndex >= _learnedSpells.Count)
            _currentSpellIndex = 0;

        _currentSpellPanel.SetCurrentSpell(_learnedSpells[_currentSpellIndex].GetComponent<Spell>());
        _currentSpellPanelIcon.color = Color.white;
        _currentSpellPanelIcon.sprite = _learnedSpells[_currentSpellIndex].GetComponent<Spell>().SpellIcon;
    }
}
