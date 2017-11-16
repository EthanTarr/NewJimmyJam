using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public int[] playerScores;

    public bool randomMap;
	public bool ConeHeadMode = false;
    public int gamesToWin;
    private UnityEngine.UI.Toggle ConeHeadToggle;
    [Space()]
    public int numOfPlayers = 2;
    public GameObject[] selectedCharacters;

    [Header("Modifiers")]
    public int maxSmashPower = 25;
    public float bounciness = 0.85f;
    public bool airControl;
    public bool seperateDashCooldown;
    public bool canDashOnGround;
    public bool instantBounceKill;
    public float maxSmashSpeed = 30f;
    public float maxChargeTime = 1.5f;
    public bool fullChargeInvinc;
    public bool holdMaxSmash;
    public bool tightDash;
    public bool doubleJump;
    public float dashDistance = 15;

    void Awake() {

        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        } else {
			Destroy(this.gameObject);
        }
        playerScores = new int[numOfPlayers];
        if (ConeHeadToggle == null) {
            //ConeHeadToggle = GameObject.Find("ConeHeadToggle").GetComponent<UnityEngine.UI.Toggle>();
            //Debug.Log("1" + ConeHeadToggle);
            //GameObject.Find("GameOptions").active = false;
        }

        
    }

    private void Start() {
        if(GameObject.Find("ConeHeadToggle") != null)
            ConeHeadToggle = GameObject.Find("ConeHeadToggle").GetComponent<UnityEngine.UI.Toggle>();

        GameManager.instance.isConeHeadMode();

        if(GameObject.Find("GameOptions") != null)
            GameObject.Find("GameOptions").active = false;
    }

    public int totalScores() {
        int total = 0;
        foreach (int i in playerScores) {
            total += i;
        }

        return total;
    }

    public int highestScore() {
        int max = 0;
        foreach (int i in playerScores) {
            max = Mathf.Max(max, i);
        }
        return max;
    }

    public void isConeHeadMode() {
        ConeHeadMode = !ConeHeadMode;

    }

    public void maxGames() {
        int.TryParse(GameObject.Find("GameCounter").GetComponent<UnityEngine.UI.InputField>().text, out gamesToWin);
    }

    public void increaseGames(int increment) {
        gamesToWin += increment;
        GameObject.Find("GameCounter").GetComponent<Text>().text = "" + gamesToWin;
    }

    public void setRandom(bool random) {
        randomMap = random;
    }

    public void clearScore() {
        for (int j = 0; j < numOfPlayers; j++) {
            playerScores[j] = 0;
        }
    }
}
