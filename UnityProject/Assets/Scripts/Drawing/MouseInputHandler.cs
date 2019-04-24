using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Drawing
{
    public class MouseInputHandler : MonoBehaviour
    {
        bool _isRecording;
        public GridDrawer GridDrawer;
        public Camera MainCamera;
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isRecording = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isRecording = false;
            }

            if (_isRecording)
            {
                var mousePosition = Input.mousePosition;
                var relativeX = mousePosition.x / Screen.width;
                var relativeY = mousePosition.y / Screen.height;

                var ray = MainCamera.ViewportPointToRay(new Vector3(relativeX, relativeY, 0));

                float intersectionResult = 0;

                var plane = new Plane(transform.forward, transform.position);
                plane.Raycast(ray, out intersectionResult);

                Vector3 intersection = ray.origin + ray.direction * intersectionResult;

                float horizontalRelative = (intersection.x - GridDrawer.TopLeftCorner.x) / (GridDrawer.TopRightCorner.x - GridDrawer.TopLeftCorner.x);
                float verticalRelative = (intersection.y - GridDrawer.TopLeftCorner.y) / (GridDrawer.BottomLeftCorner.y - GridDrawer.TopLeftCorner.y);

                if (horizontalRelative > 1 || horizontalRelative < 0 || verticalRelative > 1 || verticalRelative < 0)
                    return;

                GridDrawer.Paint(horizontalRelative, verticalRelative);
            }
        }
    }
}
