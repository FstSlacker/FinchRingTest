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

public enum CalibrationPhraseId
{
    BindUpperArmChirality,
    BindUpperArmOrientation,
    BindControllerChirality,
    NextButton,
    ConnectNodes,
    ChangeSetButton,
    CalibrateOneControllerWithoutUpperArms,
    Success,
    CalibrateLeftDash,
    CalibrateRightDash,
    WaitingForConnection,
    ReadyConnect,
    UpperArmRecalibration,
    ControllerRecalibration,
    SwapToLeft,
    SwapToRight,
    ConnectUpperArms,
    ConnectShift,
    BindRingChirality,
    CalibrateBothControllersWithoutUpperArms,
    CalibrateOneControllerWithUpperArms,
    CalibrateBothControllersWithUpperArms,
    PutOnRing,
    PutOnRingRight,
    PutOnRingLeft,
    Put,
    Open,

}

public class NotificationCalibration : NotificationBase
{
    [Header("Phrase")]
    public CalibrationPhraseId ID = CalibrationPhraseId.BindUpperArmChirality;

    protected override string idPhrase => ID.ToString();
}
