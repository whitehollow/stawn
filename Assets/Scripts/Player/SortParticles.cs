using UnityEngine;
using System.Collections;

public class SortParticles : MonoBehaviour {

    public string SortingLayerName = "Particles";

	void Start () {
        particleSystem.renderer.sortingLayerName = SortingLayerName;
	}
	
}
