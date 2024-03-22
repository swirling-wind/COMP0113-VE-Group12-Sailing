using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindDirectionDisplay : MonoBehaviour
{
    public TextMeshProUGUI directionText;
    public Wind wind;

    // Start is called before the first frame update
    void Start()
    {

        wind = GameObject.Find("Wind").GetComponent<Wind>();
    }

    // Update is called once per frame
    void Update()
    {   
        float angle = wind.transform.eulerAngles.y;
        string direction = AngleToDirection(angle);
        directionText.text = direction;
    }

    string AngleToDirection(float angle)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
        int index = Mathf.RoundToInt(angle / 45); // Divide by 45 degrees to get index
        if (index < 0) index += 8; // Ensure index is positive
        index = index % directions.Length; // Modulo to wrap around the directions array
        return angle.ToString("0") + "° " + directions[index]; // Return angle and direction string
    }

}
