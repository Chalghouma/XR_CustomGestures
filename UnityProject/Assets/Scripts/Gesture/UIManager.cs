using Assets.Scripts.Drawing;
using Assets.Scripts.UX;
using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Button = HoloToolkit.Unity.Buttons.Button;
namespace Assets.Scripts.Gesture
{
    public class UIManager : Singleton<UIManager>
    {
        private GridDrawer m_gridDrawer;
        private void Start()
        {
            m_gridDrawer = FindObjectOfType<GridDrawer>();

            var types = GestureTypesManager.Instance.LoadTypes();
            SetGestureTypesOptions(types);

            m_currentGesturesDropdown.onValueChanged.AddListener(val =>
            {
                if (OnGestureTypeSelected != null)
                    OnGestureTypeSelected(m_currentGesturesDropdown.options[val].text);
            });

            Destroy(m_recordingEnablingToggle.gameObject);
            //m_recordingEnablingToggle.OnSelectEvents.AddListener(() => { ToggleRecording(); });

            m_addGestureType.OnButtonClicked += M_addGestureType_OnButtonClicked;
            m_removeGestureType.OnButtonClicked += M_removeGestureType_OnButtonClicked;

            var datasets = DatasetManager.Instance.LoadDatasets();
            DisplayDatasets(datasets);

            m_addDataset.OnButtonClicked += M_addDataset_OnButtonClicked;
            m_removeDataset.OnButtonClicked += M_removeDataset_OnButtonClicked;

            m_datasetsDropdown.onValueChanged.AddListener(val =>
            {
                if (OnDatasetSelected != null)
                    OnDatasetSelected(m_datasetsDropdown.options[val].text);
            });

            m_cleanScreen.OnButtonClicked += M_cleanScreen_OnButtonClicked;
            m_saveImage.OnButtonClicked += M_saveImage_OnButtonClicked;

            OnGestureTypeSelected += UIManager_OnGestureTypeSelected;
            OnDatasetSelected += UIManager_OnDatasetSelected;

            m_currentGestureTypeName = types.Count != 0 ? types[0] : null;
            m_currentDatasetName = datasets.Count != 0 ? datasets[0] : null;

            CustomGestureRecognizer.Instance.OnGestureRecognized += Instance_OnGestureRecognized;


            m_uploadButton.OnButtonClicked += (gO) => UploadImages();
        }
        #region OnGestureRecognized Related
        [SerializeField]
        TextMesh _recognitionResult;
        private void Instance_OnGestureRecognized(GestureRecognizedData obj)
        {
            _recognitionResult.text = obj.ToString();
        }
        #endregion

        string m_currentDatasetName, m_currentGestureTypeName;
        private void UIManager_OnDatasetSelected(string obj)
        {
            m_currentDatasetName = obj;
        }

        private void UIManager_OnGestureTypeSelected(string obj)
        {
            m_currentGestureTypeName = obj;
        }

        private void M_removeGestureType_OnButtonClicked(GameObject obj)
        {
            DeleteCurrentGestureType();
        }

        private void M_addGestureType_OnButtonClicked(GameObject obj)
        {
            AppendGestureType();
        }

        #region GestureTypes related
        public event Action<string> OnGestureTypeSelected;
        [SerializeField]
        InputField m_gestureTypeNameInputField;
        [SerializeField]
        Dropdown m_currentGesturesDropdown;
        [SerializeField]
        Button m_addGestureType, m_removeGestureType;
        public void AppendGestureType()
        {
            var gestureTypes = GestureTypesManager.Instance.AppendGestureType(m_gestureTypeNameInputField.text);
            SetGestureTypesOptions(gestureTypes);

            if (string.IsNullOrEmpty(m_currentGestureTypeName))
                m_currentGestureTypeName = gestureTypes[0];
        }
        public void DeleteCurrentGestureType()
        {
            int nextValue = m_currentGesturesDropdown.value == m_currentGesturesDropdown.options.Count - 1 ?
                m_currentGesturesDropdown.options.Count - 2 : m_currentGesturesDropdown.value;
            string toBeDeleted = m_currentGesturesDropdown.options[m_currentGesturesDropdown.value].text;

            m_currentGesturesDropdown.ClearOptions();

            List<string> list = GestureTypesManager.Instance.DeleteGestureType(toBeDeleted);
            SetGestureTypesOptions(list);

            m_currentGesturesDropdown.value = nextValue;
        }

        private void SetGestureTypesOptions(List<string> list)
        {
            int selectedValue = m_currentGesturesDropdown.value;
            SetDropdownList(m_currentGesturesDropdown, list);
            m_currentGesturesDropdown.value = selectedValue;
        }

        private void SetDropdownList(Dropdown targetDropdown, List<string> list)
        {
            targetDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (var item in list)
            {
                options.Add(new Dropdown.OptionData { text = item });
            }

            targetDropdown.AddOptions(options);
        }

        #endregion
        #region Datasets related
        public event Action<string> OnDatasetSelected;
        [SerializeField]
        Button m_addDataset, m_removeDataset;
        [SerializeField]
        InputField m_datasetInputField;
        [SerializeField]
        Dropdown m_datasetsDropdown;

        private void M_removeDataset_OnButtonClicked(GameObject obj)
        {
            int nextValue = m_datasetsDropdown.value == m_datasetsDropdown.options.Count - 1 ?
                m_datasetsDropdown.options.Count - 2 : m_datasetsDropdown.value;
            string toBeDeleted = m_datasetsDropdown.options[m_datasetsDropdown.value].text;

            m_datasetsDropdown.ClearOptions();

            List<string> list = DatasetManager.Instance.DeleteDataset(toBeDeleted);
            SetGestureTypesOptions(list);

            m_datasetsDropdown.value = nextValue;
        }

        private void M_addDataset_OnButtonClicked(GameObject obj)
        {
            List<string> list = DatasetManager.Instance.CreateDataset(m_datasetInputField.text);
            DisplayDatasets(list);

            if (string.IsNullOrEmpty(m_currentDatasetName))
                m_currentDatasetName = list[0];
        }
        void DisplayDatasets(List<string> list)
        {
            int selectedValue = m_datasetsDropdown.value;
            SetDropdownList(m_datasetsDropdown, list);
            m_datasetsDropdown.value = selectedValue;
        }

        #endregion
        #region GestureRecording related
        [SerializeField]
        Button m_cleanScreen, m_saveImage;
        private void M_saveImage_OnButtonClicked(GameObject obj)
        {
            if (string.IsNullOrEmpty(m_currentDatasetName))
                m_currentDatasetName = "DS";
            if (string.IsNullOrEmpty(m_currentGestureTypeName))
                m_currentGestureTypeName = "L";

            if (string.IsNullOrEmpty(m_currentDatasetName) || string.IsNullOrEmpty(m_currentGestureTypeName))
                throw new Exception("Please select/create a Dataset and a GetureType");

            string datasetFolder = Path.Combine(DatasetManager.Instance.RootDatasetsFolder, m_currentDatasetName);
            if (!Directory.Exists(datasetFolder))
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(datasetFolder);
                    Debug.LogFormat("DatasetFolder Correctly Created : {0} | Exists : {1}", info.FullName, info.Exists);
                }
                catch (Exception exp)
                {
                    Debug.LogFormat("Exception while Creating Directory : {0}. Exception is : {1}", datasetFolder, exp);
                }


            string gestureTypeFolder = Path.Combine(
                datasetFolder,
                m_currentGestureTypeName);
            if (!Directory.Exists(gestureTypeFolder))
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(gestureTypeFolder);
                    Debug.LogFormat("DatasetFolder Correctly Created : {0} | Exists : {1}", info.FullName, info.Exists);
                }
                catch (Exception exp)
                {
                    Debug.LogFormat("Exception while Creating Directory : {0}. Exception is : {1}", gestureTypeFolder, exp);
                }

            int count = 0;
            while (File.Exists(Path.Combine(gestureTypeFolder, string.Format("{0}", count))))
            {
                count++;
            }

            GridDrawer.Instance.SaveGridAsImage(
                Path.Combine(gestureTypeFolder, string.Format("{0}", count))
                .Replace("\\\\", "\\").Replace("\\", "/"));
        }

        private void M_cleanScreen_OnButtonClicked(GameObject obj)
        {
            GridDrawer.Instance.CleanBlackColor();
        }

        [SerializeField]
        InteractiveToggle m_recordingEnablingToggle;

        public void ToggleRecording()
        {
            CustomTrainingGestureRecorder.Instance.IsRecordingAllowed = !CustomTrainingGestureRecorder.Instance.IsRecordingAllowed;
            CustomGestureRecognizer.Instance.gameObject.SetActive(!CustomTrainingGestureRecorder.Instance.IsRecordingAllowed);
        }


        #endregion
        #region Upload-Dataset related
        [SerializeField]
        TextMesh m_uploadLog;
        [SerializeField]
        Button m_uploadButton;
        public void UploadImages()
        {
            if (string.IsNullOrEmpty(m_currentDatasetName) || string.IsNullOrEmpty(m_currentGestureTypeName))
                throw new Exception("Please select/create a Dataset and a GetureType");

            string datasetRootPath = Path.Combine(DatasetManager.Instance.RootDatasetsFolder, m_currentDatasetName);
            var gesturesTypesFolders = Directory.GetDirectories(datasetRootPath);

            foreach (var gestureTypeFolder in gesturesTypesFolders)
            {
                var existingFiles = Directory.GetFiles(gestureTypeFolder);
                StartCoroutine(UploadFilesAsync(existingFiles));
            }
        }

        public IEnumerator UploadFilesAsync(string[] imageFiles)
        {
            foreach (var imageFile in imageFiles)
            {
                byte[] data = File.ReadAllBytes(imageFile);

                var form = new WWWForm();
                form.AddBinaryData("binarydata", data);
                form.headers["Content-Type"] = "application/octet-stream";

                var www = UnityWebRequest.Post(string.Format("localhost:6969/store_image?dataset={0}&gesturetype={1}&imagename={2}",
                    m_currentDatasetName, m_currentGestureTypeName, Path.GetFileNameWithoutExtension(imageFile)), form);
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error))
                {
                    string err = string.Format("STORING IMAGE ERROR : {0}", www.error);
                    Debug.LogFormat(err);
                    m_uploadLog.text = err;
                }
                else
                {
                    var text = www.downloadHandler.text;
                    string result = string.Format("STORING IMAGE RESULT : {0}", text);
                    Debug.LogFormat(result);
                    m_uploadLog.text = result;
                }
            }
        }
        #endregion
    }
}
