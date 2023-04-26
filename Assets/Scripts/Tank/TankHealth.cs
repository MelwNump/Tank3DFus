using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class TankHealth : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChange))] /*call when variables have changed*/
    public float _currentHealth { get; set; }

    [Networked(OnChanged = nameof(OnStateChange))]
    public bool _isDead { get; set; }

    public float startingHealth = 100f;
    public Slider slider;
    public Image fillImage;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;
    public GameObject explosionPrefab;
    public GameObject manegerTanks;

    private AudioSource _explosionAudio;
    private ParticleSystem _explosionParticles;

    //private float _currentHealth;
    //private bool _isDead;

    private void Awake()
    {
        _explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        _explosionAudio = _explosionParticles.GetComponent<AudioSource>();

        _explosionParticles.gameObject.SetActive(false);
    }
    public override void FixedUpdateNetwork()
    {
        if (_currentHealth <= 0f && !_isDead)
            OnDeath();
    }

    // pt.  OnEnable() play before Start()
    private void Start()
    {
        manegerTanks = GameObject.Find("ManegerTanks");
        manegerTanks.GetComponent<ManegerTanks>().AddPlayer(this.gameObject);

        Respawn();
    }
    public void Respawn()
    {
        _currentHealth = startingHealth;
        _isDead = false;

        SetHealthUI();
    }
    public void TakeDamage(float amount) // change to fixUpdate()
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
        _currentHealth -= amount;

        SetHealthUI();

        // change to fixUpdate()
        /* if (_currentHealth <= 0f && !_isDead)
            OnDeath(); */
    }

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        slider.value = _currentHealth;
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, _currentHealth / startingHealth);
    }

    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        _isDead = true;

        _explosionParticles.transform.position = this.transform.position;
        _explosionParticles.gameObject.SetActive(true);
        _explosionParticles.Play();

        _explosionAudio.Play();
        //this.gameObject.SetActive(false);
    }

    static void OnHPChange(Changed<TankHealth> changed)
    {
        print(changed.Behaviour._currentHealth);
        changed.Behaviour.SetHealthUI();
    }
    static void OnStateChange(Changed<TankHealth> changed)
    {
        print(changed.Behaviour._isDead);
        if (changed.Behaviour._isDead)
        {
            changed.Behaviour.manegerTanks.GetComponent<ManegerTanks>().SetPlayer(changed.Behaviour.gameObject);
        }
    }

    public void TakeHP(int HP)
    {
        if (_currentHealth + HP >= startingHealth)
        {
            _currentHealth = startingHealth;
        }
        else
        {
            _currentHealth += HP;
        }

    }

}