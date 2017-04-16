using UnityEngine;

class AutoDestroyParticles : MonoBehaviour
{
    private ParticleSystem particle;

    public void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (particle.isPlaying)
            return;

        Destroy(gameObject);
    }
}
