using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Sieger : MonoBehaviour
{
    private enum Task
    {
        Advance,
        Turn
    }

    public float movementSpeed;
    public float turningSpeed;

    public SiegePath debugInitialPath;

    private SiegePath _path;
    private Quaternion _targetAngle;
    private Quaternion _startAngle;
    private float _turnPercentage;

    private Task _currentTask = Task.Advance;

    private float _pathPercentage = 0;
    private float _pathLength = 0;
    private int _nextNode = 0;

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

    private void Start()
    {
        BeginNewPath(debugInitialPath);
    }

    private void Update()
    {
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

    private void Advance()
    {
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
}