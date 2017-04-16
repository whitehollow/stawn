﻿using UnityEngine;

class CenterTextPositioner : IFloatingTextposition
{
    private readonly float _speed;
    private float _textPosition;

    public CenterTextPositioner(float speed)
    {
        _speed = speed;
    }

    public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
    {
        _textPosition += Time.deltaTime * _speed;

        if (_textPosition > 1)
            return false;

        position = new Vector2(Screen.width / 2 - size.x / 2f, Mathf.Lerp(Screen.height / 2f + size.y, 0, _textPosition));
        
        return true;
    }
}

