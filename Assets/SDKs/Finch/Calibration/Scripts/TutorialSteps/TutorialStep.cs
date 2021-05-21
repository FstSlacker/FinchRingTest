// Copyright 2018-2020 Finch Technologies Ltd. All rights reserved.
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

using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Base calibraton step class. Inherit from TutorialStep if you want to make your own calibration step.
    /// </summary>
    public abstract class TutorialStep : MonoBehaviour
    {
        [Header("Position solver")]
        /// <summary>
        /// Defines how far calibration tips are positioned from the HMD.
        /// </summary>
        public Vector3 DistanceFromHMD;

        /// <summary>
        /// At this specified angle calibration tip will freeze and will stop to follow movements of user's head.
        /// </summary>
        public float MaxRotationDelta = 30;

        /// <summary>
        /// This value represents how smooth calibration tip will follow user's movements.
        /// </summary>
        public float StithnessY = 10f;

        [Header("Sound")]
        /// <summary>
        /// Sound played when user successfully finished the calibration.
        /// </summary>
        public AudioClip Success;

        protected FinchCalibrationSettings settings;

        private float angle;

        /// <summary>
        /// Initializes calibration tutorial step with the specified settings.
        /// </summary>
        /// <param name="calibrationSettings"></param>
        public virtual void Init(FinchCalibrationSettings calibrationSettings)
        {
            gameObject.SetActive(true);
            settings = calibrationSettings;
            angle = settings.Head.eulerAngles.y;
            UpdatePosition();
        }

        /// <summary>
        /// Loads next calibration module step.
        /// </summary>
        /// <param name="playSound">Defines if play sound on the start of the calibration step or not.</param>
        protected virtual void NextStep(bool playSound = true)
        {
            if (playSound)
            {
                FinchCalibration.Play(Success);
            }

            gameObject.SetActive(false);
            FinchCalibration.NextStep();
        }

        /// <summary>
        /// Updates calibration tips position data according to head's position.
        /// </summary>
        protected void UpdatePosition()
        {
            float delta = settings.Head.eulerAngles.y - angle;

            if (Mathf.Abs(delta) > 180)
            {
                delta = (Mathf.Abs(delta) - 360) * Mathf.Sign(delta);
            }

            if (Mathf.Abs(delta) > MaxRotationDelta)
            {
                float resultAngle = (settings.Head.eulerAngles.y - Mathf.Sign(delta) * MaxRotationDelta);
                delta = resultAngle - angle;

                if (Mathf.Abs(delta) > 180)
                {
                    delta = (Mathf.Abs(delta) - 360) * Mathf.Sign(delta);
                }

                angle += delta * (MaxRotationDelta == 0 ? 1 : Mathf.Clamp01(Time.deltaTime * StithnessY));
            }

            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

            Vector3 deltaX = DistanceFromHMD.x * (Vector3.right * cos + Vector3.forward * sin);
            Vector3 deltaY = DistanceFromHMD.y * Vector3.up;
            Vector3 deltaZ = DistanceFromHMD.z * (Vector3.right * sin + Vector3.forward * cos);

            transform.position = settings.Head.position + deltaX + deltaY + deltaZ;
            transform.LookAt(settings.Head.position, Vector3.up);
            transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
        }
    }
}
