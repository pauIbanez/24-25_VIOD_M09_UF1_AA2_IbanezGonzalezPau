
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPerson
{
  public class PlayerController : MonoBehaviour
  {
    [Tooltip("The mouse sensibility as x: horizontal, y: vertical")]
    [SerializeField] private Vector2 _sensibility = new(0.1f, 0.1f);
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float _ms = 10;
    [SerializeField] private float _jumpForce = 5;
    private Vector2 _verticalClamp = new(-89, 89);

    private float _yaw;
    private float _pitch;
    private Rigidbody _rb;

    private void Start()
    {
      _yaw = transform.localEulerAngles.y;
      _pitch = _camTransform.localEulerAngles.x;
      _rb = GetComponent<Rigidbody>();

      Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
      RotateCamera();
      Debug.Log(IsGrounded());
      if (!IsGrounded()) return;
      Move();
    }

    private void Move()
    {
      if (Keyboard.current.spaceKey.wasPressedThisFrame) { Jump(); return; }

      Vector2 dir = Vector2.zero;
      if (Keyboard.current.wKey.isPressed) dir.y += 1;
      if (Keyboard.current.sKey.isPressed) dir.y -= 1;
      if (Keyboard.current.aKey.isPressed) dir.x -= 1;
      if (Keyboard.current.dKey.isPressed) dir.x += 1;

      Vector3 realDir = transform.TransformDirection(new Vector3(dir.x, 0f, dir.y).normalized);
      _rb.linearVelocity = new Vector3(realDir.x * _ms, _rb.linearVelocity.y, realDir.z * _ms);
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

      transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
      _camTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private bool IsGrounded() => Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.2f, LayerMask.GetMask("Ground"));
  }
}
