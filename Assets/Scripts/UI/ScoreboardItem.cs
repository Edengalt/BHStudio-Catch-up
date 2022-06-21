using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI scoreCounter;
    
    private int score;
    
    public string ID =>  name.text;
    public int Score => score;
    
    public void Setup(string name)
    {
        this.name.text = name;
        scoreCounter.text = 0.ToString();
    }

    public void Highlight()
    {
        name.color = Color.green;
        scoreCounter.color = Color.green;
    }

    public void SetScore(int _score)
    {
        
        score = _score;
        scoreCounter.text = score.ToString();
    }

    public void Rollout()
    {
        Destroy(gameObject);
    }
}
