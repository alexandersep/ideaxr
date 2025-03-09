using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsChecker : MonoBehaviour
{
    public float _result;

    private void Update()
    {
        getResult();
    }
    public void getResult()
    {
        int accumulator = 0;
        // Get all Question components in child objects
        List<bool> bools =  GetComponentsInChildren<RadioButtonBehaviour>().Select(q => q._isCorrect).ToList();
        foreach (bool b in bools)
        {
            accumulator += Convert.ToInt16(b);
        }
        _result = (float) accumulator / bools.Count;
    }
}