using UnityEngine;
using UnityEngine.UI;

public class KeyboardTextDisplay : MonoBehaviour
{
    [SerializeReference]
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
        if (textDisplay.IsDestroyed())
        {
            kb.OnStringChanged -= handleStringChanged;

            return;
        }

        textDisplay.text = s + '|';
    }

    // Update is called once per frame
    void Update()
    {

    }
}
