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

[RequireComponent(typeof(SpriteRenderer))]
public class ChangeState : MonoBehaviour
{
    public bool AnimatedOnStart = true;
    public bool FinishState = true;
    public float AppearTime = 0.5f;

    public bool AnimationPass { get { return path >= 1; } }
    public bool JustReset { get { return Time.time - lastReset < 0.1f; } }
    [HideInInspector]
    public SpriteRenderer Sprite;

    private Color spriteColor;
    private float timeInit;
    private float lastReset;

    private float path { get { return Mathf.Clamp01((Time.time - timeInit) / AppearTime); } }
    void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        ResetState(!AnimatedOnStart);
    }

    public void ResetState(bool force)
    {
        lastReset = Time.time;
        timeInit = force ? -100 : Time.time;
        Update();
    }

    void Update()
    {
        if (Sprite == null)
        {
            return;
        }

        spriteColor = Sprite.color;
        spriteColor.a = FinishState ? path : 1 - path;

        Sprite.color = spriteColor;
    }
}
