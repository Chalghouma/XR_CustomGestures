using Assets.Scripts.Drawing;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Gesture
{
    public class CustomGestureRecognizer : Singleton<CustomGestureRecognizer>, IInputHandler
    {
        public event Action<GestureRecognizedData> OnGestureRecognized;

        public const float DetectableFrameWidth = .5f;
        public const float DetectableFrameHeight = .5f;

        public GridDrawer GridDrawer;

        Vector3 _topLeftCorner, _bottomLeftCorner, _bottomRightCorner, _topRightCorner;
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
            _inputSource = eventData.InputSource;
            _inputSourceUID = eventData.SourceId;
            GridDrawer.CleanBlackColor();

            IsRecording = true;
        }

        public void OnInputUp(InputEventData eventData)
        {
            IsRecording = false;
            _inputSource = null;

            var data = GridDrawer.GetData();

            //StartCoroutine(SendGestureDataAsync(data));
        }
        IEnumerator SendGestureDataAsync(byte[] data)
        {
            var form = new WWWForm();
            form.AddBinaryData("binarydata", data);
            //form.AddBinaryData("binarydata", array);
            form.headers["Content-Type"] = "application/octet-stream";

            var www = UnityWebRequest.Post("localhost:6969/recognize", form);
            yield return www.SendWebRequest();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogFormat("Error :{0}", www.error);
            }
            else
            {
                var text = www.downloadHandler.text;
                Debug.LogFormat("Text:{0}", text);

                var resultData = JsonConvert.DeserializeObject<GestureRecognizedData>(text);
                if (OnGestureRecognized != null)
                    OnGestureRecognized(resultData);
            }
        }
        void BuildCorners()
        {
            _topLeftCorner = new Vector3(-DetectableFrameWidth / 2, DetectableFrameHeight / 2, 0);

            _bottomLeftCorner = new Vector3(-DetectableFrameWidth / 2, -DetectableFrameHeight / 2, 0);

            _bottomRightCorner = new Vector3(DetectableFrameWidth / 2, -DetectableFrameHeight / 2, 0);

            _topRightCorner = new Vector3(DetectableFrameWidth / 2, DetectableFrameHeight / 2, 0);
        }
        public void GetInputSourceRelativePosition(Vector3 inputSourcePosition, out float xRelative, out float yRelative)
        {
            Vector3 inverted = _camera.transform.InverseTransformPoint(inputSourcePosition);

            xRelative = (inverted.x - _topLeftCorner.x) / DetectableFrameWidth;
            yRelative = -(inverted.y - _topLeftCorner.y) / DetectableFrameHeight;

            //Debug.LogFormat("X,Y = {0},{1}", xRelative, yRelative);
        }

        void Update()
        {
            if (!IsRecording)
                return;

            PaintAccordingToGripPosition(this._inputSource, this._inputSourceUID);
        }

        public void PaintAccordingToGripPosition(IInputSource inputSource, uint inputSourceUID)
        {
            Vector3 inputSourcePosition = Vector3.zero;
            if (!inputSource.TryGetGripPosition(inputSourceUID, out inputSourcePosition))
                return;

            float x, y;
            GetInputSourceRelativePosition(inputSourcePosition, out x, out y);

            if (x < 0 || x > 1 || y < 0 || y > 1)
                return;

            GridDrawer.Paint(x, y);
        }
    }
}
