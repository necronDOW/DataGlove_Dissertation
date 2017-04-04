using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypepadController : MonoBehaviour
{
    public GameObject screen;
    public Text output;
    public Material material;
    public string combinationGoal;

    private string _currentCombination;
    private bool complete = false;

    public void Add(char character)
    {
        if (!complete)
        {
            if (character == '*' || character == '#')
            {
                SetCombination("");
                return;
            }

            SetCombination(_currentCombination + character);
            CheckCombination();
        }
    }

    private void SetCombination(string newCombination)
    {
        _currentCombination = newCombination;
        if (output)
            output.text = _currentCombination;
    }

    private void CheckCombination()
    {
        if (_currentCombination == combinationGoal)
        {
            screen.GetComponent<MeshRenderer>().material = material;
            complete = true;
        }
    }
}
