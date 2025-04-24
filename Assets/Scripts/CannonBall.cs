using UnityEngine;

/// <summary>
/// A class for shooting cannon balls
/// </summary>
public class CannonBall : MonoBehaviour
{
    [SerializeField] private Rigidbody _rBody;
    [SerializeField] private float _lifeTime = 15f;

    private void Start()
    {
        if (_lifeTime > 0) Destroy(gameObject, _lifeTime);
    }

    public void Fire(Vector3 direction, float speed)
    {
        _rBody.AddForce(direction * speed, ForceMode.Impulse);
    }
}
