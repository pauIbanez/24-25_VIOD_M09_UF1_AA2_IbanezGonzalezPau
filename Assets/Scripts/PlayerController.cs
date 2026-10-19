
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPerson
{
  public class PlayerController : MonoBehaviour
  {
    [Tooltip("The mouse sensibility as x: horizontal, y: vertical")]
    [SerializeField] private Vector2 _sensibility = new(0.1f, 0.1f);
    [SerializeField] private Transform _camTransform;
    private Vector2 _verticalClamp = new(-89, 89);


    private float _yaw;
    private float _pitch;

    private void Start()
    {
      _yaw = _camTransform.localEulerAngles.y;
      _pitch = _camTransform.localEulerAngles.x;

      Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
      RotateCamera();
    }


    private void RotateCamera()
    {
      if (Mouse.current == null) return;

      Vector2 delta = Mouse.current.delta.ReadValue() * _sensibility;
      if (delta == Vector2.zero) return;

      _yaw = Mathf.Repeat(_yaw + delta.x, 360f);
      _pitch = Mathf.Clamp(_pitch - delta.y, _verticalClamp.x, _verticalClamp.y);

      _camTransform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
  }
}
