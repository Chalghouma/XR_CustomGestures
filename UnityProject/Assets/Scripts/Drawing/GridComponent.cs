using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Drawing
{
    class GridComponent : MonoBehaviour
    {
        public Renderer Renderer;
        private void Start()
        {
            Renderer = GetComponent<Renderer>();
        }
        public int Row, Column, Index;
        public bool IsSet;
        public void SetData(int row, int column, int index)
        {
            this.Row = row;
            this.Column = column;
            this.Index = index;
        }

        public void Clean()
        {
            IsSet = false;
            Renderer.material.color = Color.white;
        }
        public void Trigger()
        {
            IsSet = true;
            Renderer.material.color = Color.blue;
        }
    }
}
