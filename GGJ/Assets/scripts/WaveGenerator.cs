using UnityEngine;
using UnityEngine.Networking;

public class WaveGenerator : NetworkBehaviour {

    public bool isOnline;

	public GameObject pulse;
	public GameObject antiPulse; 
    public static WaveGenerator instance;

    public Color fadeIn;

	// Use this for initialization
	void Start () {
        instance = this;
    }

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("t")) {
			makeWave (); 
		}
	}
     

	void makeWave() {
        makeWave(transform.position, -1, fadeIn, 5, null);
	}

    public void StartWave(){
        makeWave(transform.position, 0, fadeIn, 5, null);
    }

    public void makeWave(Vector2 position, float amplitude, Color color, float velocity, Transform centerOfGravity) {
        if (isOnline) {
            CmdMakeWave(position, amplitude, color, velocity);
            return;
        }

        GameObject Pulse = Instantiate(pulse, new Vector3(position.x, position.y, 0), Quaternion.identity);

		Pulse.GetComponent<PulseMove> ().Amplitude = amplitude;
        Pulse.GetComponent<PulseMove>().color = color;
        Pulse.GetComponent<PulseMove>().speed = velocity;
        Pulse.GetComponent<PulseMove>().centerOfGravity = centerOfGravity;

        GameObject AntiPulse = Instantiate (antiPulse, new Vector3(position.x, position.y, 0), Quaternion.identity);
		AntiPulse.GetComponent<AntiPulseMove> ().Amplitude = amplitude;
        AntiPulse.GetComponent<AntiPulseMove>().color = color;
        AntiPulse.GetComponent<AntiPulseMove>().speed = velocity;
        AntiPulse.GetComponent<AntiPulseMove>().centerOfGravity = centerOfGravity;
    }

    [Command]
    public void CmdMakeWave(Vector2 position, float amplitude, Color color, float velocity) {
        GameObject Pulse = Instantiate(pulse, new Vector3(position.x, position.y, 0), Quaternion.identity);

        Pulse.GetComponent<PulseMove>().Amplitude = amplitude;
        Pulse.GetComponent<PulseMove>().color = color;
        Pulse.GetComponent<PulseMove>().speed = velocity;

        NetworkServer.Spawn(Pulse);

        GameObject AntiPulse = Instantiate(antiPulse, new Vector3(position.x, position.y, 0), Quaternion.identity);
        AntiPulse.GetComponent<AntiPulseMove>().Amplitude = amplitude;
        AntiPulse.GetComponent<AntiPulseMove>().color = color;
        AntiPulse.GetComponent<AntiPulseMove>().speed = velocity;

        NetworkServer.Spawn(AntiPulse);
    }
}



