using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class screenTransition : MonoBehaviour {

    public static screenTransition instance;
    public Material TransitionMaterial;
    [Range(0, 1)] public float fade;
    [Range(0, 1)] public float cutoff;
    public Color screenColor;

    private void Start()
    {
        if (Application.isPlaying) {
            instance = this;
            cutoff = 1;
            StartCoroutine("fadeIn");
        }

    }

    IEnumerator fadeIn() {
        cutoff = 1;
        yield return new WaitForSeconds(0.25f);
        while (cutoff > 0) {
            cutoff -= 0.075f;
            yield return new WaitForEndOfFrame();
        }
        
    }

    public IEnumerator fadeOut(string nextLevel) {
        yield return new WaitForSeconds(1);
        cutoff = 1;
        fade = 0;
        while (fade < 1)  {
            fade += 0.01f; ;
            yield return new WaitForEndOfFrame();
        }
        Application.LoadLevel(nextLevel);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        TransitionMaterial.SetFloat("_Cutoff", cutoff);
        TransitionMaterial.SetFloat("_fade", fade);
        TransitionMaterial.SetColor("_Color", screenColor);
        Graphics.Blit(source, destination, TransitionMaterial);
    }
}
