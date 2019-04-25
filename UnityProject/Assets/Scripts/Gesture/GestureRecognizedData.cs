using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Gesture
{
    public class GestureRecognizedData
    {
        public double Confidence { get; set; }
        public double Predicted { get; set; }
        public string Label { get; set; }
    }
}
