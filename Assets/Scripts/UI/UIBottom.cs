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
        [SerializeField] private RectTransform BtnBack;
        [SerializeField] private RectTransform BtnInfo;
        [SerializeField] private RectTransform BtnView;
        [SerializeField] private RectTransform BtnRestart;
        [SerializeField] private RectTransform BtnContact;
        [SerializeField, Range(0,1)] private float AncorMargin = 0.15f;
        public void SwitchScreenTo(ScreenType newScreen)
        {
            switch (newScreen)
            {
                case ScreenType.detail:
                    ShowButtons(BtnContact, BtnInfo, BtnView);
                    HideButtons(BtnRestart);
                    break;
                case ScreenType.ARScene:
                    ShowButtons(BtnContact, BtnRestart);
                    HideButtons(BtnInfo, BtnView);
                    break;
                case ScreenType.contact:
                    HideButtons(BtnRestart, BtnInfo, BtnView, BtnContact);
                    break;
                case ScreenType.main:
                    break;
                default:
                    Debug.Log("UIBottom SwitchScreenTo unknown type " + newScreen);
                    break;
            }
        }
        private void HideButtons(params RectTransform[] butArr)
        {
            for (int i = 0; i < butArr.Length; i++)
            {
                butArr[i].gameObject.SetActive(false);
            }
        }
        private void ShowButtons(params RectTransform[] butArr)
        {
            SetAncors(BtnBack, AncorMargin);
            for (int i = 0; i < butArr.Length; i++)
            {
                float newAncor = i == 0? 1 - AncorMargin: AncorMargin + ((1 - 2 * AncorMargin) * i) / butArr.Length;
                SetAncors(butArr[i], newAncor);
                butArr[i].gameObject.SetActive(true);
            }

        }
        private void SetAncors(RectTransform client, float newVal)
        {
            client.anchorMax = new Vector2(newVal, client.anchorMax.y);
            client.anchorMin = new Vector2(newVal, client.anchorMin.y);
        }
    }
}
