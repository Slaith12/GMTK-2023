using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SiegePath : MonoBehaviour
{
    public Vector2[] path;
    private readonly List<Vector3> _path3 = new();
    private float _totalLength = 0;
    private readonly List<Tuple<float, Vector3>> _percentageToNode = new();

    private void Awake()
    {
        RefreshPathData();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            RefreshPathData();
        }
    }

    private void RefreshPathData()
    {
        if (path.Length < 2) return;
        _path3.Clear();
        foreach (var p in path)
        {
            _path3.Add(new Vector3(p.x, p.y, 0));
        }

        _totalLength = CalculateTotalLength();
        _percentageToNode.Clear();
        var counter = 0f;
        for (var i = 0; i < path.Length - 1; i++)
        {
            var distance = Vector2.Distance(path[i], path[i + 1]);
            var percentage = distance / _totalLength;
            _percentageToNode.Add(Tuple.Create(counter, _path3[i]));
            counter += percentage;
        }
        _percentageToNode.Add(Tuple.Create(1f, _path3[^1]));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.5f, 0);
        var i = 0;
        foreach (var p in _path3)
        {
            if (i == 0 || i == _path3.Count - 1)
            {
                Gizmos.DrawSphere(p, 0.25f);
            }
            else
            {
                Gizmos.DrawSphere(p, 0.125f);   
            }
            i++;
        }
        Gizmos.DrawLineStrip(_path3.ToArray(), false);
    }

    private float CalculateTotalLength()
    {
        var counter = 0f;
        for (var i = 0; i < path.Length - 1; i++)
        {
            counter += Vector2.Distance(path[i], path[i + 1]);
        }

        return counter;
    }

    public float GetTotalLength()
    {
        if (_totalLength == 0)
        {
            // update order got messed up, recalculate
            _totalLength = CalculateTotalLength();
        }
        return _totalLength;
    }

    public Tuple<int, Vector3> GetNextNode(float percentage)
    {
        float currentPercentage = 0;
        var node = _path3[0];
        var next = 0;
        while (currentPercentage < percentage)
        {
            next++;
            var tuple = _percentageToNode[next];
            currentPercentage = tuple.Item1;
            node = tuple.Item2;
        }

        return Tuple.Create(next, node);
    }

    public Tuple<int, Vector3> GetLastNode(float percentage)
    {
        var next = GetNextNode(percentage);
        if (next.Item1 == 0) return Tuple.Create(0, next.Item2);
        return Tuple.Create(next.Item1 - 1, _percentageToNode[next.Item1 - 1].Item2);
    }

    public Vector3 GetPosition(float percentage)
    {
        var next = GetNextNode(percentage);
        var last = GetLastNode(percentage);
        if (last.Item1 == next.Item1) return next.Item2; // we're at the start of the path
        var percentAtLast = _percentageToNode[last.Item1].Item1;
        var percentAtNext = _percentageToNode[next.Item1].Item1;
        var percentBetween = (percentage - percentAtLast) / (percentAtNext - percentAtLast);
        return Vector3.Lerp(last.Item2, next.Item2, percentBetween);
    }

    public float DistanceToNode(float percentage)
    {
        var next = GetNextNode(percentage).Item2;
        return Vector3.Distance(GetPosition(percentage), next);
    }

    public int NextNodeInd(float percentage)
    {
        return GetNextNode(percentage).Item1;
    }

    public float GetDirection(float percentage)
    {
        var next = (Vector2)GetNextNode(percentage).Item2;
        var current = (Vector2)GetPosition(percentage);
        var direction = next - current;
        var angle = Vector2.SignedAngle(Vector2.up, direction.normalized);
        return angle;
    }
}
