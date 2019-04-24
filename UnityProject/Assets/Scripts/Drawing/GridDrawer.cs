using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Drawing
{
    public class GridDrawer : MonoBehaviour
    {
        const int Horizontal_Size = 56;
        const int Vertical_Size = 56;
        public GameObject GridElementPrefab;
        public Camera MainCamera;
        void Start()
        {
            BuildCorners();
            BuildGrid();
        }
        Vector3 topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner;
        private void BuildCorners()
        {
            var renderer = GetComponent<Renderer>();

            var bounds = renderer.bounds;

            var horizontalOffset = bounds.size.x / 2;
            var verticalOffset = bounds.size.y / 2;

            topLeftCorner = transform.position - transform.right * horizontalOffset + transform.up * verticalOffset;
            topRightCorner = transform.position + transform.right * horizontalOffset + transform.up * verticalOffset;

            bottomLeftCorner = transform.position - transform.right * horizontalOffset - transform.up * verticalOffset;
            bottomRightCorner = transform.position + transform.right * horizontalOffset - transform.up * verticalOffset;

            var list = new List<Vector3> { topLeftCorner, topRightCorner, bottomLeftCorner, bottomRightCorner };
            GameObject cornersHolder = new GameObject { name = "cornersHolder " };
            foreach (var item in list)
            {
                var cube = Instantiate(GridElementPrefab);
                cube.transform.localScale /= 5;
                cube.transform.position = item;

                cube.transform.parent = cornersHolder.transform;
            }
        }

        public void SaveGridAsImage(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var array = GetData();
            using (var fileStream = File.Create(filePath, array.Length))
            {
                fileStream.Write(array, 0, array.Length);
                fileStream.Flush();
            }
        }

        List<GridComponent> _grid;
        private void BuildGrid()
        {
            _grid = new List<GridComponent>();

            var hVector = (topRightCorner - topLeftCorner);
            var vVector = (bottomLeftCorner - topLeftCorner);

            GameObject gridHolder = new GameObject { name = "gridHolder" };

            Renderer renderer = GetComponent<Renderer>();
            var hSize = renderer.bounds.size.x / Horizontal_Size;
            var ySize = renderer.bounds.size.y / Vertical_Size;

            for (int i = 0; i < Horizontal_Size; i++)
            {
                for (int j = 0; j < Vertical_Size; j++)
                {
                    var gridElement = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gridElement.layer = 10;

                    var gridComponent = gridElement.AddComponent<GridComponent>();
                    gridComponent.SetData(i, j, _grid.Count);

                    var hOffset = hVector * (float)j / (Horizontal_Size - 1);
                    var vOffset = vVector * (float)i / (Vertical_Size - 1);

                    gridElement.transform.position = topLeftCorner + hOffset + vOffset + new Vector3(0, 0, -0001f);
                    gridElement.transform.localScale = new Vector3(hSize, ySize, 0.01f);

                    gridElement.GetComponent<Renderer>().material
                        .SetColor("_Color", new Color(Random.Range((float)0, 1), Random.Range((float)0, 1), Random.Range((float)0, 1)));
                    gridElement.GetComponent<Renderer>().material.color = Color.white;

                    _grid.Add(gridComponent);

                    gridElement.transform.parent = gridHolder.transform;
                }
            }
        }
        bool isRecording = false;
        public byte[] GetData()
        {
            var array = new byte[Horizontal_Size * Vertical_Size];

            foreach (var gridComponent in _grid)
            {
                array[gridComponent.Index] = gridComponent.IsSet ? (byte)255 : (byte)0;
            }

            return array;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isRecording = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isRecording = false;
            }

            if (isRecording)
            {
                var mousePosition = Input.mousePosition;
                var relativeX = mousePosition.x / Screen.width;
                var relativeY = mousePosition.y / Screen.height;

                var ray = MainCamera.ViewportPointToRay(new Vector3(relativeX, relativeY, 0));

                float intersectionResult = 0;

                var plane = new Plane(transform.forward, transform.position);
                plane.Raycast(ray, out intersectionResult);

                Vector3 intersection = ray.origin + ray.direction * intersectionResult;

                float horizontalRelative = (intersection.x - topLeftCorner.x) / (topRightCorner.x - topLeftCorner.x);
                float verticalRelative = (intersection.y - topLeftCorner.y) / (bottomLeftCorner.y - topLeftCorner.y);

                if (horizontalRelative > 1 || horizontalRelative < 0 || verticalRelative > 1 || verticalRelative < 0)
                    return;

                int rowIndex = (int)Mathf.Clamp(Mathf.Round(Vertical_Size * verticalRelative), 0f, (int)(Vertical_Size - 1));
                int verticalIndex = (int)(horizontalRelative * Horizontal_Size);
                int index = verticalIndex + rowIndex * Horizontal_Size;
                Debug.LogFormat("Row : {0} Col : {1}", rowIndex, verticalIndex);

                _grid[index].Trigger();
                if (rowIndex != Vertical_Size- 1)
                    _grid[index + Horizontal_Size].Trigger();
                if (verticalIndex != Horizontal_Size - 1)
                {
                    _grid[1 + index].Trigger();
                    if (verticalIndex != Vertical_Size - 1)
                        _grid[1 + index + Horizontal_Size].Trigger();
                }


            }
        }

        public void CleanBlackColor()
        {
            foreach (var gridElement in _grid)
            {
                gridElement.Clean();
            }
        }
    }
}
