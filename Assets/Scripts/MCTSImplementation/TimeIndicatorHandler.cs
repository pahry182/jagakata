using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeIndicatorHandler : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = GameSceneController.Instance.currentBarProgression;
        float maxTime = GameSceneController.Instance.currentMaxBarProgression;

        image.fillAmount = currentTime / maxTime;
        //R 118 188 70
        //G 188 24 - 164
        //B 24
        byte red = (byte)(188 - (70 * (currentTime / maxTime)));
        byte green = (byte)(24 + (164 * (currentTime / maxTime)));
        byte blue = 24;
        image.color = new Color32(red, green, blue, 255);
    }
}
