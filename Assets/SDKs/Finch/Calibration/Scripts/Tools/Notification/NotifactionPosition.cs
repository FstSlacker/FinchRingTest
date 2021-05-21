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

[RequireComponent(typeof(NotificationBase))]
public class NotifactionPosition : MonoBehaviour
{
    public NotificationBase NotificationLeader;
    public TextAnchor PositionAnchors = TextAnchor.LowerCenter;
    public float PositionOffset = 0;

    private NotificationBase notification;
    private bool inited;

    private void Start()
    {
        notification = GetComponent<NotificationBase>();
        inited = true;
    }

    private void OnEnable()
    {
        if (inited)
        {
            Update();
        }
    }

    void Update()
    {
        Vector2 direction = new Vector2((int)PositionAnchors % 3 - 1, 1 - Mathf.FloorToInt((int)PositionAnchors / 3));
        Vector2 distance = (NotificationLeader.TextSize + notification.TextSize) * NotificationBase.ScaleFactor * 0.5f;
        distance = new Vector2(distance.x * direction.x, distance.y * direction.y);

        Vector2 delta = distance + distance.normalized * PositionOffset;
        Vector3 position = NotificationLeader.transform.position + new Vector3(delta.x, delta.y, 0);

        if (!float.IsNaN(position.magnitude))
        {
            transform.position = position;
        }
    }
}
