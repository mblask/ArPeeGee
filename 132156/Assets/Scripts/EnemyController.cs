using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Item DropSpecificItem;

    [Header("Enemy Data Scriptable Object")]
    public EnemyData EnemyData;
    public float MovementSpeed;

    public float RemoveBodyAfterTime = 3.0f;

    [Header("Health Panel Reference")]
    [SerializeField] private EnemyHealthPanel _enemyHealthPanel;
    public EnemyHealthPanel EnemyHealthPanel => _enemyHealthPanel;

    [Header("Read-only")]
    [SerializeField] private float _stopwatch = 0.0f;
    [SerializeField] private bool _shouldMove = false;
    [SerializeField] private bool _hasCharged = false;
    [SerializeField] private float _nextAttackTime = 0.0f;
    [SerializeField] private bool _isDead = false;
    [SerializeField] public const string BackgroundLayerString = "Background";
    public bool IsDead
    {
        get
        {
            return _isDead;
        }

        set
        {
            _isDead = value;
        }
    }

    private Animator _animator;
    public Animator Animator => _animator;
    private Rigidbody2D _rigidbody2d;
    public Rigidbody2D Rigidbody2d => _rigidbody2d;

    private EnemyStats _enemyStats;

    [Header("Ranged / Meele Random Action")]
    [SerializeField] private int _randomAction = 0;
    [SerializeField] private float _minTimer;
    [SerializeField] private float _maxTimer;
    private float _rangedMeeleTimer = 0.0f;
    private bool _bossHasSeenPlayerTheFirstTime = true;

    [Header("Boss Check Surroundings For Victory")]
    [SerializeField] private float _surroundingsRadius = 5.0f;
    private bool _checkIfSurroundingsEmpty = false;
    public bool CheckIfSurroundingsEmpty
    {
        get
        {
            return _checkIfSurroundingsEmpty;
        }

        set
        {
            _checkIfSurroundingsEmpty = value;
        }
    }

    private int _currentSpellIndex = 0;
    private bool _isTemporaryModified = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2d = GetComponent<Rigidbody2D>();
        _enemyStats = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        _shouldMove = false;
        _stopwatch += Time.deltaTime;

        if (CheckIfSurroundingsEmpty)
            CheckIfSurroundingsAreEmpty();

        if (_isDead)
            return;

        if (EnemyData.IsRangedAndMeele)
        {
            Collider2D[] followColliders = Physics2D.OverlapCircleAll(transform.position, EnemyData.FollowRadius);

            if (followColliders.Length != 0)
            {
                foreach (Collider2D collider in followColliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        if (EnemyData.IsBoss && _bossHasSeenPlayerTheFirstTime && EnemyData.BossSeesPlayerAudioClip != null)
                        {
                            AudioManager.Instance.SFXAudioSource.PlayOneShot(EnemyData.BossSeesPlayerAudioClip);
                            _bossHasSeenPlayerTheFirstTime = false;
                        }

                        //random generated Action, 0 -> follow and physical attack, 1 -> cast spell
                        int action = RandomActionGenerator(2);

                        if (action == 0)
                        {
                            if (!FollowTarget(collider.transform))
                            {
                                PhysicalAttack(collider.gameObject);
                            }
                        }
                        else
                        {
                            CastSpell(collider.gameObject);
                        }
                    }
                }
            }

            _animator.SetBool("IsMoving", _shouldMove);
        }
        else if (EnemyData.IsRanged)
        {
            //Attack sequence
            Collider2D[] attackColliders = Physics2D.OverlapCircleAll(transform.position, EnemyData.FollowRadius);

            if (attackColliders.Length != 0)
            {
                foreach (Collider2D collider in attackColliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        if (!FollowTarget(collider.transform))
                        {
                            AttackTarget(collider.gameObject);
                        }
                    }
                }
            }

            _animator.SetBool("IsMoving", _shouldMove); //move animation if _shouldMove is true
        }
        else
        {
            Collider2D[] followColliders = Physics2D.OverlapCircleAll(transform.position, EnemyData.FollowRadius);

            if (followColliders.Length != 0)
            {
                foreach (Collider2D collider in followColliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        if (!FollowTarget(collider.transform))
                        {
                            AttackTarget(collider.gameObject);
                        }
                    }
                }
            }

            _animator.SetBool("IsMoving", _shouldMove);
        }
    }

    private int RandomActionGenerator(int numOfPossibleActions)
    {
        _rangedMeeleTimer -= Time.deltaTime;

        if (_rangedMeeleTimer <= 0.0f)
        {
            _randomAction = Random.Range(0, numOfPossibleActions);
            _rangedMeeleTimer = Random.Range(_minTimer, _maxTimer);
        }

        return _randomAction;
    }

    public void AttackTarget(GameObject target)
    {
        if (_isDead)
            return;

        if (EnemyData.IsRanged)
        {
            CastSpell(target);
        }
        else
        {
            PhysicalAttack(target);
        }
    }

    public void PhysicalAttack(GameObject targetObject)
    {
        Vector3 direction = (targetObject.transform.position - transform.position).normalized;
        FlipSprite(direction.x > 0.0f);

        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + 1.0f / _enemyStats.TotalEnemyAttackRate.GetValue();
            _animator.SetTrigger("Attack");

            if (IsHitting(targetObject.GetComponent<PlayerStats>().TotalArmor.GetValue()))
                targetObject.GetComponent<PlayerStats>().DamageManager(_enemyStats.TotalEnemyDamage.GetValue());
        }
    }

    public void CastSpell(GameObject targetObject)
    {
        Vector3 direction = (targetObject.transform.position - transform.position).normalized;
        FlipSprite(direction.x > 0.0f);

        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + 1.0f / _enemyStats.TotalEnemyCastingRate.GetValue();
            _animator.SetTrigger("Attack");

            if (EnemyData.EnemySpells.Count > 1)
            {
                //33% chance to switch the spell which is currently being used
                bool shouldRandomizeSpell = (Random.Range(0, 3) > 1);

                if (shouldRandomizeSpell)
                    _currentSpellIndex = Random.Range(0, EnemyData.EnemySpells.Count);
            }

            SpellType spellType = EnemyData.EnemySpells[_currentSpellIndex].GetComponent<Spell>().SpellType;

            switch (spellType)
            {
                case SpellType.Projectile:
                    float directionAngle = Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x);
                    GameObject spellClone = Instantiate(EnemyData.EnemySpells[_currentSpellIndex], transform.position, Quaternion.Euler(0.0f, 0.0f, directionAngle));
                    Projectile projectile = spellClone.GetComponent<Projectile>();
                    projectile.SetCasterTag(gameObject.tag);
                    break;

                case SpellType.AreaOfEffect:
                    AreaOfEffectSpell aoeSpell = EnemyData.EnemySpells[_currentSpellIndex].GetComponent<AreaOfEffectSpell>();
                    aoeSpell.SetCasterTag(gameObject.tag);
                    aoeSpell.SetOriginPosition(transform);
                    aoeSpell.DealAreaDamage();
                    break;
            }
        }
    }

    public bool IsHitting(float armor)
    {
        float attackArmorRatio = -_enemyStats.TotalEnemyDamage.GetValue() / armor;
        float hitChance = 1.0f - 0.6f * Mathf.Pow(10, attackArmorRatio);

        float missChance = Random.Range(0, 100) / 100.0f;

        if (hitChance > missChance)
            return true;
        else
            return false;
    }

    #region SpriteMovement
    public bool FollowTarget(Transform target)
    {
        if (_isDead)
            return false;

        float distance = Vector2.Distance(transform.position, target.position);

        Vector3 direction = (target.position - transform.position).normalized;
        FlipSprite(direction.x > 0.0f);

        float boost = Charge(EnemyData.CanCharge && distance <= EnemyData.ChargeRadius && _stopwatch >= EnemyData.ChargeCooldown);

        if (distance >= EnemyData.AttackRadius)
        {
            _shouldMove = true;
            transform.position = Vector2.MoveTowards(transform.position, target.position, _enemyStats.TotalEnemyMovementSpeed.GetValue() * boost * Time.deltaTime);
            return true;
        }
        else
        {
            _shouldMove = false;
            _stopwatch = 0.0f;
            return false;
        }
    }

    private float Charge(bool value = false)
    {

        if (value)
        {
            if (!_hasCharged)
            {
                AudioManager.Instance.SFXAudioSource.PlayOneShot(EnemyData.ChargeAudioClip);
                _hasCharged = true;
            }
            return EnemyData.ChargeSpeedBoost;
        }
        else
        {
            _hasCharged = false;
            return 1.0f;
        }
    }

    public void AddTemporaryModifier(float stat, float magnitude, float length)
    {
        if (!_isTemporaryModified)
        {
            _isTemporaryModified = true;
            StartCoroutine(TemporaryModifier(stat, magnitude, length));
        }
    }

    private IEnumerator TemporaryModifier(float stat, float magnitude, float length)
    {
        stat += magnitude;

        yield return new WaitForSeconds(length);

        stat -= magnitude;
        _isTemporaryModified = false;
    }

    private IEnumerator FadeSprite(SpriteRenderer spriteRenderer, float duration, bool inOrOut)
    {
        float startAlpha = inOrOut ? 0.0f : 1.0f;
        float endAlpha = inOrOut ? 1.0f : 0.0f;

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, startAlpha);

        float stopwatch = 0.0f;

        while (stopwatch < duration)
        {
            stopwatch += Time.deltaTime;

            float targetAlpha = Mathf.Lerp(startAlpha, endAlpha, stopwatch / duration);

            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, targetAlpha);

            yield return null;
        }

        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, endAlpha);
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

    private void FlipSprite(bool isFacingRight)
    {
        GetComponent<SpriteRenderer>().flipX = !isFacingRight;
    }

    #endregion

    public void CheckIfSurroundingsAreEmpty()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _surroundingsRadius, LayerMask.NameToLayer("Enemy"));
        if (colliders.Length != 0)
            return;
        else
        {
            GameManager.Instance.PlayerVictory();
            _checkIfSurroundingsEmpty = false;
        }
    }

    public void BloodSplash()
    {
        Vector2 bloodSplashposition = new Vector2(transform.position.x, transform.position.y + 0.2f);
        ParticleSystem bloodSplashClone = Instantiate(EnemyData.BloodSplashPrefab, bloodSplashposition, Quaternion.identity);

        ParticleSystem.MainModule settings = bloodSplashClone.main;
        settings.startColor = EnemyData.BloodSplashColor;

        bloodSplashClone.Play();
    }

    #region EnemySpriteMethods
    public void RemoveEnemyBody(float afterTime)
    {
        StartCoroutine(FadeSprite(gameObject.GetComponent<SpriteRenderer>(), afterTime, false));
        Invoke("DestroyGameObject", afterTime);
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    #endregion

    public void PlayStepAudio()
    {
        if (EnemyData.StepAudioClip != null)
            AudioManager.Instance.SFXAudioSource.PlayOneShot(EnemyData.StepAudioClip);
    }

    #region OnMouse
    private void OnMouseOver()
    {
        if (_enemyHealthPanel == null)
        {
            Debug.LogWarning("Enemy Health Panel is null!");
            return;
        }

        if (!_isDead)
        {
            _enemyHealthPanel.SetEnemyName(EnemyData.EnemyName);
            _enemyHealthPanel.UpdateHealthPanel(_enemyStats.CurrentHealth / _enemyStats.TotalEnemyHealth.GetValue());
            _enemyHealthPanel.ShowEnemyHealthPanel(true);
        }
    }

    private void OnMouseExit()
    {
        if (_enemyHealthPanel == null)
        {
            Debug.LogWarning("Enemy Health Panel is null!");
            return;
        }

        _enemyHealthPanel.ShowEnemyHealthPanel(false);
        _enemyHealthPanel.ClearHealthPanel();
    }
    #endregion

    public void SetEnemyHealthPanel(EnemyHealthPanel healthPanel)
    {
        if (_enemyHealthPanel != null)
            Destroy(_enemyHealthPanel);

        _enemyHealthPanel = healthPanel;
    }

    private void OnDrawGizmosSelected()
    {
        if (EnemyData == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, EnemyData.FollowRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyData.AttackRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, EnemyData.ChargeRadius);

        if (_surroundingsRadius != 0.0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _surroundingsRadius);
        }

        //Gizmos.DrawWireSphere(transform.position, EnemyData.StopDistance);
    }
}
