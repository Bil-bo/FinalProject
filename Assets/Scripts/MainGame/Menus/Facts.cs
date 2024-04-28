using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Class for encapsulating facts in the UI
public class Facts : MonoBehaviour
{
    // Contained class for serialising JSON data
    [System.Serializable]
    private class JSONFacts
    {
        public string[] facts;
    }
    
    // the data file
    [SerializeField]
    private TextAsset JsonF;

    [SerializeField]
    private TextMeshProUGUI FactText;

    // The instance of the contained class
    private JSONFacts RawFacts = new JSONFacts();
    private List<string> facts;

    void Awake()
    {
        EventManager.AddListener<GameOverEvent>(OnGameOver);

        RawFacts = JsonUtility.FromJson<JSONFacts>(JsonF.text);
        facts = new List<string>(RawFacts.facts);
    }

    private void OnEnable()
    {
        ShowFact();
    }

    // Show a new random fact each time the class is re-enabled
    public void ShowFact()
    {
        System.Random rand = new System.Random();
        FactText.text = facts[rand.Next(facts.Count)];
    }

    
    public void OnGameOver(GameOverEvent eventData)
    {
        ShowFact(); 
    }


}
