using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class SlidersEvent : UnityEvent<float[], int>
{

}

public class SliderManager : MonoBehaviour
{
    public SlidersEvent SliderValueChanged;

    Slider[] sliders;

    // Start is called before the first frame update
    void Start()
    {
        sliders = GetComponentsInChildren<Slider>();
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].onValueChanged.AddListener((value) =>
            {
                SliderValueChanged.Invoke(sliders.Select(s => s.value).ToArray(), i);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValues(float[] values)
    {
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].value = values[i];
        }
    }
}
