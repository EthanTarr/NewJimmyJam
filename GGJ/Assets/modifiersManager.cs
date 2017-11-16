using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class modifiersManager : MonoBehaviour {

    public InputField smashDebugInput;
    public InputField smashSpeedInput;
    public InputField bouncinessDebugInput;
    public InputField maxChargeTimeInput;
    public InputField dashDistanceInput;

    public Toggle airControlInput;
    public Toggle groundDashInput;
    public Toggle separeteDashCooldownInput;
    public Toggle fullChargeInvicInput;
    public Toggle holdMaxSmashInput;
    public Toggle tightDashInput;
    public Toggle instantBounceKillInput;
    public Toggle doubleJumpInput;

    GameManager gm;

    private void Start() {
        gm = GameManager.instance;
        resetToggleOptions();
    }

    void resetToggleOptions() {
        smashDebugInput.text = "" + gm.maxSmashPower;
        smashSpeedInput.text = "" + gm.maxSmashSpeed;
        bouncinessDebugInput.text = "" + gm.bounciness;
        dashDistanceInput.text = "" + gm.dashDistance;
        maxChargeTimeInput.text = "" + gm.maxChargeTime;

        instantBounceKillInput.isOn = gm.instantBounceKill;
        airControlInput.isOn = gm.airControl;
        groundDashInput.isOn = gm.canDashOnGround;
        separeteDashCooldownInput.isOn = gm.seperateDashCooldown;
        holdMaxSmashInput.isOn = gm.holdMaxSmash;
        fullChargeInvicInput.isOn = gm.fullChargeInvinc;
        tightDashInput.isOn = gm.tightDash;
        doubleJumpInput.isOn = gm.doubleJump;
    }

    public void updateModifiers() {
        gm.maxSmashPower = int.Parse(smashDebugInput.text);
        gm.maxSmashSpeed = float.Parse(smashSpeedInput.text);
        gm.bounciness = float.Parse(bouncinessDebugInput.text);
        gm.dashDistance = float.Parse(dashDistanceInput.text);
        gm.maxChargeTime = float.Parse(maxChargeTimeInput.text);

        gm.instantBounceKill = instantBounceKillInput.isOn;

        gm.airControl = airControlInput.isOn;
        gm.canDashOnGround = groundDashInput.isOn;
        gm.seperateDashCooldown = separeteDashCooldownInput.isOn;
        gm.holdMaxSmash = holdMaxSmashInput.isOn;
        gm.fullChargeInvinc = fullChargeInvicInput.isOn;
        gm.tightDash = tightDashInput.isOn;
        gm.doubleJump = doubleJumpInput.isOn;
    }
}
