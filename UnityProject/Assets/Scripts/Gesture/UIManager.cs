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
            
            m_recordingEnablingToggle.OnSelectEvents.AddListener(() => { ToggleRecording(); });

            m_addGestureType.OnButtonClicked += M_addGestureType_OnButtonClicked;
            m_removeGestureType.OnButtonClicked += M_removeGestureType_OnButtonClicked;
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

        private void SetDropdownList(Dropdown m_currentGesturesDropdown, List<string> list)
        {
            m_currentGesturesDropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (var item in list)
            {
                options.Add(new Dropdown.OptionData { text = item });
            }

            m_currentGesturesDropdown.AddOptions(options);
        }

        #endregion
    }
}
