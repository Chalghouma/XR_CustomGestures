using Assets.Scripts.Drawing;
using Assets.Scripts.UX;
using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
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
        }


        private void M_removeGestureType_OnButtonClicked(GameObject obj)
        {
            DeleteCurrentGestureType();
        }

        private void M_addGestureType_OnButtonClicked(GameObject obj)
        {
            AppendGestureType();
        }

        #region Recording Trainig Data related
        [SerializeField]
        InteractiveToggle m_recordingEnablingToggle;

        public void ToggleRecording()
        {
            CustomTrainingGestureRecorder.Instance.IsRecordingAllowed = !CustomTrainingGestureRecorder.Instance.IsRecordingAllowed;
        }

        public void CleanGrid()
        {
            m_gridDrawer.CleanBlackColor();
        }
        #endregion
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
        }
        void DisplayDatasets(List<string> list)
        {
            int selectedValue = m_datasetsDropdown.value;
            SetDropdownList(m_datasetsDropdown, list);
            m_datasetsDropdown.value = selectedValue;
        }

        #endregion
    }
}
