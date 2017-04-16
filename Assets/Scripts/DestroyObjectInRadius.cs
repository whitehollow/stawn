using UnityEngine;
public class DestroyObjectInRadius : MonoBehaviour
{
    public LayerMask layers;

    private void destroyObjectsInRadius(Vector2 source, float radius)
    {
        //Collider2D[] elements = Physics2D.OverlapCircleAll(source, radius, 1 << LayerMask.NameToLayer(layers));

        //for (var i = 0; i < elements.Length; i++)
        //{
        //    Debug.Log(elements[i]);
        //}
    }

    public void Update()
    {
        //destroyObjectsInRadius(transform.position, 1000.0f);
        //Gizmos.DrawSphere(transform.position, 100.0f);
    }
}

