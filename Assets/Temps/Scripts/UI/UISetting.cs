using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Roguelike
{
    public class UISetting : View
    {
        private Button m_close;
        private Slider m_brightness;
        private Slider m_voice;

        private Input m_input;

        private void Awake()
        {
            m_input = new Input();
            m_input.KeyboardAndMouse.Pause.started += Pause_started;

            Init();
        }

        private void OnEnable()
        {
            m_input.Enable();
        }

        private void Update()
        {
            Brightness();
        }

        private void OnDisable()
        {
            m_input.Disable();
        }

        private void Pause_started(InputAction.CallbackContext obj)
        {
            Close();
        }

        public override void Init()
        {
            m_close = this.transform.Find("Close").GetComponent<Button>();
            m_brightness = this.transform.Find("Brightness").GetComponent<Slider>();
            m_voice = this.transform.Find("Voice").GetComponent<Slider>();

            m_close.onClick.AddListener(Close);

            m_brightness.value = 0.5f;
            m_voice.value = 0.5f;
        }

        /// <summary>
        /// �رհ�ť�󶨺���
        /// </summary>
        private void Close()
        {
            this.Hide();
            EventManager.TriggerEvent("UIPauseUnlock");
        }

        /// <summary>
        /// �������󶨺���
        /// </summary>
        private void Brightness()
        {

            EventManager.TriggerEvent<float>("ChangeBrightness", 0.5f + m_brightness.value);
            Debug.Log("-----��ǰ����---"+ (0.5f + m_brightness.value));
        }

        /// <summary>
        /// �������󶨺���
        /// </summary>
        private void Voice()
        {

        }
    }
}
