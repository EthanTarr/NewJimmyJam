using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shake : MonoBehaviour {
	public static Shake instance;
	public Vector3 startTransform;
    public ParticleSystem dustParticles;
    public GameObject Spike;
    private List<GameObject> spikePositions;


    [Space()]
    public AudioClip[] rumbles;
    public AudioClip spikeFall;

    private int spikes = 0;

    void Awake(){
		instance = this;
        spikePositions = new List<GameObject>();
        foreach (GameObject spike in GameObject.FindGameObjectsWithTag("Spike"))  {
            spikePositions.Add(spike);
            spikes++;
        }
        startTransform = transform.position;
	}
	public void shake(float t, float strength){
        dustParticles.Emit(UnityEngine.Random.Range(5, 8));

        audioManager.instance.Play(rumbles[Random.Range(0, rumbles.Length - 1)], 0.25f, Random.Range(0.96f, 1.03f));

        if (UnityEngine.Random.value < 0.1f) {
            int Fallingspike = UnityEngine.Random.Range(0, spikes);
			spikePositions.Remove (Spike);
			spikes--;

            audioManager.instance.Play(spikeFall, 0.15f, Random.Range(0.96f, 1.03f));

            if (spikePositions[Fallingspike] != null)
            {
                GameObject temp = (GameObject)Instantiate(Spike, ((GameObject)spikePositions[Fallingspike]).transform.position, Quaternion.identity);
                temp.GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.value * 30);
                Destroy((GameObject)spikePositions[Fallingspike]);
            }
        }

        StartCoroutine(screenshake(t, strength));
		
	}
	
	IEnumerator screenshake(float t, float strength){
		float z = transform.position.z;
		while (t > 0){
			t -= Time.deltaTime*10;
			
			transform.position = new Vector2(startTransform.x,startTransform.y) + Random.insideUnitCircle*strength/8;
			transform.position += new Vector3(0,0,z);
			yield return null;
		}
		transform.position = startTransform;
	}
}
