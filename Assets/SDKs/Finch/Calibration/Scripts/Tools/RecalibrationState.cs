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
using Finch;

public class RecalibrationState
{
    public bool RecalibrationAvailable { get { return recalibrationAvailable; } }

    private bool recalibrationAvailable;
    private bool readyCalibrate;
    private bool[] ringCorrectAngle = new bool[2];

    public void Update(FinchCalibrationSettings settings, bool resetState = false)
    {
        readyCalibrate |= !FinchController.GetPress(SetSettings(settings, false), RingElement.HomeButton);
        readyCalibrate &= !resetState;

        recalibrationAvailable = readyCalibrate && (IsUpperArmAvailable(settings) ||
                                 IsRingAvailable(FinchController.LeftController, settings) || IsRingAvailable(FinchController.RightController, settings));
    }

    private bool IsUpperArmAvailable(FinchCalibrationSettings settings)
    {
        bool useUpperArm = FinchNodeManager.GetUpperArmCount() > 0;
        return useUpperArm && FinchTracker.GetPressTime(SetSettings(settings, true), RingElement.HomeButton) > settings.TimePressingToCallCalibration;
    }

    private bool IsRingAvailable(FinchController controller, FinchCalibrationSettings settings)
    {
        bool useRing = FinchNodeManager.GetUpperArmCount() == 0;

        NodeType controllerNode = (NodeType)controller.Chirality;
        Vector3 fwd = controllerNode == NodeType.RightHand ? Vector3.right : Vector3.left;
        Quaternion rotation = Finch.Internal.FinchCore.Finch_GetNodeTPosedRotation((Finch.Internal.FinchCore.Finch_Node)controllerNode).ToUnity();
        float angle = (rotation * fwd).y;

        ringCorrectAngle[(int)controller.Chirality] |= !FinchController.GetPress(SetSettings(settings, false), RingElement.HomeButton);
        ringCorrectAngle[(int)controller.Chirality] &= angle < -0.8f;

        float ringTime = FinchController.GetPressTime(controller.Chirality, RingElement.HomeButton);

        return useRing && ringCorrectAngle[(int)controller.Chirality] && ringTime > settings.TimePressingToCallCalibration;
    }

    private Chirality SetSettings(FinchCalibrationSettings settings, bool isUpperArm)
    {
        bool twoNodes = isUpperArm ? FinchNodeManager.GetUpperArmCount() > 1 : FinchNodeManager.GetControllersCount() > 1;

        return twoNodes ? settings.RecalibrationConfiguration : Chirality.Any;
    }
}
