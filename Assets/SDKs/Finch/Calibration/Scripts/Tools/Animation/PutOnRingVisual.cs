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

public class PutOnRingVisual : MonoBehaviour
{
    public GameObject FirstScreen;
    public GameObject SecondScreen;
    public GameObject Error;
    public GameObject Success;
    public GameObject StaticArrow;
    public GameObject StaticArrow2;
    public GameObject StaticArrow3;
    public NotificationCalibration Notification;
    public NotificationWords Header;

    public Chirality Chirality;

    [HideInInspector]
    public bool Complete;
    [HideInInspector]
    public bool Strated;

    private bool secondScreen = false;
    private bool error = false;
    private bool finished = false;

    public float TimeShowFirstScreen;
    public float TimeShowError;
    private float errorTimer;
    private float firstTimer;
    private float successTimer;

    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        if (Chirality == Chirality.Right)
            Init();
    }

    void Init()
    {
        Complete = Chirality == Chirality.Left && FinchNodeManager.GetControllersCount() == 1;
        FirstScreen.SetActive(false);
        SecondScreen.SetActive(false);
        Success.SetActive(false);
        Error.SetActive(false);

        successTimer = .0f;
        errorTimer = .0f;
        firstTimer = .0f;
        secondScreen = false;
        error = false;
        finished = false;
    }

    void Update()
    {
        StaticArrow.SetActive(FinchNodeManager.GetControllersCount() == 2);
        StaticArrow2.SetActive(FinchNodeManager.GetControllersCount() == 2);
        StaticArrow3.SetActive(FinchNodeManager.GetControllersCount() == 2);

        Header.Id = NotificationWord.PutOnRingHeader;

        if (FinchNodeManager.GetControllersCount() == 1)
        {
            Notification.ID = CalibrationPhraseId.PutOnRing;
        }
        else if (Chirality == Chirality.Left)
        {
            Notification.ID = CalibrationPhraseId.PutOnRingLeft;
        }
        else if (Chirality == Chirality.Right)
        {
            Notification.ID = CalibrationPhraseId.PutOnRingRight;
        }

        if (!Strated)
        {
            return;
        }

        if (TimeShowFirstScreen <= firstTimer && !secondScreen && !error)
        {
            FirstScreen.SetActive(false);
            SecondScreen.SetActive(true);
            secondScreen = true;
            firstTimer = .0f;
        }
        else if (!secondScreen && !error)
        {
            FirstScreen.SetActive(true);
            SecondScreen.SetActive(false);
            firstTimer += Time.deltaTime;
        }

        bool rightSensor = FinchInput.GetPress(Chirality == Chirality.Right ? NodeType.RightHand : NodeType.LeftHand, RingElement.CapacitySensor);

        NodeType leftNode = NodeType.LeftHand;
        NodeType rightNode = NodeType.RightHand;

        bool pressLeft = FinchInput.GetPressDown(leftNode, RingElement.HomeButton);
        bool pressRight = FinchInput.GetPressDown(rightNode, RingElement.HomeButton);

        if (pressLeft && Chirality == Chirality.Right)
        {
            FinchNodeManager.SwapNodes(leftNode, rightNode);
        }

        if (pressRight && Chirality == Chirality.Left)
        {
            return;
        }

        if (pressLeft || pressRight)
        {
            if (secondScreen && !error)
            {
                if (rightSensor)
                {
                    finished = true;
                }
                else
                {
                    secondScreen = false;
                    error = true;
                    SecondScreen.SetActive(false);
                }
            }
        }

        if (error && errorTimer < TimeShowError)
        {
            SecondScreen.SetActive(false);
            FirstScreen.SetActive(false);
            Error.SetActive(true);
            errorTimer += Time.deltaTime;
        }
        else if (errorTimer >= TimeShowError)
        {
            FirstScreen.SetActive(true);
            errorTimer = .0f;
            Error.SetActive(false);
            error = false;
            secondScreen = false;
        }

        if (finished)
        {
            successTimer += Time.deltaTime;
            Success.SetActive(true);
            Notification.ID = CalibrationPhraseId.Success;
            SecondScreen.SetActive(false);
            FirstScreen.SetActive(false);
            Error.SetActive(false);
        }

        if (successTimer > .5f)
        {
            Complete = true;
        }
    }
}