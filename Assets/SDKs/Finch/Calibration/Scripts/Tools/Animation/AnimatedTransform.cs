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

using System.Collections.Generic;
using UnityEngine;
using Finch;

public class AnimatedTransform : MonoBehaviour
{
    [Header("Graphic")]
    public bool AnimatedOnStart = true;
    public bool Loop = false;
    public bool UseRotation = false;

    public bool AnimationPass { get { return path >= 1; } }

    public AnimationCurve PositionsCurve = new AnimationCurve(new Keyframe[]
    {
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    });

    [Header("Positions")]
    public Vector3 FirstPosition;
    public Vector3 SecondPosition;

    [Header("Timers")]
    public float TimeToMove = 2.0f;

    private float path { get { return (Time.time - startTime) / TimeToMove; } }
    private float startTime;

    private void OnEnable()
    {
        ResetState(!AnimatedOnStart);
    }

    public void ResetState(bool force)
    {
        startTime = force ? -100 : Time.time;
        Update();
    }

    private void Update()
    {
        float lerpPath = PositionsCurve.Evaluate(Mathf.Clamp01(Loop ? path % 1 : path));
        Vector3 value = Vector3.Lerp(FirstPosition, SecondPosition, lerpPath);

        if (UseRotation)
        {
            transform.localEulerAngles = value;
        }
        else
        {
            transform.localPosition = value;
        }
    }
}
