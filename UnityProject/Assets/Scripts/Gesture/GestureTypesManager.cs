using HoloToolkit.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gesture
{
    public class GestureTypesManager : Singleton<GestureTypesManager>
    {
        public string CurrentSelectedGestureType;

        private void Start()
        {
            UIManager.Instance.OnGestureTypeSelected += Instance_OnGestureTypeSelected;
        }

        private void Instance_OnGestureTypeSelected(string obj)
        {
            CurrentSelectedGestureType = obj;
        }

        const string GestureTypesKey = "GestureTypesKey";
        private List<string> GestureTypes = new List<string>();
        public void StoreTypes()
        {
            string serialized = JsonConvert.SerializeObject(GestureTypes);

            PlayerPrefs.SetString(GestureTypesKey, serialized);
        }

        public List<string> LoadTypes()
        {
            string storedGestures = PlayerPrefs.GetString(GestureTypesKey);
            if (!string.IsNullOrEmpty(storedGestures))
            {
                this.GestureTypes = JsonConvert.DeserializeObject<List<string>>(storedGestures);
            }

            return this.GestureTypes;
        }
        public List<string> AppendGestureType(string gestureName)
        {
            if (GestureTypes.Contains(gestureName))
                throw new Exception(string.Format("GestureTypes already contains value : {0}", gestureName));

            GestureTypes.Add(gestureName);
            StoreTypes();

            return this.GestureTypes;
        }
        public List<string> DeleteGestureType(string gestureName)
        {
            if (!GestureTypes.Contains(gestureName))
                throw new Exception(string.Format("GestureTypes doesn't contain value : {0}", gestureName));

            GestureTypes.Remove(gestureName);
            return this.GestureTypes;
        }
    }
}
