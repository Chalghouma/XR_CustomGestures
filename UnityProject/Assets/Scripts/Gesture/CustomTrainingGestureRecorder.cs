using Assets.Scripts.Drawing;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gesture
{
    public class CustomTrainingGestureRecorder : Singleton<CustomTrainingGestureRecorder>, IInputHandler
    {
        private bool m_isHolding;
        private bool m_isRecordingAllowed;

        public bool IsHolding
        {
            get { return m_isHolding; }
            set { m_isHolding = value; }
        }
        public bool IsRecordingAllowed
        {
            get
            {
                return m_isRecordingAllowed;
            }
            set
            {
                m_isRecordingAllowed = value;
            }
        }
        public bool IsRecording
        {
            get { return IsHolding && m_isRecordingAllowed; }
        }
        IInputSource _inputSource;
        uint _inputSourceUID;
        Camera m_camera;

        public GridDrawer GridDrawer;
        void Start()
        {
            m_camera = CameraCache.Main;
            IsHolding = true;
            GridDrawer = FindObjectOfType<GridDrawer>();
        }
        public void OnInputDown(InputEventData eventData)
        {
            _inputSourceUID = eventData.SourceId;
            _inputSource = eventData.InputSource;

            IsHolding = true;
        }

        public void OnInputUp(InputEventData eventData)
        {
            IsHolding = false;
            _inputSource = null;
        }

        void Update()
        {
            if (!IsRecording)
                return;

            if (_inputSource == null)
                return;

            Vector3 inputSourcePosition = Vector3.zero;
            if (!_inputSource.TryGetGripPosition(_inputSourceUID, out inputSourcePosition))
                return;

            float x, y;
            CustomGestureRecognizer.Instance.GetInputSourceRelativePosition(inputSourcePosition, out x, out y);

            if (x < 0 || x > 1 || y < 0 || y > 1)
                return;

            GridDrawer.Paint(x, y);
        }
    }
}