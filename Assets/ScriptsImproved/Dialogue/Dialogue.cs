using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Dialogue
{
    public string name;

    // min and max size of the text box     
    [TextArea(3, 7)]
    public string[] sentences;

}
