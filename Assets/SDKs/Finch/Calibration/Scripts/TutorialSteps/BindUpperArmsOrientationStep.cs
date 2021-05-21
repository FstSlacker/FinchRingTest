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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Finch
{
    /// <summary>
    /// Calibration step for assigning orientation to the Finch Upper Arm nodes.
    /// </summary>
    public class BindUpperArmsOrientationStep : TutorialStep
    {
        /// <summary>
        /// Text on the overlay
        /// </summary>
        public NotificationCalibration Notification;

        /// <summary>
        /// Header text of the overlay
        /// </summary>
        public NotificationWords Header;

        protected const float upperArmAngleBorder = 0.5f; //30 degrees

        protected int prevUpperArmCount = 0;

        protected bool initialState = false;
        protected bool waitingCalibrationState = false;

        public override void Init(FinchCalibrationSettings settings)
        {
            bool useTwoUpperArms = Internal.Settings.ReplyManager.GetArmsConnectedMode() == ArmsConnected.TwoArmsSixDof;
            bool uncalibrateUpperArms = prevUpperArmCount != (int)settings.Set / 10;

            if (!settings.IsMomentalCalibration && useTwoUpperArms && (settings.WasNodeReconnect || uncalibrateUpperArms))
            {
                base.Init(settings);
                waitingCalibrationState = false;

                initialState = true;
                Internal.Calibration.ReplyManager.Calibration(Internal.FinchCore.Finch_CalibrationType.Reset, Internal.FinchCore.Finch_CalibrationOptions.ResetReverting);
                Internal.Calibration.ReplyManager.CalibrationReply += DoAfterInitialState;
            }
            else
            {
                NextStep();
            }
        }

        private void Update()
        {
            if (!initialState)
            {
                HandleUpdate();
            }

            UpdatePosition();

            NotificationUpdate();
        }

        /// <summary>
        /// Calibration successful reset callback
        /// </summary>
        protected void DoAfterInitialState(object obj, EventArgs args)
        {
            initialState = false;
            //Save last UpperArm set.
            prevUpperArmCount = (int)settings.Set / 10;
            HandleUpdate();
        }

        /// <summary>
        /// Update of notification updates on the step
        /// </summary>
        protected void NotificationUpdate()
        {
            Notification.ID = CalibrationPhraseId.BindUpperArmOrientation;
            Header.Id = NotificationWord.Calibration;
        }

        protected void HandleUpdate()
        {
            if (waitingCalibrationState)
            {
                return;
            }

            bool revertLeft;
            bool revertRight;

            if (TryBindOrientation(NodeType.RightUpperArm, out revertRight) && TryBindOrientation(NodeType.LeftUpperArm, out revertLeft))
            {
                if (revertLeft || revertRight)
                {
                    if (revertLeft)
                    {
                        Internal.FinchCore.Finch_RevertCalibration(Internal.FinchCore.Finch_Node.LeftUpperArm);
                    }

                    if (revertRight)
                    {
                        Internal.FinchCore.Finch_RevertCalibration(Internal.FinchCore.Finch_Node.RightUpperArm);
                    }

                    waitingCalibrationState = true;
                    Internal.Calibration.ReplyManager.Calibration(Internal.FinchCore.Finch_CalibrationType.None, Internal.FinchCore.Finch_CalibrationOptions.CalibrateReverting);
                    Internal.Calibration.ReplyManager.CalibrationReply += DoAfterWaitingCalibrationState;
                }
                else
                {
                    NextStep();
                }
            }
        }

        protected void DoAfterWaitingCalibrationState(object obj, EventArgs args)
        {
            waitingCalibrationState = false;
            NextStep();
        }

        protected bool TryBindOrientation(NodeType node, out bool shouldRevert)
        {
            var result = Internal.Calibration.Calculations.GetArmDirection((Internal.FinchCore.Finch_Node)node, upperArmAngleBorder);
            shouldRevert = (result == Internal.Calibration.ArmsDirection.Up);
            return (result != Internal.Calibration.ArmsDirection.Forward);
        }
    }
}
