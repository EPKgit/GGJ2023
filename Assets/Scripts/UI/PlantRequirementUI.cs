using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantRequirementUI : MonoBehaviour
{
    public GameObject dotPrefab;
    public GameObject dotHolder;
    public Image pathImage;
    public Text pathTextTop;
    public Text pathTextRight;

    public void Setup(Plant plant)
    {
        transform.position = plant.transform.position;

        PlantData data = plant.plantData;
        int totalReqs = data.requiredBlue + data.requiredRed;
        for (int i = 0; i < totalReqs; ++i)
        {
            GameObject g = Instantiate(dotPrefab, Vector3.left * 0.7f, Quaternion.identity, dotHolder.transform);
            g.GetComponent<RequirementDotUI>().Setup(i >= data.requiredBlue);
        }
        if(data.cantHaveBlue)
        {
            GameObject g = Instantiate(dotPrefab, Vector3.left * 0.7f, Quaternion.identity, dotHolder.transform);
            g.GetComponent<RequirementDotUI>().Setup(false, true);
        }
        if (data.cantHaveRed)
        {
            GameObject g = Instantiate(dotPrefab, Vector3.left * 0.7f, Quaternion.identity, dotHolder.transform);
            g.GetComponent<RequirementDotUI>().Setup(true, true);
        }

        if(data.growthPattenSprite == null)
        {
            pathImage.gameObject.SetActive(false);
            return;
        }
        pathImage.sprite = data.growthPattenSprite;
        if(data.growthNumber != 0 || data.isInfinite)
        {
            float deg = 0;
            Text text = null;
            switch (data.growthPattern[0])
            {
                case Direction.DOWN: deg = 180; text = pathTextRight; break;
                case Direction.RIGHT: deg = 270; text = pathTextTop; break;
                case Direction.UP: deg = 0; text = pathTextRight; break;
                case Direction.LEFT: deg = 90; text = pathTextTop; break;
            }
            pathImage.rectTransform.Rotate(new Vector3(0, 0, deg));
            text.text = string.Format("{0}", data.isInfinite ? "∞" : data.growthNumber);
            text.gameObject.SetActive(true);
        }
    }
}
