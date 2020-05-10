using UnityEngine;
using UnityEngine.UI;

public class KeyboardTextDisplay : MonoBehaviour
{
    public Keyboard kb;
    private Text textDisplay;

    // Start is called before the first frame update
    void Start()
    {
        textDisplay = GetComponent<Text>();

        textDisplay.text = "|";
        kb.OnStringChanged += handleStringChanged;
    }

    private void handleStringChanged(string s)
    {
        textDisplay.text = s + '|';
    }

    // Update is called once per frame
    void Update()
    {

    }
}
