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
public class Border : MonoBehaviour
{
    public float BorderWidth = 0.05f;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sprite.material.SetFloat("_BorderWidth", BorderWidth);
        sprite.material.SetFloat("_RatioX", transform.localScale.x / Mathf.Max(transform.localScale.x, transform.localScale.y));
        sprite.material.SetFloat("_RatioY", transform.localScale.y / Mathf.Max(transform.localScale.x, transform.localScale.y));
    }
}
