
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPerson
{
  [RequireComponent(typeof(Rigidbody))]
  public class PlayerController : MonoBehaviour
  {
    [Tooltip("The mouse sensibility as x: horizontal, y: vertical")]
    [SerializeField] private Vector2 _sensibility = new(0.1f, 0.1f);
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float _movementSpeed = 10;
    [SerializeField] private float _sidewaysMsMultiplyer = 0.75f;
    [SerializeField] private float _backwardsMsMultiplyer = 0.5f;
    [SerializeField] private float _jumpForce = 5;
    private Vector2 _verticalClamp = new(-89, 89);

    private float _yaw;
    private float _pitch;
    private Rigidbody _rb;

    private Vector3 _wishDir;
    private bool _jumpQueued;

    private void Start()
    {
      _yaw = transform.localEulerAngles.y;
      _pitch = _camTransform.localEulerAngles.x;
      _rb = GetComponent<Rigidbody>();
      _rb.freezeRotation = true;
      _rb.interpolation = RigidbodyInterpolation.Interpolate;

      // No friction so the player doesn't stick to walls
      GetComponent<Collider>().material = new PhysicsMaterial("PlayerNoFriction")
      {
        dynamicFriction = 0f,
        staticFriction = 0f,
        frictionCombine = PhysicsMaterialCombine.Minimum
      };

      Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
      RotateCamera();
      ReadMoveInput();
    }

    private void FixedUpdate()
    {
      _rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));

      if (!IsGrounded()) { _jumpQueued = false; return; }

      if (_jumpQueued) { Jump(); _jumpQueued = false; return; }
      Move();
    }

    private void ReadMoveInput()
    {
      Vector2 dir = Vector2.zero;
      if (Keyboard.current.wKey.isPressed) dir.y += 1;
      if (Keyboard.current.sKey.isPressed) dir.y -= 1;
      if (Keyboard.current.aKey.isPressed) dir.x -= 1;
      if (Keyboard.current.dKey.isPressed) dir.x += 1;
      _wishDir = new Vector3(dir.x, 0f, dir.y).normalized;

      if (Keyboard.current.spaceKey.wasPressedThisFrame) _jumpQueued = true;
    }

    private void Move()
    {



      /**
      * Could use dot and linearly interpolate between states, however 1 - 0.5 is linear already, we can plot the multipyer
      * 0.75f + dot * 0.25f
      * If we wantthe values modifyable we do need the lerp:
      * dot >= 0f  ? Mathf.Lerp(0.75f, 1.0f, dot) : Mathf.Lerp(0.75f, 0.5f, -dot);
      * I hoever will use the cheapest with is NO interpolation and just snap for angle quadrants
      */


      Vector3 camForward = _camTransform.forward;
      camForward.y = 0;
      camForward.Normalize();


      float dot = Vector3.Dot(camForward, _wishDir);
      float _msMultiplyer = dot > 0.25f ? 1 : dot < -0.25f ? _backwardsMsMultiplyer : _sidewaysMsMultiplyer;


      Vector3 target = _rb.rotation * _wishDir * _movementSpeed * _msMultiplyer;

      Vector3 current = _rb.linearVelocity;
      _rb.AddForce(new Vector3(target.x - current.x, 0f, target.z - current.z), ForceMode.VelocityChange);
    }

    private void Jump()
    {
      _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, _jumpForce, _rb.linearVelocity.z);
    }

    private void RotateCamera()
    {
      if (Mouse.current == null) return;

      Vector2 delta = Mouse.current.delta.ReadValue() * _sensibility;
      if (delta == Vector2.zero) return;

      _yaw = Mathf.Repeat(_yaw + delta.x, 360f);
      _pitch = Mathf.Clamp(_pitch - delta.y, _verticalClamp.x, _verticalClamp.y);

      _camTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private bool IsGrounded() => Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, LayerMask.GetMask("Ground"));
  }
}
