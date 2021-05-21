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

public enum HandType
{
    Hand = 0,
    Controller = 1,
    ControllerWithArmBand = 2
}

public class AnimatedHands : MonoBehaviour
{
    [Header("Sprite options")]
    public AnimatedSprites LeftHand;
    public AnimatedSprites RightHand;

    public Sprite HandSprite;
    public Sprite ControllerSprite;
    public Sprite ControllerWithArmBandSprite;

    public HandType LeftHandType = HandType.Controller;
    public HandType RightHandType = HandType.Controller;

    private void OnEnable()
    {
        Update();
    }

    void Update()
    {
        LeftHand.Sprite.sprite = GetSprite(LeftHandType);
        RightHand.Sprite.sprite = GetSprite(RightHandType);
    }

    public void SetAnimationColor(AnimatedColor[] leftColors, AnimatedColor[] rightColors)
    {
        if (!LeftHand.SameAnimations(leftColors) || !RightHand.SameAnimations(rightColors))
        {
            LeftHand.SetAnimatedColors(leftColors);
            RightHand.SetAnimatedColors(rightColors);
        }
    }

    private Sprite GetSprite(HandType handType)
    {
        switch (handType)
        {
            case HandType.ControllerWithArmBand:
                return ControllerWithArmBandSprite;

            case HandType.Controller:
                return ControllerSprite;

            default:
                return HandSprite;
        }
    }
}
