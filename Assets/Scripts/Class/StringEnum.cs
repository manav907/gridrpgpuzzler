using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StringEnum
{
    [SerializeField] int selectedIndex = -1;
    string[] options;
    public string this[int i]
    {
        get { return options[selectedIndex]; }
    }
    public StringEnum(string[] options)
    {
        this.options = options;
    }
}
