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
using UnityEngine.UI;

public class SliderChanger : MonoBehaviour
{
    public Transform HeadPoser;
    public Slider SliderRight;
    public Slider SliderLeft;

    public GameObject RightWrong;
    public GameObject LeftWrong;
    public GameObject RightNormal;
    public GameObject LeftNormal;

    private const float centerPos = .15f;//.18f;

    public float headRotation;

    public bool isWrong;

    void Update()
    {
        float headRotationXInRadDiv2 = -getPitchAngleSin(HeadPoser.rotation) * 0.707f;

        headRotation = headRotationXInRadDiv2 - centerPos;

        SliderRight.value = (headRotation * (-6.4f));
        SliderLeft.value = (headRotation * (-6.4f));

        isWrong = (headRotation > .09f) || (headRotation < -.09f);

        RightWrong.SetActive(isWrong);
        LeftWrong.SetActive(isWrong);
        RightNormal.SetActive(!isWrong);
        LeftNormal.SetActive(!isWrong);
    }

    private static float getPitchAngleSin(Quaternion rot)
    {
        return (rot * Vector3.forward).y;
    }
}
