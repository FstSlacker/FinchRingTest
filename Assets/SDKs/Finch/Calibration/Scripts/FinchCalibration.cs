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

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Finch
{
    /// <summary>
    /// Settings for calibration start and calibration process.
    /// </summary>
    [Serializable]
    public class FinchCalibrationSettings
    {
        /// <summary>
        /// Character's head position and rotation quaternions.
        /// </summary>
        public Transform Head;

        /// <summary>
        /// Defines if user can recalibrate without calling full calibration.
        /// </summary>
        public bool AvailableMomentalCalibration = true;

        /// <summary>
        /// Defines if the calibration module is called at the application launch.
        /// </summary>
        public bool CalibrateOnStart = true;

        /// <summary>
        /// Defines if calibration should start after disconnecting a node or not
        /// </summary>
        public bool RecalibrateOnDisconnect = true;

        /// <summary>
        /// Which configuration (number and type) of controllers is used.
        /// </summary>
        public PlayableSet Set = PlayableSet.OneSixDof;

        /// <summary>
        /// Which nodes can call calibration module. 
        /// Mostly used for two-arms modes - using this option will allow you to call calibration by pressing button on the right arm only - pressing the button on the left arm will be reserved for different action. And vice versa.
        /// </summary>
        public Chirality RecalibrationConfiguration = Chirality.Any;

        /// <summary>
        /// Time of pressing the Calibration button needed to call the calibration module.
        /// </summary>
        public readonly float TimePressingToCallCalibration = 1f;

        /// <summary>
        /// Vibration time in milliseconds after the calibration is complete.
        /// </summary>
        public readonly ushort HapticTime = 120;

        /// <summary>
        /// Normally there is no need to change or use this variable.
        /// True if connection process will be for the part where you can connect any configuration of controllers.
        /// </summary>
        [HideInInspector]
        public bool IsAny;

        /// <summary>
        /// Normally there is no need to change or use this variable.
        /// True if everything is ready for momental calibration
        /// </summary>
        [HideInInspector]
        public bool IsMomentalCalibration;

        /// <summary>
        /// True if node was reconnected
        /// </summary>
        [HideInInspector]
        public bool WasNodeReconnect;
    }

    /// <summary>
    /// Manages calibration module according to current settings.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class FinchCalibration : MonoBehaviour
    {
        /// <summary>
        /// True if the calibration module is active at the moment.
        /// </summary>
        public static bool IsCalibrating { get; private set; }

        /// <summary>
        /// True if nodes were already calibrated.
        /// </summary>
        public static bool WasCalibrated { get; private set; }

        /// <summary>
        /// Action that happens on calibration start.
        /// </summary>
        public static Action OnCalibrationStart;

        /// <summary>
        /// Action that happens on calibration end.
        /// </summary>
        public static Action OnCalibrationEnd;

        /// <summary>
        /// True if timestamp error occured - means that FinchRing and FinchTracker were taken out of the FinchCradle at different times
        /// </summary>
        public static bool TimeStampError = false;

        [Header("Calibration settings")]
        /// <summary>
        /// Currently defined calibration parameters.
        /// </summary>
        public FinchCalibrationSettings CalibrationOptions = new FinchCalibrationSettings();

        [HideInInspector]
        public static RenderMode NotificationRenderMode = RenderMode.WorldSpace;

        [Header("Sound")]
        /// <summary>
        /// Sound that is played on calibration start.
        /// </summary>
        public AudioClip StartCalibrationSound;

        [Header("Steps")]
        /// <summary>
        /// Calibration module steps. Every step is responsible for specific calibration aspect.
        /// </summary>
        public TutorialStep[] CalibrationSteps = new TutorialStep[0];

        private static FinchCalibration singleton;
        private static int stepId;

        private AudioSource audioSource;
        private float timeToUnpair;

        private RecalibrationState recalibration = new RecalibrationState();
        private bool wasPaused;

        private void Start()
        {
            singleton = this;

            audioSource = GetComponent<AudioSource>();

            if (CalibrationOptions.RecalibrateOnDisconnect)
            {
                Internal.FinchCore.OnDisconnected += OnDisconnectNode;
            }
            Internal.FinchCore.OnConnected += OnConnectNode;

            FindHead();

            if (CalibrationOptions.Set == PlayableSet.Any)
            {
                CalibrationOptions.IsAny = true;
            }

            if (CalibrationOptions.CalibrateOnStart)
            {
                Calibrate();
            }
            else
            {
                foreach (var i in CalibrationSteps)
                {
                    i.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            FindHead();

            CallRecalibration();
        }

        private void CallRecalibration()
        {
            if (CalibrationOptions.RecalibrationConfiguration != Chirality.None)
            {
                recalibration.Update(CalibrationOptions, IsCalibrating);

                if (!IsCalibrating && recalibration.RecalibrationAvailable)
                {
                    CalibrationOptions.IsMomentalCalibration = CalibrationOptions.AvailableMomentalCalibration;
                    Calibrate();
                }
            }

            if (FinchNodeManager.IsScanning)
            {
                timeToUnpair = Time.time + 0.5f;
            }
        }

        private void FindHead()
        {
            if (CalibrationOptions.Head == null)
            {
                CalibrationOptions.Head = Camera.main.transform;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause && wasPaused)
            {
                wasPaused = false;
                WasCalibrated = false;

                if (!IsCalibrating)
                {
                    Calibrate();
                }
            }

            wasPaused |= pause;
        }

        private void OnConnectNode(NodeType node)
        {
            if (CalibrationOptions.Set != PlayableSet.Any)
            {
                Internal.FinchNodeManager.NormalizeNodeCount((int)CalibrationOptions.Set);
            }

            CalibrationOptions.WasNodeReconnect = true;
        }

        private void OnDisconnectNode(NodeType node)
        {
            bool needController = (int)node < 2 && FinchNodeManager.GetControllersCount() < (int)CalibrationOptions.Set % 10;
            bool needUpperArm = (int)node >= 2 && FinchNodeManager.GetUpperArmCount() < (int)CalibrationOptions.Set / 10;

            if (FinchNodeManager.IsExternalDisconnect(node) && (needController || needUpperArm) && Time.time > timeToUnpair)
            {
                TimeStampError = true;
                Calibrate();
            }
        }

        /// <summary>
        /// Calls calibration module.
        /// </summary>
        public static void Calibrate()
        {
            stepId = -1;

            if (singleton != null && singleton.StartCalibrationSound != null)
            {
                singleton.audioSource.Stop();
                Play(singleton.StartCalibrationSound);
            }

            OnCalibrationStart?.Invoke();

            NextStep();
        }

        /// <summary>
        /// Loads next calibration step.
        /// </summary>
        public static void NextStep()
        {
            if (singleton == null)
            {
                return;
            }

            stepId++;

            foreach (var i in singleton.CalibrationSteps)
            {
                i.gameObject.SetActive(false);
            }

            bool availableId = stepId >= 0 && stepId < singleton.CalibrationSteps.Length;

            WasCalibrated = !availableId;
            IsCalibrating = availableId;

            if (availableId)
            {
                singleton.CalibrationSteps[stepId].Init(singleton.CalibrationOptions);
            }
            else
            {
                singleton.CalibrationOptions.WasNodeReconnect = false;
                OnCalibrationEnd?.Invoke();
            }
        }

        /// <summary>
        /// Plays an audio clip.
        /// </summary>
        /// <param name="clip">Audio clip used at the calibration start.</param>
        public static void Play(AudioClip clip)
        {
            if (singleton != null && clip != null)
            {
                singleton.audioSource.clip = clip;
                singleton.audioSource.Play();
            }
        }
    }
}
