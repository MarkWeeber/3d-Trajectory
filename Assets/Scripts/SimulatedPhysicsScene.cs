using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulatedPhysicsScene : MonoBehaviour
{
    [SerializeField] private Transform _objectsParent;
    [SerializeField] private string _obstacleTag = "Obstacle";
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private int _maxPhysicsSimulations = 20;

    private Scene _simulatedScene;
    private PhysicsScene _physicsScene;
    private GameObject _instantiatedObject;
    private GameObject _simulatedObject;
    private Renderer _objectRenderer;
    private CannonBall _cannonBall;

    private void Start()
    {
        CreateSimulatedPhysicsScene();
    }

    private void CreateSimulatedPhysicsScene()
    {
        _simulatedScene = SceneManager.CreateScene("SimulatedPhysicsScene", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulatedScene.GetPhysicsScene();
        foreach (Transform item in _objectsParent)
        {
            if (item.CompareTag(_obstacleTag))
            {
                _instantiatedObject = Instantiate(item.gameObject, item.position, item.rotation);
                if (_instantiatedObject.TryGetComponent<Renderer>(out _objectRenderer))
                {
                    _objectRenderer.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(_instantiatedObject, _simulatedScene);
            }
        }
    }

    public void SimulatePhysics(GameObject cannonBallPrefab, Vector3 position, Vector3 velocity)
    {
        _simulatedObject = Instantiate(cannonBallPrefab, position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(_simulatedObject, _simulatedScene);
        _cannonBall = _simulatedObject.GetComponent<CannonBall>();
        _cannonBall.Fire(velocity);
        _cannonBall.DisableRenderer();
        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = _maxPhysicsSimulations;
            _lineRenderer.SetPosition(0, position);
            for (int i = 1; i < _maxPhysicsSimulations; i++)
            {
                _physicsScene.Simulate(Time.fixedDeltaTime * 3f);
                _lineRenderer.SetPosition(i, _simulatedObject.transform.position);
                
            }
        }
        Destroy(_simulatedObject);
    }

    public void ClearLineRenderer()
    {
        _lineRenderer.positionCount = 0;
    }

}
