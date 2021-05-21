// Copyright 2018 - 2020 Finch Technologies Ltd. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Visualizes controller buttons elements.
    /// </summary>
    [Serializable]
    public class Buttons
    {
        /// <summary>
        /// Visualized element type.
        /// </summary>
        public RingElement ControllerElement;

        /// <summary>
        /// Visualisation mesh of button.
        /// </summary>
        public MeshRenderer ButtonMesh;

        private readonly Color pressed = new Color(0.671f, 0.671f, 0.671f);
        private readonly Color unpressed = Color.black;

        /// <summary>
        /// Update pressing state of buttons.
        /// </summary>
        /// <param name="isPressing">True if the button pressed at the moment.</param>
        public void UpdateState(bool isPressing)
        {
            ButtonMesh.material.color = isPressing ? pressed : unpressed;
            ButtonMesh.material.SetColor("_EmissionColor", isPressing ? pressed : unpressed);
        }
    }

    [Serializable]
    /// <summary>
    /// Used to visualize battery power. Displays various sprites that visualize the charge, depending on the current battery level.
    /// </summary>
    public class BatteryLevel
    {
        /// <summary>
        /// Sprite, visualizing a certain level of charge.
        /// </summary>
        public Sprite BatterySprite;

        /// <summary>
        /// The level of charge in percent.
        /// </summary>
        [Range(0, 100)]
        public int MinimumBatteryBorder;
    }

    /// <summary>
    /// Visualizes controller elements: buttons, touchpad, stick, battery level etc.
    /// </summary>
    public class FinchControllerVisual : FinchControllerTracked
    {
        [Header("Model")]
        /// <summary>
        /// Controller's state visualization Unity scene object.
        /// </summary>
        public GameObject Model;

        [Header("State")]
        /// <summary>
        /// Defines if the controllers should be hidden in calibration module.
        /// </summary>
        public bool HideInCalibration = true;

        /// <summary>
        /// Defines if the controllers should be hidden while it's unpaired.
        /// </summary>
        public bool HideWhenUnpair = true;

        [Header("Buttons")]
        /// <summary>
        /// List of visualized buttons.
        /// </summary>
        public Buttons[] Buttons = new Buttons[0];

        [Header("Battery")]
        /// <summary>
        /// Battery level visualization sprite.
        /// </summary>
        public SpriteRenderer BatteryRenderer;

        /// <summary>
        /// Different battery levels visualization sprites array.
        /// </summary>
        public BatteryLevel[] BatteryLevels = new BatteryLevel[4];

        [Header("Touch element")]
        /// <summary>
        /// Position and rotation of the element model, visualizing touch point on the touchpad model.
        /// </summary>
        public Transform TouchPoint;

        private FinchController controller;
        private float batteryLevel;
        private float touchPointPower;

        private const float epsilon = 0.05f;
        private const float chargeLevelEpsilon = 1.5f;
        private const float touchPointDepth = 0.001f;
        private const float touchPadRadius = 0.0175f;
        private const float touchPointRadius = 0.0056f;
        private const float scaleTimer = 0.15f;

        private bool isBzzed = false;

        private void LateUpdate()
        {
            if (FinchController.GetPressTime(Chirality, RingElement.HomeButton) >= 1.5f && !isBzzed)
            {
                isBzzed = true;
                FinchController.HapticPulse((NodeType)Chirality, FinchHapticPattern.LongClick);
            }

            if (FinchController.GetPressUp(Chirality, RingElement.HomeButton))
            {
                isBzzed = false;
            }

            controller = FinchController.GetController(Chirality);

            UpdateButtons();
            UpdateBattery();
            UpdateTouchpad();
            UpdateState();
        }

        private void UpdateState()
        {
            bool hideCauseCalibration = FinchCalibration.IsCalibrating && HideInCalibration;
            bool hideCauseDisconnect = !controller.IsConnected && HideWhenUnpair;
            bool hideCauseUntouch = !controller.IsTouching && !controller.HomeButton;
            Model.SetActive(!hideCauseCalibration && !hideCauseDisconnect && !hideCauseUntouch);
        }

        private void UpdateButtons()
        {
            bool activeTouchPad = controller.GetPress(RingElement.Touch) && controller.TouchAxes.SqrMagnitude() > epsilon;

            foreach (var b in Buttons)
            {
                b.UpdateState(controller.GetPress(b.ControllerElement) && (b.ControllerElement != RingElement.Touch || activeTouchPad));
            }
        }

        private void UpdateBattery()
        {
            if (BatteryRenderer == null)
            {
                return;
            }

            bool isBatteryActive = controller.IsConnected && BatteryLevels.Length > 0;
            if (BatteryRenderer.gameObject.activeSelf != isBatteryActive)
            {
                BatteryRenderer.gameObject.SetActive(isBatteryActive);
            }

            float currentBatteryLevel = Mathf.Clamp(controller.BatteryCharge, 0f, 99.9f);
            if (isBatteryActive && Math.Abs(currentBatteryLevel - batteryLevel) > chargeLevelEpsilon)
            {
                Sprite batterySprite = null;
                float maxBorder = 0;

                batteryLevel = currentBatteryLevel;

                foreach (BatteryLevel i in BatteryLevels)
                {
                    if (currentBatteryLevel > i.MinimumBatteryBorder && maxBorder <= i.MinimumBatteryBorder)
                    {
                        maxBorder = i.MinimumBatteryBorder;
                        batterySprite = i.BatterySprite;
                    }
                }

                BatteryRenderer.sprite = batterySprite;
            }
        }

        private void UpdateTouchpad()
        {
            Vector3 size = new Vector3(touchPointRadius, touchPointDepth, touchPointRadius);
            float speed = Time.deltaTime / Mathf.Max(epsilon, scaleTimer) * (controller.GetPress(RingElement.Touch) ? 1 : -1);

            touchPointPower = Mathf.Clamp01(touchPointPower + speed);

            if (TouchPoint != null)
            {
                TouchPoint.localScale = controller.GetPress(RingElement.HomeButton) ? Vector3.zero : size * touchPointPower;

                if (controller.IsTouching)
                {
                    TouchPoint.localPosition = new Vector3(controller.TouchAxes.x, TouchPoint.localPosition.y, controller.TouchAxes.y) * touchPadRadius;
                }
            }
        }
    }
}
