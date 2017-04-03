using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypepadController : MonoBehaviour
{
    public GameObject screen;
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
                _currentCombination = "";
                return;
            }

            _currentCombination += character;
            Debug.Log(_currentCombination);
            CheckCombination();
        }
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
