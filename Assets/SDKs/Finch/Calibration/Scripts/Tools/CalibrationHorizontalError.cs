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
using System;

public class CalibrationHorizontalError : MonoBehaviour
{
    [Header("Animated hands")]
    public AnimatedTransform AnimationHands;
    public Vector3 HandsStartPosition;
    public Vector3 HandsEndPosition;

    [Header("Arrows")]
    public Transform HorizontalArrows;
    public Transform DiagonalArrows;

    [Header("Notification")]
    public NotificationHorizontalError Notification;

    public const float AngleBorderSin = 0.3f;

    private const float RollAngleBorderSin = 0.5f;
    private const float PitchControllerAngleBorderSin = 0.70710678118f; //Sin(45deg)

    private void OnEnable()
    {
        AnimationHands.FirstPosition = HandsStartPosition;

        switch (Finch.Internal.Calibration.Calculations.GetNodesDirectionSign(AngleBorderSin, PitchControllerAngleBorderSin, RollAngleBorderSin))
        {
            case Finch.Internal.Calibration.ArmsDirection.Up:
                Notification.ID = HorizontalErrorPhrases.HorizontalUp;
                DiagonalArrows.gameObject.SetActive(false);
                HorizontalArrows.gameObject.SetActive(true);
                HorizontalArrows.localEulerAngles = new Vector3(180, 0, 0);
                HandsEndPosition = new Vector3(Math.Abs(HandsEndPosition.x), Math.Abs(HandsEndPosition.y), Math.Abs(HandsEndPosition.z));
                AnimationHands.SecondPosition = HandsEndPosition;
                break;

            case Finch.Internal.Calibration.ArmsDirection.Down:
                Notification.ID = HorizontalErrorPhrases.HorizontalDown;
                DiagonalArrows.gameObject.SetActive(false);
                HorizontalArrows.gameObject.SetActive(true);
                HorizontalArrows.localEulerAngles = new Vector3(0, 0, 0);
                HandsEndPosition = new Vector3(-Math.Abs(HandsEndPosition.x), -Math.Abs(HandsEndPosition.y), -Math.Abs(HandsEndPosition.z));
                AnimationHands.SecondPosition = HandsEndPosition;
                break;

            case Finch.Internal.Calibration.ArmsDirection.Different:
                Notification.ID = HorizontalErrorPhrases.DifferentSign;
                DiagonalArrows.gameObject.SetActive(true);
                HorizontalArrows.gameObject.SetActive(false);
                AnimationHands.SecondPosition = HandsStartPosition;
                break;
        }

        AnimationHands.ResetState(false);
    }
}
