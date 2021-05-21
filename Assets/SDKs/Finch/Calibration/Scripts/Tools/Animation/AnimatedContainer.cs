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

public class AnimatedContainer : MonoBehaviour
{
    public bool AutoStart = true;
    public float Pause = 0.5f;

    [HideInInspector]
    public AnimatedTransform Transform;
    [HideInInspector]
    public ChangeState State;
    [HideInInspector]
    public AnimatedSprites Color;

    private bool waitInit;
    private bool loop;
    private float timeToStart;

    public bool AnimationPass
    {
        get
        {
            return (Transform == null || Transform.AnimationPass) &&
                (State == null || State.AnimationPass) &&
                (Color == null || Color.AnimationPass);
        }
    }

    private void Start ()
    {
        Transform = GetComponent<AnimatedTransform>();
        State = GetComponent<ChangeState>();
        Color = GetComponent<AnimatedSprites>();

        if (Transform != null)
        {
            loop |= Transform.Loop;
        }

        if (Color != null)
        {
            loop |= Color.Loop;
            Color.Loop = false;
        }
    }

    private void Update()
    {
        if (State == null || !State.AnimationPass || State.JustReset)
        {
            timeToStart = Time.time + Pause;
            waitInit = true;
        }

        if (AutoStart && waitInit && Time.time < timeToStart)
        {
            Transform?.ResetState(true);
            Color?.ResetState(true);
            if (Transform != null)
            {
                Transform.Loop = false;
            }
            if (Color != null)
            {
                Color.Loop = false;
            }
        }

        if (AutoStart && waitInit && Time.time > timeToStart)
        {
            waitInit = false;
            Transform?.ResetState(false);
            Color?.ResetState(false);

            if (Transform != null)
            {
                Transform.Loop = loop;
            }

            if (Color != null)
            {
                Color.Loop = loop;
            }
        }
    }
}
