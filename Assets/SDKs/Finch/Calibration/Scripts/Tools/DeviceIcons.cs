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

[System.Serializable]
public class DeviceIcons
{
    private enum IconsAction
    {
        Dissappear,
        Moving,
        Appear,
        ChangeState
    }

    public Vector3 IconsCenter;
    public float IconDistance;
    public AnimatedContainer[] Icons = new AnimatedContainer[4];

    public AnimatedColor ConnectedColor;
    public AnimatedColor DisconnectedColor;

    public AnimatedContainer Pointer;

    private IconsAction currentAction = IconsAction.Dissappear;

    private int controllersNeed;
    private int upperArmNeed;
    private int prevPointerId = 0;

    public void ResetIcons(int controllers, int upperArm, int controllersConnected, int upperArmConnected, bool force)
    {
        if (force)
        {
            controllersNeed = controllers;
            upperArmNeed = upperArm;
            ChangeConnectionState(controllersConnected, upperArmConnected, true);
        }

        Dissappear(true, controllersConnected, upperArmConnected);
        UpdateTransform(true, controllersConnected, upperArmConnected);
        Appear(true, controllersConnected, upperArmConnected);

        currentAction = IconsAction.Dissappear;
        controllersNeed = controllers;
        upperArmNeed = upperArm;
    }

    public void Update(int controllersCount, int upperArmCount)
    {
        foreach (var i in Icons)
        {
            if (!i.AnimationPass)
            {
                return;
            }
        }

        switch (currentAction)
        {
            case IconsAction.Dissappear:
                Dissappear(false, controllersCount, upperArmCount);
                break;

            case IconsAction.Appear:
                Appear(false, controllersCount, upperArmCount);
                break;

            case IconsAction.Moving:
                UpdateTransform(false, controllersCount, upperArmCount);
                break;

            case IconsAction.ChangeState:
                ChangeConnectionState(controllersCount, upperArmCount, false);
                break;
        }
    }

    private void Dissappear(bool force, int controllersConnected, int upperArmsConnected)
    {
        bool dissappearEnded = true;
        
        for (int i = 0; i < 4; i++)
        {
            bool endState = i < 2 && i % 2 < controllersNeed || i >= 2 && i % 2 < upperArmNeed;

            if (!endState && Icons[i].State.FinishState)
            {
                Icons[i].State.FinishState = false;
                Icons[i].State.ResetState(force);
                dissappearEnded = false;
            }
        }

        if (!dissappearEnded && Pointer.State.FinishState)
        {
            Pointer.State.FinishState = false;
            Pointer.State.ResetState(force);
            dissappearEnded = false;
        }

        if (dissappearEnded)
        {
            currentAction = IconsAction.Moving;
        }
    }

    private void UpdateTransform(bool force, int controllersConnected, int upperArmsConnected)
    {
        bool transformEnded = true;
        Vector3 position = IconsCenter - Vector3.right * (controllersNeed + upperArmNeed - 1) * 0.5f * IconDistance;

        for (int i = 0; i < 4; i++)
        {
            if (i < controllersNeed + upperArmNeed)
            {
                int id = i < controllersNeed ? i : 2 + (i - controllersNeed) % 2;

                if (Icons[id].Transform.SecondPosition != position)
                {
                    Icons[id].Transform.FirstPosition = Icons[id].Transform.SecondPosition;
                    Icons[id].Transform.SecondPosition = position;
                    Icons[id].Transform.ResetState(force);
                    transformEnded = false;
                }

                position += IconDistance * Vector3.right;
            }
        }

        int pointerId = GetPoiterIconId(controllersConnected, upperArmsConnected);
        Vector3 pointerPosition = new Vector3(Icons[pointerId].Transform.SecondPosition.x,
                                              Pointer.transform.localPosition.y,
                                              Pointer.transform.localPosition.z);
        Pointer.Transform.FirstPosition = Pointer.Transform.SecondPosition = pointerPosition;

        if (transformEnded)
        {
            currentAction++;
        }
    }

    private void Appear(bool force, int controllersConnected, int upperArmsConnected)
    {
        bool appearEnded = true;

        for (int i = 0; i < 4; i++)
        {
            bool endState = i < 2 && i % 2 < controllersNeed || i >= 2 && i % 2 < upperArmNeed;

            if (endState && !Icons[i].State.FinishState)
            {
                Icons[i].State.FinishState = true;
                Icons[i].State.ResetState(force);
                appearEnded = false;
            }
        }

        bool state = controllersNeed + upperArmNeed > 1 && (controllersNeed > controllersConnected || upperArmNeed > upperArmsConnected);

        if (state && !Pointer.State.FinishState)
        {
            Pointer.State.FinishState = true;
            Pointer.State.ResetState(force);
            appearEnded = false;
        }

        if (appearEnded)
        {
            currentAction++;
        }
    }

    private void ChangeConnectionState(int controllers, int upperArm, bool force)
    {
        foreach (var i in Icons)
        {
            if (!i.AnimationPass)
            {
                return;
            }
        }

        Icons[0].Color.SetAnimatedColors(controllers > 0 ? ConnectedColor : DisconnectedColor, force);
        Icons[1].Color.SetAnimatedColors(controllers > 1 ? ConnectedColor : DisconnectedColor, force);
        Icons[2].Color.SetAnimatedColors(upperArm > 0 ? ConnectedColor : DisconnectedColor, force);
        Icons[3].Color.SetAnimatedColors(upperArm > 1 ? ConnectedColor : DisconnectedColor, force);

        int pointerId = GetPoiterIconId(controllers, upperArm);
        Vector3 pointerPosition = new Vector3(Icons[pointerId].Transform.SecondPosition.x, 
                                              Pointer.transform.localPosition.y, 
                                              Pointer.transform.localPosition.z);

        if (Pointer.Transform.SecondPosition.x != pointerPosition.x)
        {
            Pointer.Transform.FirstPosition = Pointer.Transform.SecondPosition;
            Pointer.Transform.SecondPosition = pointerPosition;
            Pointer.Transform.ResetState(!Pointer.State.FinishState && Pointer.State.AnimationPass);
        }

        bool state = controllersNeed + upperArmNeed > 1 && (controllersNeed > controllers || upperArmNeed > upperArm);

        if (Pointer.State.FinishState != state)
        {
            Pointer.State.FinishState = state;
            Pointer.State.ResetState(false);
        }
    }

    private int GetPoiterIconId(int controllersConnected, int upperArmsConnected)
    {
        if (IsConnectedNode(prevPointerId, controllersConnected, upperArmsConnected))
        {
            for (int i = 0; i < controllersNeed + upperArmNeed; i++)
            {
                int id = (i + prevPointerId) % (controllersNeed + upperArmNeed);

                if (!IsConnectedNode(id, controllersConnected, upperArmsConnected))
                {
                    prevPointerId = id;
                    break;
                }
            }
        }

        return prevPointerId < controllersNeed ? prevPointerId : 2 + (prevPointerId - controllersNeed) % 2;
    }

    private bool IsConnectedNode(int id, int controllersConnected, int upperArmsConnected)
    {
        return id < controllersNeed ? (controllersConnected > id) : 
                                      (upperArmsConnected > id - controllersNeed);
    }
}