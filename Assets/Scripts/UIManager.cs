using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField] private Image _powerBar;

    public void UpdatePowerBar(float power)
    {
        if (_powerBar != null)
        {
            _powerBar.fillAmount = power;
        }
    }
}
