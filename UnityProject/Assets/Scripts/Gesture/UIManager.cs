using Assets.Scripts.Drawing;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gesture
{
    public class UIManager : Singleton<UIManager>
    {
        private GridDrawer m_gridDrawer;
        private void Start()
        {
            m_gridDrawer = FindObjectOfType<GridDrawer>();

            m_currentGesturesDropdown.onValueChanged.AddListener((val) =>
            {
                if (OnGestureTypeSelected != null)
                    OnGestureTypeSelected(m_currentGesturesDropdown.options[val].text);
            });

            m_recordingEnablingButton.onClick.AddListener(() =>
            {
                ToggleRecording();
            });
        }
        #region Recording Trainig Data related
        [SerializeField]
        Button m_recordingEnablingButton;

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

        public void AppendGestureType()
        {
            var gestureTypes = GestureTypesManager.Instance.AppendGestureType(m_gestureTypeNameInputField.text);
            AppendGestureTypesOptions(new List<string> { m_gestureTypeNameInputField.text });
        }
        public void DeleteCurrentGestureType()
        {
            int nextValue = m_currentGesturesDropdown.value == m_currentGesturesDropdown.options.Count - 1 ?
                m_currentGesturesDropdown.options.Count - 2 : m_currentGesturesDropdown.value;
            string toBeDeleted = m_currentGesturesDropdown.options[m_currentGesturesDropdown.value].text;

            m_currentGesturesDropdown.ClearOptions();

            List<string> list = GestureTypesManager.Instance.DeleteGestureType(toBeDeleted);
            AppendGestureTypesOptions(list);

            m_currentGesturesDropdown.value = nextValue;
        }

        private void AppendGestureTypesOptions(List<string> list)
        {
            List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();
            foreach (var element in list)
            {
                dropdownOptions.Add(new Dropdown.OptionData
                {
                    text = element
                });
            }

            m_currentGesturesDropdown.AddOptions(dropdownOptions);
        }
        #endregion
    }
}
