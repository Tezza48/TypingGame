using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordChecker : MonoBehaviour
{
    public delegate void SubmitEvent(bool wasCorrect);

    public event SubmitEvent OnWordSubmitted;

    public TextAsset wordsFile;
    public Keyboard kb;

    public Text currentWordText;

    private string[] wordsList;
    private string currentWord;

    // Start is called before the first frame update
    void Start()
    {
        wordsList = wordsFile.text.Split();

        NewRandomWord();

        kb.OnStringSubmitted += handleStringSubmitted;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void NewRandomWord()
    {
        currentWord = wordsList[Random.Range(0, wordsList.Length)];
        currentWordText.text = currentWord;
    }

    public void CorrectWord()
    {
        NewRandomWord();

        OnWordSubmitted?.Invoke(true);
    }

    public void IncorrectWord()
    {
        OnWordSubmitted?.Invoke(false);
    }

    private void OnEnable()
    {
        currentWordText.gameObject.SetActive(true);

        if (wordsList != null)
        {
            NewRandomWord();
        }
    }

    private void OnDisable()
    {
        currentWordText.gameObject.SetActive(false);
    }

    private void handleStringSubmitted(string s)
    {
        if (s == currentWord)
        {
            CorrectWord();
        } else
        {

        }
    }
}
