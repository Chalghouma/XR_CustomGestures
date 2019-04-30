using Assets.Scripts.Gesture;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Drawing
{
    public class GridDrawer : MonoBehaviour, IFocusable
    {
        const int Horizontal_Size = 56;
        const int Vertical_Size = 56;
        public GameObject GridElementPrefab;
        private Camera m_camera;
        private Renderer _renderer;

        byte[] _data;
        Texture2D _texture2D;
        public static GridDrawer Instance;
        void Start()
        {
            InitializeTextureByteArray();
            BuildCorners();
            //BuildGrid();

            m_camera = CameraCache.Main;
            Instance = this;

        }

        private void InitializeTextureByteArray()
        {
            _data = new byte[Horizontal_Size * Vertical_Size];
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = 0x80;
            }

            _renderer = GetComponent<Renderer>();
            _texture2D = new Texture2D(Horizontal_Size, Vertical_Size, TextureFormat.R8, false);
            _renderer.material.mainTexture = _texture2D;
            for (int y = 0; y < Horizontal_Size; y++)
            {
                for (int x = 0; x < Vertical_Size; x++)
                {
                    _texture2D.SetPixel(x, y, Color.black);

                }
            }
            _texture2D.Apply();
        }

        public Vector3 TopLeftCorner, TopRightCorner, BottomLeftCorner, BottomRightCorner;
        private void BuildCorners()
        {
            _renderer = GetComponent<Renderer>();

            var bounds = _renderer.bounds;

            var horizontalOffset = bounds.size.x / 2;
            var verticalOffset = bounds.size.y / 2;

            TopLeftCorner = transform.position - transform.right * horizontalOffset + transform.up * verticalOffset;
            TopRightCorner = transform.position + transform.right * horizontalOffset + transform.up * verticalOffset;

            BottomLeftCorner = transform.position - transform.right * horizontalOffset - transform.up * verticalOffset;
            BottomRightCorner = transform.position + transform.right * horizontalOffset - transform.up * verticalOffset;

            var list = new List<Vector3> { TopLeftCorner, TopRightCorner, BottomLeftCorner, BottomRightCorner };
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

            var hVector = (TopRightCorner - TopLeftCorner);
            var vVector = (BottomLeftCorner - TopLeftCorner);

            GameObject gridHolder = new GameObject { name = "gridHolder" };

            Renderer renderer = GetComponent<Renderer>();
            var hSize = renderer.bounds.size.x / Horizontal_Size;
            var ySize = renderer.bounds.size.y / Vertical_Size;

            for (int i = 0; i < Horizontal_Size; i++)
            {
                for (int j = 0; j < Vertical_Size; j++)
                {
                    var gridElement = Instantiate(GridElementPrefab);
                    gridElement.layer = 10;

                    var gridComponent = gridElement.AddComponent<GridComponent>();
                    gridComponent.SetData(i, j, _grid.Count);

                    var hOffset = hVector * (float)j / (Horizontal_Size - 1);
                    var vOffset = vVector * (float)i / (Vertical_Size - 1);

                    gridElement.transform.position = TopLeftCorner + hOffset + vOffset + new Vector3(0, 0, -0001f);
                    gridElement.transform.localScale = new Vector3(hSize, ySize, 0.01f);

                    gridElement.GetComponent<Renderer>().material
                        .SetColor("_Color", new Color(Random.Range((float)0, 1), Random.Range((float)0, 1), Random.Range((float)0, 1)));
                    gridElement.GetComponent<Renderer>().material.color = Color.white;

                    _grid.Add(gridComponent);

                    gridElement.transform.parent = gridHolder.transform;
                }
            }
        }

        /// <summary>
        /// Paints the corresponding GridElement according to the relative position
        /// </summary>
        /// <param name="horizontalRelative">Clamped between [0,1]</param>
        /// <param name="verticalRelative">Clamped between [0,1]</param>
        public void Paint(float horizontalRelative, float verticalRelative)
        {
            int y = (int)Mathf.Clamp(Mathf.Round(Vertical_Size * verticalRelative), 0f, (int)(Vertical_Size - 1));
            int x = (int)(horizontalRelative * Horizontal_Size);
            int index = x + y * Horizontal_Size;

            PaintAtIndex(x, y);
            if (y != Vertical_Size - 1)
            {
                PaintAtIndex(x, y + 1);
            }
            if (x != Horizontal_Size - 1)
            {
                PaintAtIndex(x + 1, y);
                if (y != Horizontal_Size - 1)
                    PaintAtIndex(x + 1, y + 1);
            }


            _texture2D.Apply();
            Debug.LogFormat("Row,Column : {0}||{1} ----- ", y, x);
        }
        void PaintAtIndex(int x, int y)
        {
            _texture2D.SetPixel(x, (Vertical_Size - 1) - y, Color.red);
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


        public void CleanBlackColor()
        {
            for (int y = 0; y < Horizontal_Size; y++)
            {
                for (int x = 0; x < Vertical_Size; x++)
                {
                    _texture2D.SetPixel(x, y, Color.black);
                }
            }
            _texture2D.Apply();
        }

        public bool IsGazedAt { get; private set; }
        public void OnFocusEnter()
        {
            IsGazedAt = true;
        }

        public void OnFocusExit()
        {
            IsGazedAt = false;
        }

        public void PaintAccordingToInputSourcePosition(Vector3 inputSourcePosition)
        {
            float xRelative = 0, yRelative = 0;
            GetInputSourceRelativePosition(inputSourcePosition, out xRelative, out yRelative);

            if (xRelative < 0 || xRelative > 1 || yRelative < 0 || yRelative > 1)
                return;

            Paint(xRelative, yRelative);
        }

        void GetInputSourceRelativePosition(Vector3 inputSourcePosition, out float xRelative, out float yRelative)
        {
            Vector3 inverted = m_camera.transform.InverseTransformPoint(inputSourcePosition);

            xRelative = (inverted.x - TopLeftCorner.x) / CustomGestureRecognizer.DetectableFrameWidth;
            yRelative = -(inverted.y - TopLeftCorner.y) / CustomGestureRecognizer.DetectableFrameHeight;

            Debug.LogFormat("X,Y = {0},{1}", xRelative, yRelative);
        }
    }
}
