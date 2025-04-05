using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScoreIncrement : MonoBehaviour
{

    private void OnEnable()
    {
        Text text = transform.GetComponent<Text>();

        text.CrossFadeAlpha(0f, 1f, false);
    }
}
