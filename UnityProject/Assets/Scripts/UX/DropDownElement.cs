using HoloToolkit.Examples.InteractiveElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UX
{
    public class DropDownElement : MonoBehaviour
    {
        public string Text { get { return m_textMesh.text; } }
        [SerializeField]
        private TextMesh m_textMesh;
        [SerializeField]
        private InteractiveToggle m_toggle;
        [SerializeField]
        private LabelTheme m_labelTheme;
        public void SetText(string text)
        {
            m_textMesh.text = text;
            m_labelTheme.Selected = text;
            m_labelTheme.Default = text;
            m_toggle.Keyword = text;
        }

        internal void SetSelectionCallback(Action<DropDownElement> callback)
        {
            m_toggle.OnSelection.AddListener(() =>
            {
                callback(this);
            });
        }
    }
}
