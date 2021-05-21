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

namespace Finch
{
    /// <summary>
    /// Final calibration step
    /// </summary>
    public class RingCalibrationStep : TutorialStep
    {
        /// <summary>
        /// Calibration step errors
        /// </summary>
        public enum HandHint
        {
            /// <summary>
            /// No errors
            /// </summary>
            Base,
            /// <summary>
            /// "Lower your hands" - error appears when user's hands are too high 
            /// </summary>
            LowerHand,
            /// <summary>
            /// "Raise your hands" - error appears when user's hands are too low 
            /// </summary>
            UpperHand,
            /// <summary>
            /// "Straighten your arms" - error appears when user's arms are not parallel to each other or when arms are bent 
            /// </summary>
            StraightHand,
            /// <summary>
            /// Error appears when user is not looking forward
            /// </summary>
            HeadHint
        }

        protected enum State
        {
            None,
            InitialState,
            WaitingForUserState,
            WaitingForBindUpperArmOrientation,
            WaitingForCalibration,
        }

        [Header("Tutorials")]
        /// <summary>
        /// Text on the overlay
        /// </summary>
        public NotificationCalibration Notification;

        /// <summary>
        /// Header text of the overlay
        /// </summary>
        public NotificationWords Header;

        /// <summary>
        /// Calibration hint for two arms 6DoF set, ready for calibration
        /// </summary>
        public GameObject CalibrateBothHandsWithUpperArmsReady;

        /// <summary>
        /// Calibration hint for two arms 6DoF set
        /// </summary>
        public GameObject CalibrateBothHandsWithUpperArms;

        /// <summary>
        /// Calibration hint for two hands 3DoF set
        /// </summary>
        public GameObject CalibrateBothHandsWithoutUpperArms;

        /// <summary>
        /// Calibration hint for the right hand 3DoF set
        /// </summary>
        public GameObject CalibrateRigthHandWithoutArms;

        /// <summary>
        /// Calibration hint for the left hand 3DoF set
        /// </summary>
        public GameObject CalibrateLeftHandWithoutArms;

        /// <summary>
        /// Calibration hint for the right hand 6DoF set
        /// </summary>
        public GameObject CalibrateRigthHandWithArms;

        /// <summary>
        /// Calibration hint for the left hand 6DoF set
        /// </summary>
        public GameObject CalibrateLeftHandWithArms;

        [Header("Swap Hint")]
        /// <summary>
        /// Hint for left to right swap
        /// </summary>
        public GameObject LeftSwapHint;

        /// <summary>
        /// Hint for right to left swap
        /// </summary>
        public GameObject RightSwapHint;

        [Header("Errors")]
        /// <summary>
        /// Duration of errors notifications
        /// </summary>
        public float ErrorDuration = 1.5f;

        /// <summary>
        /// Array of the error animations
        /// </summary>
        public GameObject[] HorizontalError = new GameObject[5];

        protected const float angleBorderWide = 0.5f;

        protected const float RollAngleBorderSin = 0.5f;
        protected const float PitchControllerAngleBorderSin = 0.70710678118f; //Sin(45deg)

        protected float TimeEndError;
        protected HandHint hint;
        protected bool readyToCalibrate;

        protected float CurrentAngleBorder;

        protected State state = State.None;

        public override void Init(FinchCalibrationSettings settings)
        {
            base.Init(settings);
            state = State.InitialState;
            var targetMode = Internal.Settings.ReplyManager.GetArmsConnectedMode();
            Internal.Settings.ReplyManager.ChangeBodyRotationMode(targetMode);
            Internal.Settings.ReplyManager.ChangeUseConvergedPositions(targetMode);
            Internal.Settings.ReplyManager.SettingsReply += DoAfterInitialState;
            return;
        }

        private void Update()
        {
            if (state == State.WaitingForUserState)
            {
                HandleUpdate();
            }

            UpdatePosition();
        }

        protected void DoAfterInitialState(object obj, System.EventArgs args)
        {
            state = State.WaitingForUserState;
            settings.IsMomentalCalibration &= false;
            readyToCalibrate = false;

            if (settings.IsMomentalCalibration)
            {
                Calibrate();
            }
            else
            {
                TimeEndError = 0;
                HandleUpdate();
            }

            CurrentAngleBorder = CalibrationHorizontalError.AngleBorderSin;
        }

        protected void HandleUpdate()
        {
            UpdateChirality();
            UpdateSprite();
            TryCalibrate();
        }

        /// <summary>
        /// Starts calibration process
        /// </summary>
        protected virtual void TryCalibrate()
        {
            RingElement button = RingElement.HomeButton;

            readyToCalibrate &= Time.time > TimeEndError;
            readyToCalibrate |= !FinchController.GetPress(Chirality.Any, button);
            readyToCalibrate |= Time.time < TimeEndError && GetHandAngle() == HandHint.Base;

            if (FinchController.GetPressDown(FinchNodeManager.GetControllerConectionChirality(), button))
            {
                hint = GetHandAngle();
                if (hint != HandHint.Base)
                {
                    TimeEndError = Time.time + ErrorDuration;
                }
            }

            if (FinchController.GetPressTime(FinchNodeManager.GetControllerConectionChirality(), button) > settings.TimePressingToCallCalibration && Time.time > TimeEndError && readyToCalibrate)
            {
                if (Mathf.Abs((float)Internal.Calibration.Calculations.GetHmdPitchAngle()) * Mathf.Rad2Deg > 45)
                {
                    return;
                }

                hint = GetHandAngle();

                if (hint == HandHint.Base)
                {
                    Calibrate();
                }
                else
                {
                    TimeEndError = Time.time + ErrorDuration;
                }
            }
        }

        /// <summary>
        /// Calibrates Finch nodes
        /// </summary>
        protected virtual void Calibrate()
        {
            bool wasReverted = (Internal.Settings.ReplyManager.GetArmsConnectedMode() == ArmsConnected.OneArmSixDof) &&
                               UpperArmsOrientationsBinder.BindUpperArmOrientation();

            if (!wasReverted)
            {
                CalibrateByHmd();
            }
            else
            {
                state = State.WaitingForBindUpperArmOrientation;
                Internal.Calibration.ReplyManager.CalibrationReply += DoAfterWaitingForBindUpperArmOrientation;
            }
        }

        private void CalibrateByHmd()
        {
            //Load next step.
            FinchController.HapticPulse(NodeType.RightHand, FinchHapticPattern.LongClick);
            FinchController.HapticPulse(NodeType.LeftHand, FinchHapticPattern.LongClick);

            Internal.Calibration.ReplyManager.Calibration(Internal.FinchCore.Finch_CalibrationType.Hmd, Internal.FinchCore.Finch_CalibrationOptions.None);
            Internal.Calibration.ReplyManager.CalibrationReply += DoAfterWaitingForCalibration;
            state = State.WaitingForCalibration;
        }

        protected virtual void DoAfterWaitingForBindUpperArmOrientation(object obj, System.EventArgs args)
        {
            CalibrateByHmd();
        }

        protected void DoAfterWaitingForCalibration(object obj, System.EventArgs args)
        {
            state = State.None;
            NextStep();
        }

        /// <summary>
        /// Updates chirality states of FinchRing controllers
        /// </summary>
        protected void UpdateChirality()
        {
            if (FinchNodeManager.GetControllersCount() == 1 && (FinchController.LeftController.SwipeRight || FinchController.RightController.SwipeLeft))
            {
                FinchNodeManager.SwapNodes(NodeType.LeftHand, NodeType.RightHand);
            }

            bool differentRight = FinchController.RightController.IsConnected && FinchNodeManager.IsConnected(NodeType.LeftUpperArm);
            bool differentLeft = FinchController.LeftController.IsConnected && FinchNodeManager.IsConnected(NodeType.RightUpperArm);

            if (FinchNodeManager.GetUpperArmCount() == 1 && FinchNodeManager.GetControllersCount() == 1 && (differentRight || differentLeft))
            {
                FinchNodeManager.SwapNodes(NodeType.LeftUpperArm, NodeType.RightUpperArm);
            }
        }

        /// <summary>
        /// Updates all tutorial sprites of the step
        /// </summary>
        protected virtual void UpdateSprite()
        {
            bool bothController = (int)settings.Set % 10 == 2;
            bool errorConnect = (int)settings.Set % 10 != FinchNodeManager.GetControllersCount();
            bool wasLeftHint = CalibrateLeftHandWithArms.activeSelf || CalibrateLeftHandWithoutArms.activeSelf;

            bool haveUpperArms = (int)settings.Set / 10 > 0;
            bool leftConnected = !bothController && (errorConnect ? wasLeftHint : FinchController.LeftController.IsConnected);
            bool rightConnected =!bothController && (errorConnect ? !wasLeftHint : FinchController.RightController.IsConnected);

            if (haveUpperArms)
            {
                Notification.ID = bothController ? CalibrationPhraseId.CalibrateBothControllersWithUpperArms : CalibrationPhraseId.CalibrateOneControllerWithUpperArms;
            }
            else
            {
                Notification.ID = bothController ? CalibrationPhraseId.CalibrateBothControllersWithoutUpperArms : CalibrationPhraseId.CalibrateOneControllerWithoutUpperArms;
            }

            LeftSwapHint.SetActive(leftConnected);
            RightSwapHint.SetActive(rightConnected);

            if (GetHandAngle(CurrentAngleBorder) == HandHint.Base)
            {
                CurrentAngleBorder = angleBorderWide;
            }
            else
            {
                CurrentAngleBorder = CalibrationHorizontalError.AngleBorderSin;
            }

            bool handHintIsBase = (GetHandAngle(CurrentAngleBorder) == HandHint.Base);

            CalibrateBothHandsWithUpperArmsReady?.SetActive(bothController && haveUpperArms && handHintIsBase);
            CalibrateBothHandsWithoutUpperArms?.SetActive(bothController && !haveUpperArms);
            CalibrateBothHandsWithUpperArms?.SetActive(bothController && haveUpperArms && !handHintIsBase);
            CalibrateLeftHandWithArms?.SetActive(haveUpperArms && leftConnected && !handHintIsBase);
            CalibrateLeftHandWithoutArms?.SetActive((!haveUpperArms || handHintIsBase) && leftConnected);
            CalibrateRigthHandWithoutArms?.SetActive((!haveUpperArms || handHintIsBase) && rightConnected);
            CalibrateRigthHandWithArms?.SetActive(haveUpperArms && rightConnected && !handHintIsBase);

            if (Time.time > TimeEndError)
            {
                hint = HandHint.Base;
            }

            for (int i = 0; i < HorizontalError.Length; i++)
            {
                HorizontalError[i].SetActive((int)hint == i);
            }
        }

        /// <summary>
        /// Returns hint for hands angle errors with fixed threshold
        /// </summary>
        /// <returns></returns>
        protected virtual HandHint GetHandAngle()
        {
            return GetHandAngle(CalibrationHorizontalError.AngleBorderSin);
        }

        /// <summary>
        /// Returns hint for hands angle errors with custom threshold
        /// </summary>
        /// <param name="customAngle">Angle threshold</param>
        /// <returns></returns>
        protected HandHint GetHandAngle(float customAngle)
        {
            switch (Internal.Calibration.Calculations.GetNodesDirectionSign(customAngle, PitchControllerAngleBorderSin, RollAngleBorderSin))
            {
                case Internal.Calibration.ArmsDirection.Up:
                    return HandHint.LowerHand;

                case Internal.Calibration.ArmsDirection.Down:
                    return HandHint.UpperHand;

                case Internal.Calibration.ArmsDirection.LeanClockwise:
                case Internal.Calibration.ArmsDirection.LeanCounterClockwise:
                case Internal.Calibration.ArmsDirection.Different:
                    return HandHint.StraightHand;
            }

            return HandHint.Base;
        }

        protected static class UpperArmsOrientationsBinder
        {
            /// <summary>
            /// Binds upper arms orientation in calibration pose.
            /// Working only for case if hand and upper arm nodes with same chirality are internal recentered.
            /// </summary>
            /// <returns> True if reverting was changed, otherwise false </returns>
            public static bool BindUpperArmOrientation()
            {
                bool isLeftRevert = IsRevertedUpperArmOrientation(Chirality.Left);
                bool isRightRevert = IsRevertedUpperArmOrientation(Chirality.Right);

                if (isLeftRevert || isRightRevert)
                {
                    Internal.Calibration.ReplyManager.Calibration(Internal.FinchCore.Finch_CalibrationType.None, Internal.FinchCore.Finch_CalibrationOptions.CalibrateReverting);
                    return true;
                }

                return false;
            }

            private static bool IsRevertedUpperArmOrientation(Chirality chirality)
            {
                var upper = (chirality == Chirality.Right) ?
                            Internal.FinchCore.Finch_Node.RightUpperArm :
                            Internal.FinchCore.Finch_Node.LeftUpperArm;

                var hand = (chirality == Chirality.Right) ?
                           Internal.FinchCore.Finch_Node.RightHand :
                           Internal.FinchCore.Finch_Node.LeftHand;

                Quaternion rawUpper = Internal.FinchCore.Finch_GetNodeTPosedRotation(upper).ToUnity();
                Quaternion rawHand = Internal.FinchCore.Finch_GetNodeTPosedRotation(hand).ToUnity();
                bool isReverted = Internal.FinchCore.Finch_IsCalibrationReverted(upper) == 1;
                bool needReverted = RevertDetector.IsUpperArmReverted(rawUpper, rawHand);

                if (isReverted != needReverted)
                {
                    Internal.FinchCore.Finch_RevertCalibration(upper);
                }

                return isReverted != needReverted;
            }
        }
    }
}