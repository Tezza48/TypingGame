using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoDisplay : MonoBehaviour
{
    public GridEntity player;
    public Text hitpointsDisplay;

    // Start is called before the first frame update
    void Start()
    {
        player.OnHitpointsChanged += handlePlayerHitpoints;
    }

    private void handlePlayerHitpoints(uint hitpoints)
    {
        hitpointsDisplay.text = "HP: " + hitpoints.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
