using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatSystem : MonoBehaviour, IKeyboardTarget
{
    public delegate void CombatCompleteEvent(GridEntity winner, GridEntity loser);

    public event CombatCompleteEvent OnCombatComplete;

    public TextAsset wordsFile;

    // TODO WT: Move this into CombatDisplay.
    public Text currentWordText;

    private string[] wordsList;
    private string currentWord;

    public GridEntity player;
    private GridEntity enemy;

    public AudioClip killSound;
    public AudioClip damageSound;

    public CombatDisplay display;

    void Start()
    {
        OnCombatComplete += handleCombatComplete;

        wordsList = wordsFile.text.Split();
    }

    // TODO WT: Support multiple enemys and round robin combat
    public void StartCombat(GridEntity enemy)
    {
        this.enemy = enemy;

        NewRandomWord();

        currentWordText.gameObject.SetActive(true);

        display.Show(enemy);
    }

    private void NewRandomWord()
    {
        currentWord = wordsList[Random.Range(0, wordsList.Length)];
        currentWordText.text = currentWord;
    }

    public void Backspace() { }

    public void KeyPressed(char key) { }

    public void StringChanged(string value) { }

    public void StringSubmitted(string value)
    {
        if (value == currentWord)
        {
            NewRandomWord();

            PerformDamage(player, enemy);
        } else
        {
            PerformDamage(enemy, player);
        }
    }

    private void PerformDamage(GridEntity attacker, GridEntity defender)
    {
        AudioClip clip;

        defender.Hitpoints--;
        if (defender.Hitpoints == 0)
        {
            display.Hide();

            OnCombatComplete?.Invoke(attacker, defender);

            clip = killSound;
        }
        else
        {
            clip = damageSound;
        }

        AudioSource.PlayClipAtPoint(clip, Vector3.zero);
    }

    private void handleCombatComplete(GridEntity winner, GridEntity loser)
    {
        currentWordText.gameObject.SetActive(false);
    }
}
