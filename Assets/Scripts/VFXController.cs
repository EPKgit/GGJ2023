using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    private List<ParticleSystem> effects;
    private bool killing = false;
    private bool hasStartedPlaying = false;
    void Awake()
    {
        effects = new List<ParticleSystem>();
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            effects.Add(ps);
        }
    }

    public void StopParticlePlaying()
    {
        if(!hasStartedPlaying) //make sure the system has a chance to start before killing it if we want it dead immediately
        {
            killing = true;
            return;
        }
        bool playing = false;
        foreach (ParticleSystem ps in effects)
        {
            ps.Stop();
            playing |= ps.isPlaying;
        }
        if (!playing) 
        {
            Destroy(gameObject);
        }
        else
        {
            killing = true;
        }
    }

    void Update()
    {
        foreach (ParticleSystem ps in effects)
        {
            if (ps.isPlaying)
            {
                if(killing && !hasStartedPlaying)
                {
                    StopParticlePlaying();
                }
                hasStartedPlaying = true;
                return;
            }
        }
        if (!killing)
        {
            return;
        }
        Destroy(gameObject);
    }
}
