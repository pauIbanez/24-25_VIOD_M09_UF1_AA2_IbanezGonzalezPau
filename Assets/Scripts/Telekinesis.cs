
using UnityEngine;
using UnityEngine.InputSystem;

namespace FirstPerson
{
  public class Telekinesis : MonoBehaviour
  {
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float _pickupRange = 2f;
    [Tooltip("How fast the held object moves towards the center of the screen")]
    [SerializeField] private float _followSpeed = 15f;
    [SerializeField] private float _minHoldDistance = 1f;
    [SerializeField] private float _maxHoldDistance = 5f;
    [Tooltip("Distance moved per scroll wheel notch")]
    [SerializeField] private float _scrollStep = 0.25f;

    private Rigidbody _current;
    private float _holdDistance;
    private bool _hadGravity;
    private CollisionDetectionMode _prevCollisionMode;
    private bool HasObject => _current != null;

    private void Start()
    {
      if (!_camTransform) _camTransform = Camera.main.transform;
    }

    private void Update()
    {
      if (Mouse.current == null) return;

      if (!HasObject && Mouse.current.rightButton.isPressed) TryPickup();
      else if (HasObject && !Mouse.current.rightButton.isPressed) Drop();

      if (HasObject) ReadScrollInput();
    }

    private void ReadScrollInput()
    {
      float scroll = Mouse.current.scroll.ReadValue().y;
      if (scroll == 0f) return;

      _holdDistance = Mathf.Clamp(_holdDistance + Mathf.Sign(scroll) * _scrollStep, _minHoldDistance, _maxHoldDistance);
    }

    private void FixedUpdate()
    {
      if (!HasObject) return;

      // Drive the velocity instead of setting the position so the object still collides with the world
      Vector3 target = _camTransform.position + _camTransform.forward * _holdDistance;
      _current.linearVelocity = (target - _current.position) * _followSpeed;
      _current.angularVelocity = Vector3.zero;
    }

    private void TryPickup()
    {
      // Raycast against everything so walls block the pickup, then check the layer of what we hit
      if (!Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _pickupRange)) return;
      Debug.Log(hit.collider.gameObject.name);
      if (!hit.rigidbody || hit.collider.gameObject.layer != LayerMask.NameToLayer("Pickable")) return;

      _current = hit.rigidbody;
      _holdDistance = Mathf.Clamp(hit.distance, _minHoldDistance, _maxHoldDistance);
      _hadGravity = _current.useGravity;
      _current.useGravity = false;

      // Continuous detection so the object doesn't tunnel through the scene when moved fast
      _prevCollisionMode = _current.collisionDetectionMode;
      _current.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Drop()
    {
      // Velocity is left untouched so the object keeps its momentum and can be thrown
      _current.useGravity = _hadGravity;
      _current.collisionDetectionMode = _prevCollisionMode;
      _current = null;
    }
  }
}
