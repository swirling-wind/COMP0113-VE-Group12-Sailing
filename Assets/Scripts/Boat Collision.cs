using Org.BouncyCastle.Tls.Crypto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatCollision : MonoBehaviour
{
    public GameObject failPanel;
    private BoatMovement script;

    private AudioSource crashAudioSource;
    private AudioSource loseAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        script = GetComponent<BoatMovement>();
        loseAudioSource = GetComponents<AudioSource>()[0];
        crashAudioSource = GetComponents<AudioSource>()[1];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Terrain") || other.CompareTag("rock"))
        {
            crashAudioSource.Play();
            loseAudioSource.Play();

            script.Collision();
            failPanel.SetActive(true);  
        }
    }
}
