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

/// <summary>
/// Step for checking that the FinchRing is on the user's finger
/// </summary>
public class PutOnFinchRingStep : TutorialStep
{
    /// <summary>
    /// Right ring tutorial visualisation
    /// </summary>
    public PutOnRingVisual Right;

    /// <summary>
    /// Left ring tutorial visualisation
    /// </summary>
    public PutOnRingVisual Left;

    private void Start()
    {
        FastCheck();
        Init();
    }

    private void OnEnable()
    {
        FastCheck();
        Init();
    }

    protected void Init()
    {
        Right.gameObject.SetActive(true);
        Right.Strated = true;
        Left.gameObject.SetActive(false);
        Right.Complete = false;
    }

    private void Update()
    {
        UpdatePosition();
        StatusUpdate();
    }

    /// <summary>
    /// Updates status of nodes capacitive sensors
    /// </summary>
    protected void StatusUpdate()
    {
        if (Right.Complete && Left.Complete)
        {
            NextStep();
        }
        else if (Right.Complete && !Left.Complete)
        {
            Left.gameObject.SetActive(true);
            Left.Strated = true;
            Right.gameObject.SetActive(false);
        }
    }

    protected void FastCheck()
    {
        bool oneHandReady = (FinchInput.GetPress(NodeType.RightHand, RingElement.CapacitySensor) || FinchInput.GetPress(NodeType.LeftHand, RingElement.CapacitySensor))
            && (FinchNodeManager.GetControllersCount() == 1);

        if (oneHandReady)
        {
            NextStep();
        }
    }
}
