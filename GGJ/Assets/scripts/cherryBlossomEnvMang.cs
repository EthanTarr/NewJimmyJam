using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cherryBlossomEnvMang : enviornmentManager {

    [Header("cherryBlossomStuff")]
    public objectShake[] treeParts;
    public ParticleSystem[] treeParticles;

	// Use this for initialization
	void Start () {
        instance = this;
    }

    public override void enviornmentCall() {

        foreach (ParticleSystem treeParticles in treeParticles) {
            treeParticles.Emit(UnityEngine.Random.Range(5, 10));
        }

        foreach (objectShake obj in treeParts) {
            obj.shake();
        }
    }
}
