using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public delegate void OnNewSpellLearned(bool value);
    public OnNewSpellLearned onNewSpellLearned;

    public bool IsRanged;
    public List<GameObject> AllPlayerSpells = new List<GameObject>();
    
    [Header("Read-only")]
    [SerializeField] private List<GameObject> _learnedSpells = new List<GameObject>();
    public List<GameObject> LearnedSpells => _learnedSpells;

    public int CurrentSpellIndex = 0;
    public AudioClip ProjectileCastingAudioClip;

    [Header("General Settings")]
    public float DestinationOffset = 0.05f;
    public Color BloodSplashColor;
    public ParticleSystem BloodSplashPrefab;
    public LayerMask EnemyLayerMask;
    public LayerMask Obstacles;

    [Header("Read-only")]
    [SerializeField] private bool _shouldMove = false;
    [SerializeField] private float _nextAttackTime = 0.0f;
    [SerializeField] private float _obstacleDistance = 0.4f;
    [SerializeField] private float _enemyAttackDistance = 0.7f;
    [SerializeField] private Interactable _focus = null;

    [SerializeField] private Collider2D _currentEnemy = null;
    [SerializeField] private Interactable _currentInteractable = null;

    //private GameObject _currentMeelePanel;
    private CurrentSpellPanel _currentSpellPanel;
    //private Image _currentMeelePanelIcon;
    private Image _currentSpellPanelIcon;
    //private const string _currentMeeleIconName = "CurrentMeeleIcon";
    private const string _currentSpellIconName = "CurrentSpellIcon";

    private float _rightClickRaycastDistance = 0.01f;

    private Vector2 _destination = new Vector2();

    private Rigidbody2D _rigidbody2d;
    private Animator _playerAnimator;

    [Header("Read-only: Weapon Slot")]
    [SerializeField] private WeaponSlot _weaponSlot;
    private Animator _weaponSlotAnimator;

    private void Awake()
    {
        Instance = this;

        _rigidbody2d = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponentInChildren<Animator>();

        _weaponSlotAnimator = _weaponSlot.GetComponent<Animator>();
    }

    private void Start()
    {
        if (IsRanged)
        {
            //Not used currently
            //_currentMeelePanelIcon = GameObject.Find(_currentMeelePanelName).GetComponentInChildren<Image>();
            _currentSpellPanelIcon = GameObject.Find(_currentSpellIconName).GetComponentInChildren<Image>();
            _currentSpellPanel = _currentSpellPanelIcon.GetComponentInParent<CurrentSpellPanel>();

            _currentSpellPanel.SetCurrentSpell(_learnedSpells[0].GetComponent<Spell>());
            _currentSpellPanelIcon.color = Color.white;
            _currentSpellPanelIcon.sprite = _learnedSpells[CurrentSpellIndex].GetComponent<Spell>().SpellIcon;
        }
    }

    private void Update()
    {
        if (PlayerStats.Instance.IsDead)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.W))
            ChangeActiveSpell();

        if (Input.GetKey(KeyCode.Mouse0))
        {
            _shouldMove = true;

            Vector3 mouseInputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseInputPosition, Vector2.up, _rightClickRaycastDistance);

            if (hit)
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null && _currentInteractable != interactable)
                {
                    if (!interactable.transform.IsChildOf(_weaponSlot.transform))
                    {
                        SetFocus(interactable);

                        float distance = Vector2.Distance(transform.position, hit.collider.transform.position);
                        if (distance >= interactable.InteractionRadius)
                        {
                            _destination = hit.collider.transform.position;

                            _shouldMove = true;
                            _currentInteractable = null;
                        }
                        else
                        {
                            _shouldMove = false;
                            if (!interactable.HasInteracted)
                            {
                                interactable.Interact();
                                interactable.HasInteracted = true;
                                _currentInteractable = interactable;
                                RemoveFocus();
                            }
                        }
                    }                    
                }
            }
            else
            {
                _destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (_focus != null)
                    RemoveFocus();

                _currentInteractable = null;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
            _currentInteractable = null;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (_currentEnemy != null)
            {
                if (!_currentEnemy.gameObject.GetComponent<EnemyController>().IsDead)
                {
                    if (IsRanged && !_shouldMove && PlayerStats.Instance.CurrentMagic >= _learnedSpells[CurrentSpellIndex].GetComponent<Spell>().BaseSpellCost)
                    {
                        CastSpell(_currentEnemy.transform.position);
                    }
                    else
                    {
                        float distance = Vector2.Distance(transform.position, _currentEnemy.transform.position);
                        if (distance > _enemyAttackDistance)
                        {
                            _destination = _currentEnemy.transform.position;
                            _shouldMove = true;
                        }
                        else
                        {
                            _shouldMove = false;
                            _destination = transform.position;
                            if (Time.time >= _nextAttackTime && !_shouldMove)
                                PlayerAttacks();
                        }
                    }
                }
            }
            else
            {
                Vector3 mouseInputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D enemyHit = Physics2D.Raycast(mouseInputPosition, Vector2.up, _rightClickRaycastDistance, EnemyLayerMask);

                if (enemyHit)
                {
                    _currentEnemy = enemyHit.collider;
                }
                else
                {
                    _shouldMove = false;
                    _destination = transform.position;
                    RemoveFocus();

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (IsRanged && PlayerStats.Instance.CurrentMagic >= _learnedSpells[CurrentSpellIndex].GetComponent<Spell>().BaseSpellCost)
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

    private void FixedUpdate()
    {
        if (_shouldMove)
        {
            MoveToDestination(_destination);
        }

        _playerAnimator.SetBool("IsMoving", _shouldMove);
        _weaponSlotAnimator.SetBool("IsMoving", _shouldMove);
    }

    #region Interactable Focus
    public void SetFocus(Interactable newFocus)
    {
        if (newFocus != _focus)
        {
            if (_focus != null)
                _focus.OnDefocus();

            _focus = newFocus;
            _focus.OnFocus(transform);
        }
    }

    public void RemoveFocus()
    {
        if (_focus != null)
            _focus.OnDefocus();

        _focus = null;
    }
    #endregion

    #region Sprite Movement and Flipping
    private void MoveToDestination(Vector2 destination)
    {
        Vector2 direction = (destination - _rigidbody2d.position).normalized;
        FlipSprite(direction.x);

        float distance = Vector2.Distance(destination, _rigidbody2d.position);

        if (distance >= DestinationOffset)
        {
            _rigidbody2d.MovePosition(_rigidbody2d.position + direction * PlayerStats.Instance.TotalMovementSpeed.GetValue() * Time.fixedDeltaTime);
        }
        else
        {
            _shouldMove = false;
        }

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, _obstacleDistance, Obstacles);
        if (hitInfo.collider != null)
        {
            _shouldMove = false;
        }
    }

    private void FlipSprite(float direction)
    {
        Vector3 localScale = transform.localScale;
        if (direction > 0.0f)
        {
            localScale.x = 1;
            transform.localScale = localScale;
        }
        else if (direction < 0.0f)
        {
            localScale.x = -1;
            transform.localScale = localScale;
        }
    }
    #endregion

    #region PlayerAttack
    
    private void PlayerAttacks()
    {
        _weaponSlotAnimator.SetBool("IsMoving", _shouldMove);

        Vector2 mouseClickPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseClickPoint - _rigidbody2d.position).normalized;
        FlipSprite(direction.x);

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
        FlipSprite(direction.x);

        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + 1.0f / PlayerStats.Instance.TotalCastingRate.GetValue();
            _weaponSlotAnimator.SetTrigger("Attack");

            SpellType spellType = _learnedSpells[CurrentSpellIndex].GetComponent<Spell>().SpellType;

            switch (spellType)
            {
                case SpellType.Projectile:
                    float directionAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    GameObject spellClone = Instantiate(_learnedSpells[CurrentSpellIndex], _weaponSlot.transform.position, Quaternion.Euler(0.0f, 0.0f, directionAngle));
                    Projectile projectile = spellClone.GetComponent<Projectile>();
                    projectile.SetCasterTag(gameObject.tag);
                    PlayerStats.Instance.UpdateMagic(-projectile.BaseSpellCost);
                    AudioManager.Instance?.SFXAudioSource.PlayOneShot(ProjectileCastingAudioClip);
                    break;

                case SpellType.AreaOfEffect:
                    AreaOfEffectSpell aoeSpell = _learnedSpells[CurrentSpellIndex].GetComponent<AreaOfEffectSpell>();
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

        CurrentSpellIndex++;

        if (CurrentSpellIndex >= _learnedSpells.Count)
            CurrentSpellIndex = 0;

        _currentSpellPanel.SetCurrentSpell(_learnedSpells[CurrentSpellIndex].GetComponent<Spell>());
        _currentSpellPanelIcon.color = Color.white;
        _currentSpellPanelIcon.sprite = _learnedSpells[CurrentSpellIndex].GetComponent<Spell>().SpellIcon;
    }

    #endregion

    public void BloodSplash()
    {
        Vector2 bloodSplashposition = new Vector2(transform.position.x, transform.position.y + 0.2f);
        ParticleSystem bloodSplashClone = Instantiate(BloodSplashPrefab, bloodSplashposition, Quaternion.identity);

        ParticleSystem.MainModule settings = bloodSplashClone.main;
        settings.startColor = BloodSplashColor;

        bloodSplashClone.Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _destination);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _enemyAttackDistance);
    }
}
