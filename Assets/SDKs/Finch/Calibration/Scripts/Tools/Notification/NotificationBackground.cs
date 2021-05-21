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
using UnityEngine.UI;

[RequireComponent(typeof(NotificationBase))]
public class NotificationBackground : MonoBehaviour
{
    public enum OffsetType
    {
        Absolute,
        TextOffset
    }

    public enum LeaderType
    {
        OnlyScale,
        ScaleAndPosition
    }

    [Header("BackGround color")]
    public Color BackGroundColor = new Color(0, 0, 0, 0);

    [Header("BackGround size")]
    public OffsetType XOffset = OffsetType.TextOffset;
    public OffsetType YOffset = OffsetType.TextOffset;

    public bool UseMinSize = true;
    public bool UseMaxSize = true;

    public Vector2 MinSize = new Vector2(400, 80);
    public Vector2 MaxSize = new Vector2(4000, 800);
    public Vector2 OffsetText = new Vector2(400, 80);

    public Vector2 BackgroudSize { get; private set; }

    private NotificationBase notification;

    private Image bg;

    void Start ()
    {
        notification = GetComponent<NotificationBase>();

        bg = new GameObject("BackGround").AddComponent<Image>();
        bg.transform.SetParent(transform.GetChild(0));
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localRotation = Quaternion.identity;
        bg.transform.localScale = Vector3.one * NotificationBase.ScaleFactor;
        bg.material = null;
    }

    void Update()
    {
        Vector2 originalSize = OffsetText;

        if (XOffset == OffsetType.TextOffset)
        {
            originalSize += Vector2.right * notification.TextSize.x;
        }

        if (YOffset == OffsetType.TextOffset)
        {
            originalSize += Vector2.up * notification.TextSize.y;
        }

        if (UseMinSize)
        {
            originalSize = new Vector2(Mathf.Max(MinSize.x, originalSize.x), Mathf.Max(MinSize.y, originalSize.y));
        }

        if (UseMaxSize)
        {
            originalSize = new Vector2(Mathf.Min(MaxSize.x, originalSize.x), Mathf.Min(MaxSize.y, originalSize.y));
        }

        bg.rectTransform.sizeDelta = BackgroudSize = originalSize;
    }
}
