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

[System.Serializable]
public struct SpriteIteration
{
    public GameObject SpriteRight;
    public GameObject SpriteLeft;
    public GameObject RightDisconnected;
    public GameObject LeftDisconnected;

    public float PhaseTime;
}

public class AnimatedSilhuette : MonoBehaviour
{
    public SpriteIteration[] iterations;

    private float timer;
    private int iter;

    private void Start()
    {
        iter = 0;
        timer = .0f;
    }

    private void OnEnable()
    {
        iter = 0;
        timer = .0f;
        Update();
    }

    private void Update ()
    {
        timer += Time.deltaTime;

        bool isSixDof = FinchNodeManager.GetUpperArmCount() > 0;

        if (timer >= iterations[iter].PhaseTime)
        {
            iterations[iter].SpriteLeft.SetActive(false);
            iterations[iter].SpriteRight.SetActive(false);
            iterations[iter].RightDisconnected.SetActive(false);
            iterations[iter].LeftDisconnected.SetActive(false);

            timer = .0f;
            if (iter == iterations.Length-1)
            {
                iter = 0;
            }
            else
            {
                ++iter;
            }
        }
        else
        {
            iterations[iter].SpriteLeft.SetActive(FinchController.LeftController.IsConnected && isSixDof);
            iterations[iter].SpriteRight.SetActive(FinchController.RightController.IsConnected && isSixDof);
            iterations[iter].RightDisconnected.SetActive(!FinchController.RightController.IsConnected && isSixDof);
            iterations[iter].LeftDisconnected.SetActive(!FinchController.LeftController.IsConnected && isSixDof);
        }
    }
}
