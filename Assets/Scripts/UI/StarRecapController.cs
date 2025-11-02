using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarRecapController : MonoBehaviour
{

    [SerializeField] private Sprite fullStar;
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Transform firstStarTransform;
    [SerializeField] private Transform secondStarTransform;
    [SerializeField] private Transform thirdStarTransform;

    public void SetStarsHighlight(int x)
    {

        firstStarTransform.gameObject.GetComponent<Image>().sprite = emptyStar;
        secondStarTransform.gameObject.GetComponent<Image>().sprite = emptyStar;
        thirdStarTransform.gameObject.GetComponent<Image>().sprite = emptyStar;

        if (x >= 1)
        {
            firstStarTransform.gameObject.GetComponent<Image>().sprite = fullStar;
        }

        if (x >= 2)
        {
            secondStarTransform.gameObject.GetComponent<Image>().sprite = fullStar;
        }

        if (x >= 3)
        {
            thirdStarTransform.gameObject.GetComponent<Image>().sprite = fullStar;
        }

    }


}
