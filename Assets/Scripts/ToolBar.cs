using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBar : MonoBehaviour
{
    #region property
    public static ToolBar instance;
    #endregion

    #region serialize
    #endregion

    #region private
    List<GameObject> toolBarList;
    [SerializeField]
    int selectToolIndex = 0;
    Color defaultColor;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods

    private void Awake()
    {
        if (instance = null)
            instance = this;
    }

    private void Start()
    {
        toolBarList = new List<GameObject>();
        for(int childCnt = 0;childCnt<transform.childCount; childCnt++)
        {
            toolBarList.Add(transform.GetChild(childCnt).gameObject);
        }

        defaultColor = toolBarList[0].GetComponent<Image>().color;

        selectToolIndex = 0;
        ChangeToolToLight(selectToolIndex, Color.white);
    }

    private void Update()
    {

    }



    #endregion

    #region public method
    public void SetSelectTo(int scroll)
    {
        if (scroll == 0) return;

        selectToolIndex += scroll;

        if (selectToolIndex < 0)
            selectToolIndex = toolBarList.Count - 1;
        else if (selectToolIndex >= toolBarList.Count)
            selectToolIndex = 0;

        ChangeToolToLight(selectToolIndex, Color.white);
    }
    #endregion

    #region private method
    public void ChangeToolToLight(int num, Color color)
    {
        for (int childCnt = 0; childCnt < transform.childCount; childCnt++)
        {
            if (childCnt == num)
            {
                toolBarList[childCnt].GetComponent<Image>().color = color;
                continue;
            }
            toolBarList[childCnt].GetComponent<Image>().color = defaultColor;
        }
    }
    #endregion
}
