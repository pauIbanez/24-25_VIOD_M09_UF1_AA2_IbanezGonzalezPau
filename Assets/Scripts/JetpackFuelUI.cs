
using UnityEngine;
using UnityEngine.UI;

namespace FirstPerson
{
  [RequireComponent(typeof(Image))]
  public class JetpackFuelUI : MonoBehaviour
  {
    [SerializeField] private PlayerController _player;
    [SerializeField] private Gradient _color = new()
    {
      colorKeys = new[] { new GradientColorKey(Color.red, 0f), new GradientColorKey(Color.white, 1f) }
    };
    [Tooltip("Hide the meter while the jetpack is full")]
    [SerializeField] private bool _hideWhenFull = false;

    private Image _image;

    private void Awake()
    {
      _image = GetComponent<Image>();
      _image.type = Image.Type.Filled;
      _image.fillMethod = Image.FillMethod.Radial360;
      _image.fillOrigin = (int)Image.Origin360.Top;
      _image.fillClockwise = false;
    }

    private void Update()
    {
      float fuel = _player.JetpackFuel;
      _image.fillAmount = fuel;
      _image.color = _color.Evaluate(fuel);
      _image.enabled = !_hideWhenFull || fuel < 1f;
    }
  }
}
