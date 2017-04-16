﻿using UnityEngine;

public class FromWorldPointTextPositioner : IFloatingTextposition
{
    private readonly Camera _camera;
    private readonly Vector3 _wordPosition;
    private readonly float _speed;
    private float _timeToLive;
    private float _yOffset;

    public FromWorldPointTextPositioner(Camera camera, Vector3 wordPosition, float timeToLive, float speed)
    {
        _camera = camera;
        _wordPosition = wordPosition;
        _speed = speed;
        _timeToLive = timeToLive;
    }

    public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
    {
        if ((_timeToLive -= Time.deltaTime) <= 0)
            return false;

        var screenPosition = _camera.WorldToScreenPoint(_wordPosition);

        position.x = screenPosition.x - (size.x / 2);

        position.y = Screen.height - screenPosition.y - _yOffset;

        _yOffset += Time.deltaTime * _speed;
        return true;
    }
}

