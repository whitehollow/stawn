using UnityEngine;

public interface IFloatingTextposition
{
    bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size);
}

