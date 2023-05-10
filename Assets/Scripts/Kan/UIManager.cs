using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region property
    #endregion

    #region serialize
    #endregion

    #region private
    GameObject toolBarObjcet;
    ToolBar toolBar;
    #endregion

    #region Constant
    #endregion

    #region Event
    #endregion

    #region unity methods
    private void Awake()
    {

    }

    private void Start()
    {
        toolBarObjcet = transform.GetChild(0).gameObject;
        toolBar = toolBarObjcet.GetComponent<ToolBar>();
    }

    private void Update()
    {
        int scroll = (int)Input.mouseScrollDelta.y;

        toolBar.SetSelectTo(scroll);
    }
    #endregion

    #region public method
    #endregion

    #region private method
    #endregion
}
