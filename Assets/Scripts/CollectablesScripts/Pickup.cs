using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pickup : MonoBehaviour
{
    public int Value = 1;

    private ScoreManager _scoreManager;

    private Animator _pickupAC;

    private Text _pickupValueText;

    private void Start()
    {
        _scoreManager = GameManager.instance.Score;

        _pickupAC = GetComponent<Animator>();

        _pickupValueText = GetComponentInChildren<Text>();
        _pickupValueText.text = Value.ToString();
    }

    public void DestroyPickup()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _scoreManager.AddToTotalScore(Value);
            _pickupAC.SetTrigger("PickedUp");
        }
    }
}
