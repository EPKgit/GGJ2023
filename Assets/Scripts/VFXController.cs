using UnityEngine;

public class VFXController : MonoBehaviour
{
    private ParticleSystem effect;
    private bool killing = false;
    void Awake()
    {
        effect = GetComponent<ParticleSystem>();
    }

    public void StopParticlePlaying()
    {
        effect.Stop();
        if (!effect.main.loop)
        {
            Destroy(gameObject);
        }
        else
        {
            killing = true;
        }
    }

    protected virtual void Update()
    {
        if (killing && effect.particleCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
