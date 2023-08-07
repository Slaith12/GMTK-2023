using System;
using UnityEngine;

public class Sieger : MonoBehaviour
{
    public float movementSpeed;
    public float turningSpeed;

    public GameObject arrowPrefab;
    [SerializeField] LevelEnd levelEnd;

    public int attackOrcsAvailable;

    public SiegePath debugInitialPath;

    private Task _currentTask = Task.Advance;
    private int _nextNode;

    private SiegePath _path;
    private float _pathLength;

    private float _pathPercentage;
    private Quaternion _startAngle;
    private Quaternion _targetAngle;
    private float _turnPercentage;

    public bool slowdown;
    private float mspeedslow;
    private float mspeed;

    private OrcCage _cage;

    private void Awake()
    {
        GetComponent<HealthObject>().enabled = false; //prevent HealthObject from initializing until after this has initialized
    }

    private void Start()
    {
        BeginNewPath(debugInitialPath);
        InitModules();
        slowdown = false;
        mspeed = movementSpeed;
        mspeedslow = movementSpeed * .5f;
        _cage = GetComponentInChildren<OrcCage>();
        GetComponent<HealthObject>().enabled = true;
    }

    private void InitModules()
    {
        if (GameManager.gameManager.siegeMachineData == null)
            return;
        foreach(ModuleData module in GameManager.gameManager.siegeMachineData)
        {
            module.type.ModuleEffect(this);
        }
    }

    private void OnDestroy()
    {
        levelEnd.ShowLose();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _cage.SpawnOrcs();
        }
        
        if (!_path) return;
        switch (_currentTask)
        {
            case Task.Advance:
                Advance();
                break;
            case Task.Turn:
                Turn();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void BeginNewPath(SiegePath newPath)
    {
        _path = newPath;
        _pathPercentage = 0;
        _currentTask = Task.Advance;
        _pathLength = _path.GetTotalLength();
        _nextNode = 1;
        _targetAngle = Quaternion.Euler(0, 0, _path.GetDirection(0));
        transform.rotation = _targetAngle;
        _startAngle = _targetAngle;
    }

    private void Advance()
    {
        if (slowdown)
            movementSpeed = mspeedslow;
        else
            movementSpeed = mspeed;
        var distanceToTravel = movementSpeed * Time.deltaTime;
        var percentageToTravel = distanceToTravel / _pathLength;
        _pathPercentage += percentageToTravel;
        transform.position = _path.GetPosition(_pathPercentage);
        var next = _path.NextNodeInd(_pathPercentage);
        if (next != _nextNode)
        {
            _nextNode = next;
            _currentTask = Task.Turn;
            _startAngle = transform.rotation;
            _targetAngle = Quaternion.Euler(0, 0, _path.GetDirection(_pathPercentage));
            _turnPercentage = 0;
        }
    }

    private void Turn()
    {
        if (Quaternion.Angle(_startAngle, _targetAngle) <= 1)
        {
            transform.rotation = _targetAngle;
            _currentTask = Task.Advance;
            return;
        }

        _turnPercentage += turningSpeed * Time.deltaTime * (1 / Quaternion.Angle(_startAngle, _targetAngle));
        transform.rotation = Quaternion.Lerp(_startAngle, _targetAngle, _turnPercentage);
        if (_turnPercentage >= 1.0f)
        {
            transform.rotation = _targetAngle;
            _currentTask = Task.Advance;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        slowdown = (collision.gameObject.GetComponent<SlowdownProj>() != null);
    }

    private enum Task
    {
        Advance,
        Turn
    }
}