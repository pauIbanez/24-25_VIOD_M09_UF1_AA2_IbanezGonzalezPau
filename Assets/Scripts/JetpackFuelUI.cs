
using UnityEngine;
using UnityEngine.UI;

namespace FirstPerson
{
  [RequireComponent(typeof(Image))]
  public class JetpackFuelUI : MonoBehaviour
  {
    [SerializeField] private PlayerController _player;
    private Image _image;

    private void Awake()
    {
      _image = GetComponent<Image>();
      _image.type = Image.Type.Filled;

      _image.fillMethod = Image.FillMethod.Vertical;
      _image.fillOrigin = (int)Image.OriginVertical.Bottom;
    }

    private void Update()
    {
      _image.fillAmount = _player.JetpackFuel;
    }
  }
}
