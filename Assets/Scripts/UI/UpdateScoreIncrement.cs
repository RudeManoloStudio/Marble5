using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateScoreIncrement : MonoBehaviour
{
    private void OnEnable()
    {
        TMP_Text text = transform.GetComponent<TMP_Text>();
        text.CrossFadeAlpha(0f, 1f, false);
    }
}
