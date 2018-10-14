using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PaintApp
{
    public class Picture : JSON.ISeralizable
    {
        const string STR_Id = "id";
        const string STR_Size_x = "model_size_x";
        const string STR_Size_y = "model_size_y";
        const string STR_Description = "description";
        const string STR_User_params = "user_params";
        const string STR_Is_new = "is_new";
        const string STR_Main_Name = "resource_name";
        const string STR_Preview_Name = "preview_pic_name";
        private const string STR_Name = "name";
        private const string STR_Value = "value";
        public int Id { get; private set; }
        public int Index { get; set; }
        public float SizeX { get; private set; }
        public float SizeY { get; private set; }

        private string MainPictureStr;
        private string PreviewPictureStr;
        public Sprite MainPicture;
        public Sprite PreviewPicture;

        public string Description;
        public bool IsNew = false;
        public Dictionary<string, string> UserParams = new Dictionary<string, string>();
        public Picture(JSON.ANode node,int index)
        {
            Index = index;
            JsonReadFrom(node);
        }
        public void JsonReadFrom(JSON.ANode node, string outerName = "")
        {
            Id = node[STR_Id].AsInt;
            SizeX = node[STR_Size_x].AsFloat;
            SizeY = node[STR_Size_y].AsFloat;
            JSON.TryReadString(node, ref Description, STR_Description);
            JSON.TryReadString(node, ref MainPictureStr, STR_Main_Name);
            JSON.TryReadString(node, ref PreviewPictureStr, STR_Preview_Name);
            JSON.TryReadBool(node, ref IsNew, STR_Is_new);
            JSON.Class userParNode = node[STR_User_params].AsClass;
            var properties = userParNode.GetNamesOfAllChildren();
            foreach (string item in properties)
            {
                UserParams.Add(Config.Localization[item], userParNode[item]);
            }

            MainPicture = Resources.Load<Sprite>(MainPictureStr);
            if (MainPicture == null) 
            {
                Debug.Log("No main image " + MainPictureStr);
            }
            PreviewPicture = Resources.Load<Sprite>(PreviewPictureStr);
        }

        public void JsonWriteTo(JSON.ANode parent, string forceName = "")
        {
            throw new NotImplementedException();
        }
    }
}
