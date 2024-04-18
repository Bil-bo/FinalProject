using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Facts : MonoBehaviour
{
    [System.Serializable]
    private class JSONFacts
    {
        public string[] facts;
    }

    [SerializeField]
    private TextAsset JsonF;

    [SerializeField]
    private TextMeshProUGUI FactText;

    private JSONFacts RawFacts = new JSONFacts();
    private List<string> facts;

    // Start is called before the first frame update
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
