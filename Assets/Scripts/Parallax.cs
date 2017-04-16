using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

    public Transform[] BackgroundLayers;
    public float ParallaxScale;
    public float ParallaxReductionFactor;
    public float Smoothing;

    private Vector3 lastPosition;

	void Start () {
        lastPosition = transform.position;
	}

	void Update () {
        
        var parallax = (lastPosition.x - transform.position.x) * ParallaxScale;

        for (var i = 0; i < BackgroundLayers.Length; i++)
        {
            var backgroundPosition = BackgroundLayers[i].position.x + parallax * (i * ParallaxReductionFactor + 1);

            BackgroundLayers[i].position = Vector3.Lerp(
                BackgroundLayers[i].position,
                new Vector3(backgroundPosition, BackgroundLayers[i].position.y, BackgroundLayers[i].position.z),
                Smoothing * Time.deltaTime);

            lastPosition = transform.position;
        }
	}
}
