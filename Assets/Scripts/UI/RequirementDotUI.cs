using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequirementDotUI : MonoBehaviour
{
    public Image baseImage;
    public Image notAllowedImage;
    public void Setup(bool isRed, bool isNotAllowed = false)
    {
        baseImage.color = isRed ? GridManager.instance.redTileColor : GridManager.instance.blueTileColor;
        if(isNotAllowed)
        {
            notAllowedImage.gameObject.SetActive(true);
        }
    }
}
