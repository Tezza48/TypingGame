using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKeyboardTarget
{
    void KeyPressed(char key);
    void StringChanged(string value);
    void StringSubmitted(string value);
    void Backspace();
}
