using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Keyboard keyboard;

    public WorldSystem worldSystem;
    public CombatSystem combatSystem;

    public KeyboardTextDisplay keyboardDisplay;

    private static StateMessage BASE_STATE = new StateMessage();
    private static CombatState COMBAT_STATE = new CombatState();

    // Deffered state which is entered in the subsequent Update.
    private StateMessage nextState;

    // Start is called before the first frame update
    void Start()
    {
        nextState = BASE_STATE.Init(State.World);

        worldSystem.OnCombatStarted += handleCombatStarted;
        combatSystem.OnCombatComplete += handleCombatFinished;
    }

    private void handleCombatFinished(GridEntity winner, GridEntity loser)
    {
        if (winner == worldSystem.player)
        {
            worldSystem.entities.Remove(loser);

            Destroy(loser.gameObject);// Do this next frame
        } else
        {
            // Player lost, start game over.
            Debug.Log("Player lost in combat");
        }

        nextState = BASE_STATE.Init(State.World);
    }

    private void handleCombatStarted(GridEntity enemy)
    {
        nextState = COMBAT_STATE.Init(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        if (nextState != null)
        {
            switch (nextState.state)
            {
                case State.World:
                    keyboard.target = worldSystem;

                    keyboardDisplay.gameObject.SetActive(false);
                    break;
                case State.Combat:
                    keyboard.target = combatSystem;

                    combatSystem.StartCombat(((CombatState)nextState).entity);

                    keyboardDisplay.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }

            keyboard.Clear();

            nextState = null;
        }
    }
}

class StateMessage
{
    public State state;

    public StateMessage Init(State state)
    {
        this.state = state;
        return this;
    }
}

class CombatState: StateMessage
{
    public GridEntity entity;

    public CombatState()
    {
        this.state = State.Combat;
    }

    public CombatState Init(GridEntity entity)
    {
        this.entity = entity;

        return this;
    }
}

enum State
{
    World,
    Combat,
}