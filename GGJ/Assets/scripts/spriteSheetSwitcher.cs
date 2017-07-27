using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class spriteSheetSwitcher : MonoBehaviour {

    public Sprite spriteSheet;
    public Sprite[] subsprites;

    private void LateUpdate() {
        string spriteSheetName =  "spriteSheets/" + spriteSheet.name;
        spriteSheetName = spriteSheetName.Split('_')[0];

        subsprites = Resources.LoadAll<Sprite>(spriteSheetName);


        string spriteName = GetComponent<SpriteRenderer>().sprite.name;
        string[] hoi = spriteName.Split('_');
        var newSprite = subsprites[int.Parse(hoi[1])];

        if (newSprite) {
            GetComponent<SpriteRenderer>().sprite = newSprite;          
        }
    }
}
