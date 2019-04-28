using HoloToolkit.Examples.InteractiveElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UX
{
    public class HoloDropDown : MonoBehaviour
    {
        public event Action<DropDownElement> OnDropDownSelected;
        public int SelectedIndex { get; private set; }
        public int Count { get { return m_instantiatedElements.Count; } }
        [SerializeField]
        GameObject m_dropDownElementPrefab;
        [SerializeField]
        InteractiveSet m_interactiveSet;

        const float VerticalSize = 0.057f;
        List<string> m_data = new List<string>();
        public List<string> Data
        {
            get
            {
                return m_data;
            }
            private set
            {
                m_data = value;
            }
        }
        List<DropDownElement> m_instantiatedElements = new List<DropDownElement>();
        public void SelectByIndex(int index)
        {
            SelectedIndex = index;
            m_interactiveSet.HandleOnSelection(index);
        }
        public void SetList(List<string> newData)
        {
            ClearList();

            Data = newData;

            DisplayList();
        }
        public void DisplayList()
        {
            if (m_interactiveSet.Interactives == null)
                m_interactiveSet.Interactives = new List<InteractiveToggle>();

            for (int i = 0; i < Data.Count; i++)
            {
                GameObject dropDownGameObject = Instantiate(m_dropDownElementPrefab);
                dropDownGameObject.transform.parent = this.transform;
                dropDownGameObject.transform.localPosition = Vector3.down * VerticalSize * i;


                DropDownElement dropDownElement = dropDownGameObject.GetComponent<DropDownElement>();
                dropDownElement.SetText(Data[i]);
                dropDownElement.SetSelectionCallback((ddElement) =>
                {
                    if (OnDropDownSelected != null)
                        OnDropDownSelected(ddElement);
                });

                m_instantiatedElements.Add(dropDownElement);
                m_interactiveSet.Interactives.Add(dropDownElement.GetComponent<InteractiveToggle>());
            }
        }

        public void ClearList()
        {
            for (int i = m_instantiatedElements.Count - 1; i >= 0; i--)
            {
                m_interactiveSet.RemoveInteractive(i);
                Destroy(m_instantiatedElements[i].gameObject);
                m_instantiatedElements.RemoveAt(i);
            }
        }
    }
}