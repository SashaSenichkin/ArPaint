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
        ARscene = 3,
        contact = 4
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
            if (History.Count > 1)
            {
                History.RemoveAt(History.Count - 1);
                SwitchScreenWrap(History.Last());
                History.RemoveAt(History.Count - 1);
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
            SwitchScreenWrap(ScreenType.main);
            UIDetContr.OnDoubleClick += ()=> SwitchScreenWrap(ScreenType.ARscene);
            Debug.LogError("stop here");
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
            History.Add(screen);
            UIBot.SwitchScreenTo(screen);
            UIcontr.SwitchScreenTo(screen);
        }
        public void OpenContact()
        {
            SwitchScreenWrap(ScreenType.contact);
        }
        public void OpenAR()
        {
            SwitchScreenWrap(ScreenType.ARscene);
        }
        public void OpenMain()
        {
            SwitchScreenWrap(ScreenType.main);
        }
        public void SendContactEmail(string email, string phone)
        {
            Debug.Log("message send");
        }
    }
}
