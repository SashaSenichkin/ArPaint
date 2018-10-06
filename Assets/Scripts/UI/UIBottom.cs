using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace PaintApp
{

    class UIBottom : MonoBehaviour
    {
        [SerializeField] private GameObject BtnInfo;
        [SerializeField] private GameObject BtnView;
        [SerializeField] private GameObject BtnRestart;
        [SerializeField] private GameObject BtnDetail;

        [SerializeField] private GameObject BtnContact;

        public void SwitchScreenTo(ScreenType newScreen)
        {

            switch (newScreen)
            {
                case ScreenType.detail:
                    BtnInfo.SetActive(true);
                    BtnView.SetActive(true);
                    BtnRestart.SetActive(false);
                    BtnDetail.SetActive(false);
                    break;
                case ScreenType.ARscene:
                    BtnInfo.SetActive(false);
                    BtnView.SetActive(false);
                    BtnRestart.SetActive(true);
                    BtnDetail.SetActive(true);
                    break;
                default:
                    Debug.Log("UIBottom SwitchScreenTo unknown type " + newScreen);
                    break;
            }
        }
    }
}
