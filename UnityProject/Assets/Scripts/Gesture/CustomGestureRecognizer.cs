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
    public class CustomGestureRecognizer : MonoBehaviour, IInputHandler
    {
        const float DetectableFrameWidth = .5f;
        const float DetectableFrameHeight = .5f;
        const float DetectableFrameDistanceFromCamera = .75f;

        public GridDrawer GridDrawer;

        GameObject _topLeftCorner, _bottomLeftCorner, _bottomRightCorner, _topRightCorner;
        IInputSource _inputSource;
        uint _inputSourceUID;
        [SerializeField]
        private Camera _camera;
        void Start()
        {
            _camera = CameraCache.Main;

            BuildCorners();
        }
        public bool IsRecording { get; private set; }
        public void OnInputDown(InputEventData eventData)
        {
            IsRecording = true;
            _inputSource = eventData.InputSource;
            _inputSourceUID = eventData.SourceId;
            GridDrawer.CleanBlackColor();
        }

        public void OnInputUp(InputEventData eventData)
        {
            IsRecording = false;
            _inputSource = null;
        }
        void BuildCorners()
        {
            GameObject cornersHolder = new GameObject { name = "DetectableFrame_CornersHolder" };
            cornersHolder.transform.parent = _camera.transform;
            cornersHolder.transform.localPosition = Vector3.zero;
            cornersHolder.transform.localRotation = Quaternion.identity;

            _topLeftCorner = new GameObject { name = "TopLeftCorner" };
            _topLeftCorner.transform.parent = cornersHolder.transform;
            _topLeftCorner.transform.localPosition = new Vector3(-DetectableFrameWidth / 2, DetectableFrameHeight / 2, 0);

            _bottomLeftCorner = new GameObject { name = "BottomLeftCorner" };
            _bottomLeftCorner.transform.parent = cornersHolder.transform;
            _bottomLeftCorner.transform.localPosition = new Vector3(-DetectableFrameWidth / 2, -DetectableFrameHeight / 2, 0);

            _bottomRightCorner = new GameObject { name = "BottomRightCorner" };
            _bottomRightCorner.transform.parent = cornersHolder.transform;
            _bottomRightCorner.transform.localPosition = new Vector3(DetectableFrameWidth / 2, -DetectableFrameHeight / 2, 0);

            _topRightCorner = new GameObject { name = "TopRightCorner" };
            _topRightCorner.transform.parent = cornersHolder.transform;
            _topRightCorner.transform.localPosition = new Vector3(DetectableFrameWidth / 2, DetectableFrameHeight / 2, 0);
        }
        public GameObject Cursor;
        void GetInputSourceRelativePosition(Vector3 inputSourcePosition, out float xRelative, out float yRelative)
        {
            Vector3 inverted = _camera.transform.InverseTransformPoint(inputSourcePosition);

            Cursor.transform.parent = _camera.transform;
            Cursor.transform.localPosition = inverted;

            xRelative = (inverted.x - _topLeftCorner.transform.localPosition.x) / DetectableFrameWidth;
            yRelative = -(inverted.y - _topLeftCorner.transform.localPosition.y) / DetectableFrameHeight;

            Debug.LogFormat("X,Y = {0},{1}", xRelative, yRelative);
        }

        void Update()
        {
            if (_inputSource == null)
                return;

            Vector3 inputSourcePosition = Vector3.zero;
            if (!_inputSource.TryGetGripPosition(_inputSourceUID, out inputSourcePosition))
                return;

            float x, y;
            GetInputSourceRelativePosition(inputSourcePosition, out x, out y);

            if (x < 0 || x > 1 || y < 0 || y > 1)
                GridDrawer.Paint(x, y);
        }
    }
}
