using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ubiq.Messaging;
using Unity.VisualScripting.Antlr3.Runtime;

public class WindSpeedSlider : MonoBehaviour
{
    NetworkContext context;

    public Slider slider;
    public TextMeshProUGUI sliderText;
    public Wind wind;
    public float MaxWindSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        wind = GameObject.Find("Wind").GetComponent<Wind>();

        // Optionally, you can add a listener for when the slider value changes
        //slider.onValueChanged.AddListener(delegate { OnSliderChanged(); });

    }
    private struct Message
    {
        public float value;
    }

    // public void testSliderChange()
    // {
    //     Debug.Log("Slider Changed: " + slider.value);
    // }

    // Update is called once per frame
    void Update()
    {
        //sliderText.text = slider.value.ToString();
        wind.speed = MaxWindSpeed * slider.value;
        sliderText.text = wind.speed.ToString();

        Message m = new Message();
        m.value = slider.value;
        context.SendJson(m);

        wind.Synchronize();
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage m)
    {
        var message = m.FromJson<Message>();
        slider.value = message.value;
    }
}
