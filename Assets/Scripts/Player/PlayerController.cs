using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{

    PlayerControls controls;

    [Header("Camera View")]
    public Transform cameraTarget;
    public Transform viewPoint;
    public float mouseSensivity = 1f;
    public GameObject CM_Camera;
    private float verticalRotStore;
    private Vector2 mouseInput = new();
    private Animator cameraAnimator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float turboSpeed = 15f;
    private bool isTurbo = false;
    private Vector3 moveDir, movement = new();

    [Header("Shooting")]
    public GameObject laserBeamPrefab;
    public Transform shootingPointRight;
    public Transform shootingPointLeft;

    [Header("Radar")]
    public GameObject pulse;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Enable();

        controls.Gameplay.Shoot1.performed += ctx => Shoot();
        controls.Gameplay.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => moveDir = Vector3.zero;
        controls.Gameplay.Rotation.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Rotation.canceled += ctx => mouseInput = Vector2.zero;
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            CM_Camera.SetActive(false);
            pulse.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        cameraAnimator = cameraTarget.GetComponent<Animator>();
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            isTurbo = controls.Gameplay.Turbo.IsInProgress();

            SetRotation(mouseInput);

            SetMovement();
        }
    }
    private void Shoot()
    {

        GameObject laserBeam1 = PhotonNetwork.Instantiate(laserBeamPrefab.name, shootingPointRight.position, shootingPointRight.rotation);
        GameObject laserBeam2 = PhotonNetwork.Instantiate(laserBeamPrefab.name, shootingPointLeft.position, shootingPointRight.rotation);
    }

    private void SetMovement()
    {
        movement = transform.forward * 3f;

        if(isTurbo)
        {
            transform.position += movement * turboSpeed * Time.deltaTime;
            cameraAnimator.SetBool("isTurbo", true);
        } else
        {
            transform.position += movement * moveSpeed * Time.deltaTime;
            cameraAnimator.SetBool("isTurbo", false);
        }
    }

    private void SetRotation(Vector2 _mouseInput)
    { 
        if(transform.up.y < 0.0f)
        {
            _mouseInput.x *= -1f;
        }

        transform.rotation = Quaternion.Euler(-verticalRotStore, transform.rotation.eulerAngles.y + _mouseInput.x * mouseSensivity, transform.rotation.eulerAngles.z);

        verticalRotStore += _mouseInput.y;
        //verticalRotStore = Mathf.Clamp(verticalRotStore, -90f, 90f);

        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(photonView.IsMine)
        {
            if(collision.gameObject.CompareTag("Bullet") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Player"))
            {
                PlayerSpawner.instance.Die();
            }
        }
    }
}
