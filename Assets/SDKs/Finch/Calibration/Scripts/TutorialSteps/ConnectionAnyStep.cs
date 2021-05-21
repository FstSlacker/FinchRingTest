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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Finch
{
    /// <summary>
    /// Calibration step, on which it's possible to connect any available combination of Finch nodes
    /// </summary>
    public class ConnectionAnyStep : ConnectionBaseStep
    {
        [Header("Screens")]
        /// <summary>
        /// Waiting for connection tutorial part
        /// </summary>
        public GameObject WaitingConnection;

        /// <summary>
        /// Connect first node tutorial part
        /// </summary>
        public GameObject ConnectionFirstNode;

        /// <summary>
        /// Launcher tutorial part
        /// </summary>
        public GameObject Launcher;

        /// <summary>
        /// Header text of the Launcher's overlay
        /// </summary>
        public NotificationWords LauncherHeader;

        /// <summary>
        /// Text on the hint overlay of the Launcher part 
        /// </summary>
        public NotificationCalibration LauncherHint;

        /// <summary>
        /// Text on the hint overlay of the First Node part 
        /// </summary>
        public NotificationCalibration FirstNodeHint;

        [Header("Launcher set")]
        /// <summary>
        /// GameObject of the Launcher Set
        /// </summary>
        public GameObject LaunchSet;

        /// <summary>
        /// Text on the hint overlay of the Current Set part 
        /// </summary>
        public NotificationSet CurrentSetHint;

        /// <summary>
        /// GameObject of the Touchpad hint 
        /// </summary>
        public GameObject TouchpadHint;

        /// <summary>
        /// GameObject of the Modes list 
        /// </summary>
        public GameObject ModesList;

        /// <summary>
        /// Array of texts for the Available Modes overlay
        /// </summary>
        public NotificationSet[] AvailableModes = new NotificationSet[4];

        /// <summary>
        /// State of the Launch button 
        /// </summary>
        public ChangeState ButtonLaunch;

        /// <summary>
        /// State of the Choose button
        /// </summary>
        public ChangeState ButtonChoose;

        [Header("Choose set")]
        /// <summary>
        /// GameObject of the Choose set part 
        /// </summary>
        public GameObject ChooseSet;

        /// <summary>
        /// Transform of the Choose set button
        /// </summary>
        public Transform ChooseSetButton;

        /// <summary>
        /// Array of texts for the Modes overlay 
        /// </summary>
        public NotificationSet[] Modes = new NotificationSet[4];

        /// <summary>
        /// Background GameObject, which can be disabled
        /// </summary>
        public GameObject DisableBg;

        /// <summary>
        /// Controller tutorial state
        /// </summary>
        public ChangeState ControllerTutorial;

        /// <summary>
        /// Controller arrow state
        /// </summary>
        public ChangeState ControllerArrow;

        /// <summary>
        /// Upper arm tutorial state
        /// </summary>
        public ChangeState UpperArmTutorial;

        /// <summary>
        /// Upper arm arrow state
        /// </summary>
        public ChangeState UpperArmArrow;

        [Header("Silhouette")]
        /// <summary>
        /// Silhuettes states
        /// </summary>
        public ChangeState[] Silhouettes = new ChangeState[4];

        [Header("Icons")]
        /// <summary>
        /// Animated icons
        /// </summary>
        public DeviceIcons Icons;

        protected const float endWaitingTime = 3f;
        protected float timeLoadNextStep;
        protected bool chooseSetScreenActive;
        protected bool buttonLaunchActive;
        protected bool oncePass;

        protected float timePrePart;
        protected bool isPrePart;
        protected bool isDisconnect;

        protected const float successDuration = 1.3f;
        protected const float prePartDuration = 8.8f;

        [Header("Available sets")]
        /// <summary>
        /// List of available playable sets
        /// </summary>
        public List<PlayableSet> SetsTurn = new List<PlayableSet>()
        {
        };

        private PlayableSet currentSet;

        public override void Init(FinchCalibrationSettings calibrationSettings)
        {
            base.Init(calibrationSettings);
            buttonLaunchActive = true;
            chooseSetScreenActive = false;

            if (FinchCalibration.TimeStampError)
            {
                timeStampsError = true;
            }
            else
            {
                timePrePart = Time.time + prePartDuration;
            }

            if (settings.Set != PlayableSet.Any || oncePass)
            {
                oncePass = true;
                NextStep();
            }
        }

        private void Update()
        {
            UpdateScanner(SetsTurn[SetsTurn.Count - 1]);
            UpdatePosition();
            UpdateScreen();
            UpdateLauncherButtons();

            UpdateTutorials(false);

            TimestampStatusUpdate();

            UpdateButtonController();
            UpdateIcons();
            UpdateChooseModeButtons();

        }

        /// <summary>
        /// Updates status of node timestamps
        /// </summary>
        protected void TimestampStatusUpdate()
        {
            isPrePart = (Time.time < timePrePart); ;

            if ((timeStampsError && controllersConnected > 0 && upperArmConnected > 0))
            {
                return;
            }
            else if (timeStampsError && controllersConnected == 0 && upperArmConnected == 0)
            {
                timeStampsError = false;
                timePrePart = Time.time + prePartDuration;
                FinchCalibration.TimeStampError = false;
            }
        }

        /// <summary>
        /// Updates states of visual elements of the step
        /// </summary>
        public virtual void UpdateScreen()
        {
            WaitingConnection.SetActive(Time.time < endWaitingTime);
            ConnectionFirstNode.SetActive(!WaitingConnection.activeSelf && controllersConnected == 0);
            Launcher.SetActive(!WaitingConnection.activeSelf && !ConnectionFirstNode.activeSelf);
            LaunchSet.SetActive(!chooseSetScreenActive);
            ChooseSet.SetActive(chooseSetScreenActive);
        }

        /// <summary>
        /// Updates button states of the step
        /// </summary>
        protected void UpdateButtonController()
        {
            RingElement home = RingElement.HomeButton;
            bool pressedButton = FinchController.GetPressDown(Chirality.Any, home) || Input.GetKeyDown(KeyCode.D);
            bool swipedUp =!loadNextStep && (FinchController.LeftController.SwipeTop || FinchController.RightController.SwipeTop || Input.GetKeyDown(KeyCode.W));
            bool swipedDown = !loadNextStep && (FinchController.LeftController.SwipeBottom || FinchController.RightController.SwipeBottom || Input.GetKeyDown(KeyCode.S));

            if (!Launcher.activeSelf)
            {
                currentSet = GetCurrentSet();
                return;
            }

            if (chooseSetScreenActive)
            {
                UpdateChooseSetScreen(FinchController.GetPressDown(Chirality.Any, home), swipedUp, swipedDown);
            }
            else
            {
                UpdateLaunchScreen(FinchController.GetPressDown(Chirality.Any, home) && FinchController.GetPressTime(Chirality.Any,home) < .5f, swipedUp, swipedDown);
            }
        }

        /// <summary>
        /// Updates elements in Choose Step iteration of the step
        /// </summary>
        protected void UpdateChooseSetScreen(bool pressedButton, bool swipedUp, bool swipedDown)
        {
            int currentId = SetsTurn.FindIndex(x => x == currentSet);

            if (swipedUp)
            {
                //Choose prev set.
                currentId--;
                if (currentId < 0)
                {
                    currentId = SetsTurn.Count - 1;
                }

                UpdateTutorials(true);

                currentSet = SetsTurn[currentId];
                Icons.ResetIcons((int)currentSet % 10, (int)currentSet / 10, controllersConnected, upperArmConnected, false);
            }

            if (swipedDown)
            {
                //Choose next set.
                currentId++;
                if (currentId > SetsTurn.Count - 1)
                {
                    currentId = 0;
                }

                UpdateTutorials(true);

                currentSet = SetsTurn[currentId];
                Icons.ResetIcons((int)currentSet % 10, (int)currentSet / 10, controllersConnected, upperArmConnected, false);
            }

            bool fullSet = (int)currentSet % 10 <= controllersConnected && (int)currentSet / 10 <= upperArmConnected;

            if (pressedButton && fullSet && !loadNextStep)
            {
                //Load next step
                loadNextStep = true;
                timeLoadNextStep = Time.time + 0.2f;
                FinchNodeManager.StopScan();
                settings.Set = currentSet;
                Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
            }

            if (loadNextStep && Time.time > timeLoadNextStep)
            {
                loadNextStep = false;
                oncePass = true;
                settings.Set = currentSet;
                Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
                isPrePart = false;
                CheckTimeStamps();
            }
        }

        /// <summary>
        /// Updates elements in Launch Screen iteration of the step
        /// </summary>
        protected void UpdateLaunchScreen(bool pressedButton, bool swipedUp, bool swipedDown)
        {
            currentSet = GetCurrentSet();

            if (loadNextStep && Time.time > timeLoadNextStep)
            {
                loadNextStep = false;
                oncePass = true;
                settings.Set = currentSet;
                Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
                isPrePart = false;
                CheckTimeStamps();
            }

            if (pressedButton && !loadNextStep)
            {
                if (buttonLaunchActive)
                {
                    FinchNodeManager.StopScan();
                    settings.Set = currentSet;
                    Internal.FinchNodeManager.NormalizeNodeCount((int)currentSet);
                    loadNextStep = true;
                    timeLoadNextStep = Time.time + 0.2f;
                }
                else
                {
                    chooseSetScreenActive = true;
                    Icons.ResetIcons((int)currentSet % 10, (int)currentSet / 10, controllersConnected, upperArmConnected, false);
                }
            }

            buttonLaunchActive |= swipedUp;
            buttonLaunchActive &= !swipedDown;
        }

        /// <summary>
        /// Returns current playable set
        /// </summary>
        protected PlayableSet GetCurrentSet()
        {
            for (int i = SetsTurn.Count - 1; i >= 0; i--)
            {
                if(controllersConnected >= (int)SetsTurn[i] % 10 && upperArmConnected >= (int)SetsTurn[i] / 10)
                {
                    return SetsTurn[i];
                }
            }

            return SetsTurn[0];
        }

        /// <summary>
        /// Updates button states of Choose Screen iteration of the step
        /// </summary>
        protected void UpdateChooseModeButtons()
        {
            int currentId = SetsTurn.FindIndex(x => x == currentSet);
            if (currentId >= 0 && currentId < 4)
            {
                //Move and update indicator.
                Vector3 pos = ChooseSetButton.transform.localPosition;
                pos.y = Modes[currentId].transform.localPosition.y;
                ChooseSetButton.transform.localPosition = pos;
                DisableBg.SetActive((int)currentSet % 10 > controllersConnected || (int)currentSet / 10 > upperArmConnected);
            }

            for(int i = 0; i < SetsTurn.Count; i++)
            {
                Modes[i].gameObject.SetActive(i < SetsTurn.Count);

                if (i < SetsTurn.Count)
                {
                    Modes[i].ID = SetsTurn[i];
                }
            }
        }

        /// <summary>
        /// Updates button states of Launch Screen iteration of the step
        /// </summary>
        protected void UpdateLauncherButtons()
        {
            CurrentSetHint.ID = (PlayableSet)Mathf.Max((int)currentSet, 1);

            if (ButtonLaunch.FinishState != buttonLaunchActive || ButtonChoose.FinishState == buttonLaunchActive)
            {
                ButtonLaunch.FinishState = buttonLaunchActive;
                ButtonChoose.FinishState = !buttonLaunchActive;
                ButtonLaunch.ResetState(false);
                ButtonChoose.ResetState(false);
            }

            TouchpadHint.SetActive(buttonLaunchActive);
            ModesList.SetActive(!buttonLaunchActive);

            int idLastHint = 0;

            for (int i = 0; i < SetsTurn.Count; i++)
            {
                //Turn on available set hint.
                bool availableSet = (int)SetsTurn[i] % 10 <= controllersConnected && (int)SetsTurn[i] / 10 <= upperArmConnected;

                if (availableSet)
                {
                    AvailableModes[idLastHint].ID = SetsTurn[i];
                    AvailableModes[idLastHint].gameObject.SetActive(!buttonLaunchActive);

                    idLastHint++;
                }
            }

            for (int i = idLastHint; i < SetsTurn.Count; i++)
            {
                AvailableModes[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Updates visual tutorials of the step
        /// </summary>
        protected void UpdateTutorials(bool force)
        {
            bool fullSet = (int)currentSet % 10 <= controllersConnected && (int)currentSet / 10 <= upperArmConnected;
            bool allconnected = controllersConnected + upperArmConnected == 4 || chooseSetScreenActive && fullSet;

            if (PrePart.activeSelf)
            {
                LauncherHint.ID = CalibrationPhraseId.Open;
                LauncherHeader.Id = NotificationWord.Ready;
            }
            else if (Error.activeSelf)
            {
                LauncherHint.ID = CalibrationPhraseId.Put;
                LauncherHeader.Id = NotificationWord.Warning;
            }
            else if (ConnectionFirstNode.activeSelf)
            {
                LauncherHint.ID = CalibrationPhraseId.ConnectNodes;
                LauncherHeader.Id = NotificationWord.Ready;
            }
            else
            {
                if (allconnected)
                {
                    LauncherHint.ID = CalibrationPhraseId.ReadyConnect;
                    LauncherHeader.Id = chooseSetScreenActive ? NotificationWord.ChooseMode : NotificationWord.Launch;
                }
                else if (controllersConnected < (int)currentSet % 10)
                {
                    LauncherHint.ID = CalibrationPhraseId.ConnectNodes;
                    LauncherHeader.Id = chooseSetScreenActive ? NotificationWord.ChooseMode : NotificationWord.Launch;
                }
                else
                {
                    LauncherHint.ID = CalibrationPhraseId.ConnectUpperArms;
                    LauncherHeader.Id = chooseSetScreenActive ? NotificationWord.ChooseMode : NotificationWord.Launch;
                }
            }

            bool silhouetteHint = fullSet || !chooseSetScreenActive;
            bool controllerHint = !fullSet && chooseSetScreenActive && controllersConnected < (int)currentSet % 10;
            bool upperArmHint = !fullSet && !controllerHint && chooseSetScreenActive && upperArmConnected < (int)currentSet / 10;

            bool activeOld = !silhouetteHint && !Silhouettes[0].AnimationPass ||
                             !controllerHint && !ControllerTutorial.AnimationPass ||
                             !upperArmHint && !UpperArmTutorial.AnimationPass;


            int controllersNeed = (int)currentSet % 10;
            int upperArmsNeed = Math.Min(controllersNeed, (int)currentSet / 10);

            PrePart.SetActive(isPrePart && !timeStampsError && !Error.activeSelf);

            UpdateState(Silhouettes[0], silhouetteHint && controllersNeed > 0, force, activeOld);
            UpdateState(Silhouettes[1], silhouetteHint && controllersNeed > 1, force, activeOld);
            UpdateState(Silhouettes[2], silhouetteHint && upperArmsNeed > 0, force, activeOld);
            UpdateState(Silhouettes[3], silhouetteHint && upperArmsNeed > 1, force, activeOld);

            UpdateState(ControllerTutorial, controllerHint, force, activeOld);
            UpdateState(ControllerArrow, controllerHint, force, activeOld);
            UpdateState(UpperArmTutorial, upperArmHint, force, activeOld);
            UpdateState(UpperArmArrow, upperArmHint, force, activeOld);

            CommonPart.SetActive(!timeStampsError && !isPrePart && !Error.activeSelf);
            Error.SetActive(timeStampsError && !isPrePart);
        }

        /// <summary>
        /// Updates connected node icons states
        /// </summary>
        protected void UpdateIcons()
        {
            if (!Launcher.activeSelf)
            {
                int set = (int)SetsTurn[SetsTurn.Count - 1];
                Icons.ResetIcons(set % 10, set / 10, controllersConnected, upperArmConnected, true);
            }

            Icons.Update(controllersConnected, upperArmConnected);
        }
    }
}
