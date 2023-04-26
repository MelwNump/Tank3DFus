using UnityEngine;
using Fusion;

public class TankMovement : NetworkBehaviour
{
    public int playerNumber = 1;
    public float speed = 5f;
    public float turnSpeed = 180f;
    public AudioSource movementAudio;
    public AudioClip engineIdling;
    public AudioClip engineDriving;
    public float pitchRange = 0.2f;
    
    private string _movementAxis;
    private string _turnAxis;
    private Rigidbody _rb;
    private float _movementInput;
    private float _turnInput;
    private float _originalPitch;

    [Networked]
    public Vector3 MovementDirection { get; set; }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _rb.isKinematic = false;
        _movementInput = 0f;
        _turnInput = 0f;
    }

    private void OnDisable()
    {
        _rb.isKinematic = true;
    }

    private void Start()
    {
        _movementAxis = "Vertical" + playerNumber;
        _turnAxis = "Horizontal" + playerNumber;

        _originalPitch = movementAudio.pitch;
    }
    private void Update()
    {
        /*// Store the player's input and make sure the audio for the engine is playing.
        _movementInput = Input.GetAxis(_movementAxis);
        _turnInput = Input.GetAxis(_turnAxis);

        EngineAudio(); */
    }

    public override void FixedUpdateNetwork()
    {
        Vector3 direction;
        if (GetInput(out NetworkInputPrototype input))
        {
            direction = default;
            if (input.IsDown(NetworkInputPrototype.BUTTON_FORWARD))
            {
                direction += Vector3.forward;
            }
            if (input.IsDown(NetworkInputPrototype.BUTTON_BACKWARD))
            {
                direction -= Vector3.forward;
            }
            if (input.IsDown(NetworkInputPrototype.BUTTON_LEFT))
            {
                direction -= Vector3.right;
            }
            if (input.IsDown(NetworkInputPrototype.BUTTON_RIGHT))
            {
                direction += Vector3.right;
            }

            direction = direction.normalized;
            MovementDirection = direction;
        }
        else
        {
            direction = MovementDirection;
        }

        EngineAudio();
       // SpeedD();

    }

    /*public void SpeedD()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 movement = transform.forward * MovementDirection.z * speedD * Time.deltaTime;
        }
    }*/
    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(_movementInput) < 0.1f && Mathf.Abs(_turnInput) < 0.1f)
        {
            if (movementAudio.clip == engineDriving)
            {
                movementAudio.clip = engineIdling;
                movementAudio.pitch = Random.Range(_originalPitch - pitchRange, _originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
        else
        {
            if (movementAudio.clip == engineIdling)
            {
                movementAudio.clip = engineDriving;
                movementAudio.pitch = Random.Range(_originalPitch - pitchRange, _originalPitch + pitchRange);
                movementAudio.Play();
            }
        }
    }

    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }

    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        //Vector3 movement = transform.forward * _movementInput * speed * Time.deltaTime;
        Vector3 movement = transform.forward * MovementDirection.z * speed * Time.deltaTime;

        _rb.MovePosition(_rb.position + movement);
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        //float turn = _turnInput * turnSpeed * Time.deltaTime;
        float turn = MovementDirection.x * turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        _rb.MoveRotation(_rb.rotation * turnRotation);
    }
    public void TakeSpeed(float Speed)
    {
       if(speed < 20)
        speed += Speed;
    }
}