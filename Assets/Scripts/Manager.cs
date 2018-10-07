using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace PaintApp
{
    [Serializable]
    enum ScreenType
    {
        main = 1,
        detail = 2,
        ARScene = 3,
        contact = 4,
        header,
        bottom,
        paramDialog,
        ARHelpDialog
    }
    class Manager : MonoBehaviour
    {
        ScreenType curOpenScreen = ScreenType.main;
        public static Manager Instanse { get; private set; }
        public UICanvas UIcontr;
        public UIDetail UIDetContr;
        public UIBottom UIBot;
        public TextAsset ConfigJson;
        public SpriteRenderer PicModel;

        private List<ScreenType> History;
        Config mainConf;
        int curPic;
        public void BackButton()
        {
            int lastIndex = History.Count-1;
            if (lastIndex > 0)
            {
                UIcontr.SwitchScreenTo(History[lastIndex], History[lastIndex-1]);
                print("BackButton SwitchScreenTo location " + History[lastIndex] +" to " + History[lastIndex - 1]);
                UIBot.SwitchScreenTo(History[lastIndex -1]);
                History.RemoveAt(lastIndex);
            }
            else
            {
                print("last history location");
            }
        }

     
        private void Awake()
        {
            History = new List<ScreenType>();
            Instanse = this;
            mainConf = new Config(JSON.Parse(ConfigJson.text));
            UIcontr.Initialize();
            UIcontr.MainSceneInvalidate();
            History.Add(ScreenType.main);
            UIDetContr.OnDoubleClick += ()=> SwitchScreenWrap(ScreenType.ARScene);
        }
        public Dictionary<string, string> GetPropForPic(int id)
        {
           
            var result = new Dictionary<string, string>();
            var properties = mainConf.AllProducts[id].UserParams;
            foreach (var item in properties)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
        public Dictionary<int, Sprite> GetAllPreviews()
        {
            var result = new Dictionary<int, Sprite>();

            foreach (var item in mainConf.AllProducts)
            {
                result.Add(item.Key, item.Value.PreviewPicture);
            }
            return result;
        }
        public void SetChoosedPic(int id)
        {
            curPic = id;
            var picture = mainConf.AllProducts[id];
            UIDetContr.DetailInvalidate(picture.MainPicture, picture.Description, GetPropForPic(id));
            SwitchScreenWrap(ScreenType.detail);            
            PicModel.sprite = picture.MainPicture;
        }
        public void SwitchScreenWrap(ScreenType screen)
        {
            if (screen == History.Last())
            {
                return;
            }
            //UIBot.SwitchScreenTo(screen);
            UIcontr.SwitchScreenTo(History.Last(), screen);
            History.Add(screen);
        }
        public void OpenContact()
        {
            SwitchScreenWrap(ScreenType.contact);
        }
        public void OpenAR()
        {
            SwitchScreenWrap(ScreenType.ARScene);
        }
        public void OpenMain()
        {
            SwitchScreenWrap(ScreenType.main);
        }
        public void InfoDialogShowHide()
        {
            UIcontr.SwitchDialog (ScreenType.paramDialog);
        }
        public void ArHelperDialogShowHide()
        {
            UIcontr.SwitchDialog(ScreenType.ARHelpDialog);
        }
        public void SendContactEmail(string email, string phone)
        {
            Debug.Log("message send");
        }
    }
}
