using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDisplay : MonoBehaviour
{
    public Text hitpointsDisplay;

    private GridEntity entity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(GridEntity entity)
    {
        gameObject.SetActive(true);

        handleEntityHitpoints(entity.Hitpoints);

        entity.OnHitpointsChanged += handleEntityHitpoints;
        this.entity = entity;
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        entity.OnHitpointsChanged -= handleEntityHitpoints;
        entity = null;
    }

    private void handleEntityHitpoints(uint hitpoints)
    {
        hitpointsDisplay.text = "HP: " + hitpoints.ToString();
    }
}
