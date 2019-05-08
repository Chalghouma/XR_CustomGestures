using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class DetectableFrameController : Singleton<DetectableFrameController>
    {
        public float DetectableFrameWidth = 0.5f;
        public float DetectableFrameHeight = 0.5f;

        const string StoredFrameSizeKey = "FrameSizeKey";

        [SerializeField]
        SliderGestureControl m_slider;

        float m_lastTimeSizeWasStored;
        private void Start()
        {
            float size = GetSize();
            SetSize(size);


            m_slider.SetSliderValue(size);
            m_slider.OnUpdateEvent.AddListener((sliderValue) =>
            {
                SetSize(sliderValue);

                if ((Time.time - m_lastTimeSizeWasStored) > 0.25f)
                {
                    m_lastTimeSizeWasStored = Time.time;
                    StoreSize(sliderValue);
                }
            });
        }

        private void StoreSize(float sliderValue)
        {
            PlayerPrefs.SetFloat(StoredFrameSizeKey, sliderValue);
        }
        private float GetSize()
        {
            if (PlayerPrefs.HasKey(StoredFrameSizeKey))
                return PlayerPrefs.GetFloat(StoredFrameSizeKey);

            return 0.5f;
        }
        public void SetSize(float size)
        {
            gameObject.transform.localScale = new Vector3(size, size, 1);

            DetectableFrameWidth = DetectableFrameHeight = size;
        }
    }
}
