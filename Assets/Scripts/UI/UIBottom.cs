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
        [SerializeField] private GameObject BtnContact;
        [SerializeField]
        private LayoutGroup BottomLayout;
        public void SwitchScreenTo(ScreenType newScreen)
        {
            switch (newScreen)
            {
                case ScreenType.detail:
                    BtnInfo.SetActive(true);
                    BtnView.SetActive(true);
                    BtnRestart.SetActive(false);
                    break;
                case ScreenType.ARScene:
                    BtnInfo.SetActive(false);
                    BtnView.SetActive(false);
                    BtnRestart.SetActive(true);
                    break;
                case ScreenType.contact:
                    BtnInfo.SetActive(false);
                    BtnView.SetActive(false);
                    BtnRestart.SetActive(false);
                    BtnContact.SetActive(false);
                    break;
                case ScreenType.main:
                    break;
                default:
                    Debug.Log("UIBottom SwitchScreenTo unknown type " + newScreen);
                    break;
            }
        }
    }
}
