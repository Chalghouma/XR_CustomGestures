using HoloToolkit.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gesture
{
    public class DatasetManager : Singleton<DatasetManager>
    {
        const string DataSetsKey = "ExistingDatasets";

        List<string> m_existingDatasets = new List<string>();
        string RootDatasetsFolder { get { return Path.Combine(Application.persistentDataPath, "Datasets"); } }
        public List<string> CreateDataset(string datasetName)
        {
            if (!Directory.Exists(RootDatasetsFolder))
            {
                Directory.CreateDirectory(RootDatasetsFolder);
            }

            string dataSetFolder = Path.Combine(RootDatasetsFolder, datasetName);
            if (!Directory.Exists(dataSetFolder))
            {
                Directory.CreateDirectory(dataSetFolder);
            }

            m_existingDatasets.Add(datasetName);
            StoreDatasets();

            return m_existingDatasets;
        }

        private void StoreDatasets()
        {
            PlayerPrefs.SetString(DataSetsKey, JsonConvert.SerializeObject(m_existingDatasets));
        }
        public List<string> DeleteDataset(string datasetName)
        {
            m_existingDatasets.Remove(datasetName);

            Directory.Delete(Path.Combine(RootDatasetsFolder, datasetName), true);

            return m_existingDatasets;
        }
        public List<string> LoadDatasets()
        {
            if (PlayerPrefs.HasKey(DataSetsKey))
            {
                m_existingDatasets = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString(DataSetsKey));
            }

            return m_existingDatasets;
        }
    }
}
