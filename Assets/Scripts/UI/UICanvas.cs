using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


namespace PaintApp
{

    class UICanvas : MonoBehaviour
    {
        [SerializeField] private GameObject MainScreenProxy;
        [SerializeField] private GameObject OnePicturePrefab;
        public List<Animator> AnimatorsSave;
        Manager manager;
        private ScreenType _LastState = ScreenType.main;
        public ScreenType LastState
        {
            get
            {
                return _LastState;
            }
            private set
            {
                _LastState = value;
            }
        }

        public void Initialize()
        {
            manager = Manager.Instanse;
        }
        public void MainSceneInvalidate()
        {
            for (int i = 0; i < MainScreenProxy.transform.childCount; i++)
            {
                Destroy(MainScreenProxy.transform.GetChild(i));
            }
            var allPics = manager.GetAllPreviews();
            foreach (var item in allPics)
            {
                var newPic = Instantiate(OnePicturePrefab, MainScreenProxy.transform);
                Button.ButtonClickedEvent newPicClick = newPic.GetComponent<Button>().onClick;
                newPicClick.AddListener(() => manager.SetChoosedPic(item.Key));
                newPic.GetComponent<Image>().sprite = item.Value;
            }
        }

        public void SwitchScreenTo(ScreenType newScreen)
        {
            print("SwitchScreenTo " + (int)newScreen + " " + newScreen);
            foreach (var item in AnimatorsSave)
            {
                item.SetInteger("state", (int)newScreen);
            }
            LastState = newScreen;
        }
    }
}
