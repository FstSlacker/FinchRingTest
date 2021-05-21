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

public class RingHintStep : TutorialStep
{
    /// <summary>
    /// Hint for 3DoF calibration
    /// </summary>
    public GameObject ThreeDofHint;

    /// <summary>
    /// Hint for 6DoF calibration
    /// </summary>
    public GameObject SixDofHint;

    /// <summary>
    /// Text on the overlay
    /// </summary>
    public NotificationCalibration Notification;

    /// <summary>
    /// Header text of the overlay
    /// </summary>
    public NotificationWords Header;

    protected RecalibrationState recalibration = new RecalibrationState();

    private bool oncePass;

    public override void Init(FinchCalibrationSettings calibrationSettings)
    {
        base.Init(calibrationSettings);

        if (oncePass)
        {
            NextStep();
        }
        else
        {
            recalibration.Update(settings, true);
            Update();
        }
    }

    private void Update()
    {
        UpdatePosition();

        VisualUpdate();

        recalibration.Update(settings);

        NotificationUpdate();

        StatusUpdate();
    }

    /// <summary>
    /// Updates states of visual tutorials of the step
    /// </summary>
    protected void VisualUpdate()
    {
        ThreeDofHint.SetActive((int)settings.Set / 10 == 0);
        SixDofHint.SetActive((int)settings.Set / 10 > 0);
    }

    /// <summary>
    /// Updates states of notifications of the step
    /// </summary>
    protected void NotificationUpdate()
    {
        Header.Id = NotificationWord.Calibration;

        if ((int)settings.Set / 10 == 0)
        {
            Notification.ID = CalibrationPhraseId.ControllerRecalibration;
        }
        else if ((int)settings.Set / 10 > 0)
        {
            Notification.ID = CalibrationPhraseId.UpperArmRecalibration;
        }
    }

    /// <summary>
    /// Updates status of step activity
    /// </summary>
    protected void StatusUpdate()
    {
        if (recalibration.RecalibrationAvailable)
        {
            oncePass = true;
            NextStep();
        }
    }
}
