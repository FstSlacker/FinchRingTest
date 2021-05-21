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
    /// Base step to provide Finch connection class
    /// </summary>
    public abstract class ConnectionBaseStep : TutorialStep
    {
        protected bool loadNextStep = false;
        protected int controllersConnected = 0;
        protected int upperArmConnected = 0;

        private const float timeToStartScanner = 1.5f;
        protected bool timeStampsError;

        /// <summary>
        /// Part before the main connection part - for reminders and warnings
        /// </summary>
        public GameObject PrePart;

        /// <summary>
        /// Connection part
        /// </summary>
        public GameObject CommonPart;

        /// <summary>
        /// Activated in case of errors during connection
        /// </summary>
        public GameObject Error;

        protected override void NextStep(bool playSound = true)
        {
            Internal.Scanner.Stop();
            base.NextStep(playSound);
        }

        private void Start()
        {
            Internal.FinchCore.OnDisconnected += OnDisconnectNode;
        }

        private void OnDisconnectNode(NodeType node)
        {
            Internal.Scanner.DropState();
        }

        /// <summary>
        /// Updates states of Finch connection
        /// </summary>
        protected void UpdateState(ChangeState obj, bool state, bool force, bool activeOld)
        {
            //Reset animation appear/dissappear without collision.
            if (activeOld && state && !force)
            {
                obj.FinishState = false;
                obj.ResetState(true);
                return;
            }

            if (obj.FinishState != state || force)
            {
                obj.FinishState = state;
                obj.ResetState(!obj.gameObject.activeInHierarchy || force);
            }
        }

        /// <summary>
        /// Updates scanner status
        /// </summary>
        protected void UpdateScanner(PlayableSet maxSet)
        {
            //Remember first connected node or last active
            controllersConnected = Mathf.Min((int)maxSet % 10, FinchNodeManager.GetControllersCount());
            upperArmConnected = Mathf.Min((int)maxSet / 10, FinchNodeManager.GetUpperArmCount());

            if (Time.time > timeToStartScanner && !loadNextStep)
            {
                Internal.Scanner.Run(FinchNodeManager.ScannerType);
            }
            else if (loadNextStep)
            {
                Internal.Scanner.Stop();
            }
        }

        /// <summary>
        /// Updates status on node timestamps
        /// </summary>
        protected void CheckTimeStamps()
        {
            if (FinchNodeManager.GetUpperArmCount() == 0)
            {
                timeStampsError = false;
                NextStep();
                return;
            }

            ulong rightHandTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.RightHand);
            ulong leftHandTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.LeftHand);
            ulong rightUpperArmTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.RightUpperArm);
            ulong leftUpperArmTimeStamp = Internal.FinchNodeManager.GetNodeTimeStamp(NodeType.LeftUpperArm);

            Debug.Log($"ConnectionBaseStep: rhTs = {rightHandTimeStamp}; ruaTs = {rightUpperArmTimeStamp}; lhTs = {leftHandTimeStamp}; luaTs = {leftUpperArmTimeStamp}");

            if (Internal.FinchNodeManager.CheckTimeStamps())
            {
                timeStampsError = false;
                NextStep();
            }
            else
            {
                timeStampsError = true;
            }
        }
    }
}
