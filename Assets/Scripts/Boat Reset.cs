using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatReset : MonoBehaviour
{
    public Transform startPoint;
    public GameObject failPanel;

    private RudderMovement rudderMovement;
    private Winchforsaildirection winchForSailDirection;
    private Sailsizewinch winchForSailSize;
    private BoatMovement script;

    // Start is called before the first frame update
    void Start()
    {
        rudderMovement = FindAnyObjectByType<RudderMovement>();
        winchForSailDirection = FindAnyObjectByType<Winchforsaildirection>();
        winchForSailSize = FindAnyObjectByType<Sailsizewinch>();
        script = GetComponent<BoatMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetBoat()
    {
        rudderMovement.ResetState();
        winchForSailDirection.ResetState();
        winchForSailSize.ResetState();
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;
        failPanel.SetActive(false);
        script.UndoCollision();
    }

}
