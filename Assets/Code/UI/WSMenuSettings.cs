using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSMenuSettings : MonoBehaviour
{
    public enum WSMenuOption
    {
        CAPTIONS,
        VOLUME,
        //LANGUAGE
    };

    int _currentLabel = 0;
    int[] _currentOption = new int[] {0,1,0};
    int[][] _currentOptionChoice = { new int[] {0,1},
                            new int[] {0,1,2},
                            new int[] {0,1}
                          };
    Color32 LabelHighlightColor = new Color32(255,251,0,255);
    Color32 LabelUnhighlightColor = new Color32(121,141,123,255);

    Color32 OptionHighlightColor = new Color32(232, 239, 210, 255);
    Color32 OptionUnhighlightColor = new Color32(109, 128, 111, 255);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("MoveDown")]
    public void MoveMenuDown()
    {
        GameObject labelParent = transform.GetChild(0).gameObject;
        if(labelParent.transform.childCount-1 > _currentLabel+1)        //take off -1 once language option available.
        {
            labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelUnhighlightColor;
            _currentLabel+=1;
            labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelHighlightColor;
        }

        Debug.Log(_currentLabel);
    }

    [ContextMenu("MoveUp")]
    public void MoveMenuUp()
    {
        GameObject labelParent = transform.GetChild(0).gameObject;
        if(_currentLabel - 1 >= 0 )
        {
            labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelUnhighlightColor;
            _currentLabel -= 1;
            labelParent.transform.GetChild(_currentLabel).GetComponent<TMPro.TextMeshPro>().color = LabelHighlightColor;
        }

        Debug.Log(_currentLabel);
    }

    [ContextMenu("MoveRight")]
    public void MoveOptionRight()
    {
        GameObject optionParent = transform.GetChild(1).gameObject;
        if(_currentOption[_currentLabel] + 1 < _currentOptionChoice[_currentLabel].Length)
        {
            optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionUnhighlightColor;
            _currentOption[_currentLabel] +=1;
            optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionHighlightColor;
        }

        Debug.Log(_currentLabel);
        Debug.Log(_currentOption[_currentLabel]);
    }

    [ContextMenu("MoveLeft")]
    public void MoveOptionLeft()
    {
        GameObject optionParent = transform.GetChild(1).gameObject;
        if(_currentOption[_currentLabel] - 1 >= 0)
        {
            optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionUnhighlightColor;
            _currentOption[_currentLabel] -=1;
            optionParent.transform.GetChild(_currentLabel).GetChild(_currentOptionChoice[_currentLabel][_currentOption[_currentLabel] ]).GetComponent<MeshRenderer>().material.color = OptionHighlightColor;
        }

        Debug.Log(_currentLabel);
        Debug.Log(_currentOption[_currentLabel]);
    }
}
