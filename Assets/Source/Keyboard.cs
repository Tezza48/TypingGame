using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KeyboardInput = UnityEngine.InputSystem.Keyboard;

// TODO WT: feedback for when typing mode has started

public class Keyboard : MonoBehaviour
{
    public delegate void CharEvent(char c);
    public delegate void StringEvent(string s);

    public delegate void BackspaceEvent();

    public event CharEvent OnKeyPressed;
    public event StringEvent OnStringSubmitted;
    public event StringEvent OnStringChanged;

    public event BackspaceEvent OnBackspace;

    public IKeyboardTarget target;

    private string currentString;

    public bool modeLocked;
    public bool isTypingMode;

    // Should be a world object, not move the player directly.
    public World grid;

    // Start is called before the first frame update
    void Start()
    {
        modeLocked = false;

        currentString = "";

        KeyboardInput.current.onTextInput += handleKeyboardKeyPressed;

        OnKeyPressed += handleKeyPressed;
        OnStringChanged += handleStringChanged;
        OnStringSubmitted += handleSubmit;
        OnBackspace += handleBackspace;
    }

    // Update is called once per frame
    void Update()
    {
        var current = KeyboardInput.current;

        if (current.tabKey.wasReleasedThisFrame && !modeLocked)
        {
            isTypingMode = !isTypingMode;
        }

        if (current.backspaceKey.wasPressedThisFrame)
        {
            OnBackspace?.Invoke();
        }

        if (current.enterKey.wasPressedThisFrame)
        {
            OnStringSubmitted?.Invoke(currentString);
        }
    }

    private void handleKeyboardKeyPressed(char c)
    {
        if (isTypingMode)
        {
            OnKeyPressed.Invoke(c);
        } else
        {
            // Handle Moving
            switch (c)
            {
                case 'w':
                    grid.MovePlayer(Vector2Int.up);
                    break;
                case 'a':
                    grid.MovePlayer(Vector2Int.left);
                    break;
                case 's':
                    grid.MovePlayer(Vector2Int.down);
                    break;
                case 'd':
                    grid.MovePlayer(Vector2Int.right);
                    break;
                case ' ':
                    grid.TickEntities();
                    break;
                default:
                    break;
            }
        }
    }

    private void handleKeyPressed(char c)
    {
        if (char.IsWhiteSpace(c) && c != ' ')
        {
            return;
        }

        currentString += c;

        OnStringChanged?.Invoke(currentString);

        target?.KeyPressed(c);
    }

    private void handleStringChanged(string value)
    {
        target?.StringChanged(value);
    }

    private void handleSubmit(string value)
    {
        currentString = "";

        OnStringChanged?.Invoke(currentString);

        target?.StringSubmitted(value);
    }

    private void handleBackspace()
    {
        if (currentString.Length == 0)
        {
            return;
        }

        if (currentString.Length == 1)
        {
            currentString = "";
        } else
        {
            currentString = currentString.Substring(0, currentString.Length - 2);
        }
        
        OnStringChanged?.Invoke(currentString);

        target?.Backspace();
    }
}
