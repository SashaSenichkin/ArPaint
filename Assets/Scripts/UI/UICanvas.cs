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
        //public List<Animator> AnimatorsSave;

        [SerializeField]
        private List<Mover> Movers;

        Manager manager;

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
        public void SwitchDialog(ScreenType dialogType)
        {
            Mover dialog = GetScreenByType(dialogType);
            Translition.Instance.StartSwitchDialog(dialog, !dialog.gameObject.activeSelf);
        }
        public void SwitchScreenTo(ScreenType oldScreen, ScreenType newScreen)
        {
            if (newScreen == ScreenType.main)
            {
                Translition.Instance.StartSwitchDialog(GetScreenByType(ScreenType.bottom), false);
            }
            else if (newScreen == ScreenType.ARScene)
            {
                Translition.Instance.StartSwitchDialog(GetScreenByType(ScreenType.header), false);
            }
            else if (newScreen == ScreenType.detail && oldScreen == ScreenType.main)
            {
                Translition.Instance.StartSwitchDialog(GetScreenByType(ScreenType.bottom), true);
            }
            else if ((newScreen == ScreenType.contact || newScreen == ScreenType.detail) && oldScreen == ScreenType.ARScene)
            {
                Translition.Instance.StartSwitchDialog(GetScreenByType(ScreenType.header), true);
            }
            Translition.Instance.StartReplaceScreens(GetScreenByType(oldScreen), GetScreenByType(newScreen));
        }
        private Mover GetScreenByType(ScreenType type)
        {
            Mover result = Movers.FirstOrDefault(x => x.MyType == type);
            if (result==null)
            {
                throw new Exception("GetScreenByType unknownType " + type);
            }
            return result;
        }
    }
}
