using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windTurbulenceEnvMang : enviornmentManager {

    [Header("Wind Turbulence")]
    public ParticleSystem[] windowParticles;
    public objectShake[] backgroundBuildings;

    void Start() {
        instance = this;
    }

    public override void enviornmentCall() {

        foreach (ParticleSystem treeParticles in windowParticles) {
            if(Random.Range(0, 100) > 50)
                treeParticles.Emit(UnityEngine.Random.Range(3, 10));
        }

        foreach (objectShake obj in backgroundBuildings)
        {
            obj.shake();
        }
    }
}
