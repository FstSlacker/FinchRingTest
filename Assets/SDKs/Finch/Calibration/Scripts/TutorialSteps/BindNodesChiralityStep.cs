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
using System;

namespace Finch
{
    /// <summary>
    /// Calibration step for assigning chiralities (left or right) to the Finch Upper Arm nodes.
    /// </summary>
    public class BindNodesChiralityStep : TutorialStep
    {
        /// <summary>
        /// Text on the overlay
        /// </summary>
        public NotificationCalibration Notification;

        /// <summary>
        /// Header text of the overlay
        /// </summary>
        public NotificationWords Header;

        public bool IsControllerStep;

        protected int prevNodesCount = 0;

        protected bool waitingCalibrationState = false;

        public override void Init(FinchCalibrationSettings settings)
        {
            int nodeCount = IsControllerStep ? (int)settings.Set % 10 : (int)settings.Set / 10;
            bool uncalibrateNodes = prevNodesCount != nodeCount;

            if (!settings.IsMomentalCalibration && nodeCount == 2 && (settings.WasNodeReconnect || uncalibrateNodes))
            {
                base.Init(settings);
                waitingCalibrationState = false;

                //Save last UpperArm set.
                prevNodesCount = (int)settings.Set / 10;

                Update();
            }
            else
            {
                NextStep();
            }
        }

        private void Update()
        {
            if (!waitingCalibrationState)
            {
                HandleUpdate();
            }

            UpdatePosition();
            NotificationUpdate();
        }

        protected void HandleUpdate()
        {
            NodeType leftNode = IsControllerStep ? NodeType.LeftHand : NodeType.LeftUpperArm;
            NodeType rightNode = IsControllerStep ? NodeType.RightHand : NodeType.RightUpperArm;

            bool pressLeft = FinchInput.GetPressDown(leftNode, RingElement.HomeButton);
            bool pressRight = FinchInput.GetPressDown(rightNode, RingElement.HomeButton);

            //if press left node it that means user swap UpperArms
            if (pressLeft)
            {
                waitingCalibrationState = true;
                FinchNodeManager.SwapNodes(leftNode, rightNode);
                Internal.Calibration.ReplyManager.CalibrationReply += DoAfterWaitingCalibrationState;
                return;
            }

            if (pressLeft || pressRight)
            {
                NextStep();
            }
        }

        protected void DoAfterWaitingCalibrationState(object obj, EventArgs args)
        {
            waitingCalibrationState = false;
            NextStep();
        }

        protected void NotificationUpdate()
        {
            Notification.ID = CalibrationPhraseId.BindUpperArmChirality;
            Header.Id = NotificationWord.BindUpperArmChiralityHeader;
        }
    }
}
