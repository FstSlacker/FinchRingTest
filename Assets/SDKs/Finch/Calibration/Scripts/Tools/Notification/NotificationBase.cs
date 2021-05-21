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

using UnityEngine;
using UnityEngine.UI;
using Finch;

public abstract class NotificationBase : MonoBehaviour
{
    [Header("Phrase option")]
    public int LayerOrder;

    [Header("Text options")]
    public FontStyle Style = FontStyle.Normal;
    public Color TextBaseColor = Color.white;
    public TextAnchor Anchors = TextAnchor.MiddleCenter;
    public int FontSize = 120;

    public bool UseCustomLineSpacing;
    public float LineSpacing = 1;

    public const float ScaleFactor = 0.001f;

    public Vector2 TextSize
    {
        get
        {
            return new Vector2(LayoutUtility.GetPreferredWidth(text.rectTransform), LayoutUtility.GetPreferredHeight(text.rectTransform));
        }
    }

    protected virtual string idPhrase { get { return ""; } }

    private static bool updateCanvas;
    [HideInInspector] public Text text;
    private RectTransform textTransform;

    private void Awake()
    {
        if (FinchCalibration.NotificationRenderMode == RenderMode.WorldSpace)
        {
            Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
            canvas.hideFlags = HideFlags.HideInHierarchy;
            canvas.sortingOrder = LayerOrder;
            canvas.transform.SetParent(transform);
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.localPosition = Vector3.zero;
            canvas.transform.localRotation = Quaternion.identity;

            text = new GameObject("Text").AddComponent<Text>();
            textTransform = text.GetComponent<RectTransform>();

            text.transform.SetParent(canvas.transform);
        }
        else
        {
            text = new GameObject("Text").AddComponent<Text>();
            textTransform = text.GetComponent<RectTransform>();

            text.transform.SetParent(transform);
        }

        text.transform.localPosition = Vector3.zero;
        text.transform.localRotation = Quaternion.identity;
        text.transform.localScale = Vector3.one * ScaleFactor;
        text.fontStyle = Style;

        if (UseCustomLineSpacing)
        {
            text.lineSpacing = LineSpacing;
        }

        ContentSizeFitter sizeFitter = text.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        UpdateTextPivot();
    }

    private void Update()
    {
        string phrase = TextImporter.GetPhrase(idPhrase);
        updateCanvas = text.text != phrase;

        text.text = phrase;
        text.color = TextBaseColor;
        text.font = TextImporter.DefaultFont;
        text.fontSize = FontSize;
        text.alignment = Anchors;
        text.fontStyle = Style;
        text.rectTransform.sizeDelta = TextSize;

        if (UseCustomLineSpacing)
        {
            text.lineSpacing = LineSpacing;
        }

        UpdateTextPivot();
    }

    private void UpdateTextPivot()
    {
        float x = 0.5f;
        float y = 0.5f;

        if (Anchors == TextAnchor.UpperLeft || Anchors == TextAnchor.MiddleLeft || Anchors == TextAnchor.LowerLeft)
        {
            x = 0;
        }

        if (Anchors == TextAnchor.UpperRight || Anchors == TextAnchor.MiddleRight || Anchors == TextAnchor.LowerRight)
        {
            x = 1;
        }


        if (Anchors == TextAnchor.LowerLeft || Anchors == TextAnchor.LowerCenter || Anchors == TextAnchor.LowerRight)
        {
            y = 0;
        }

        if (Anchors == TextAnchor.UpperLeft || Anchors == TextAnchor.UpperCenter || Anchors == TextAnchor.UpperRight)
        {
            y = 1;
        }

        textTransform.pivot = new Vector2(x, y);
    }

    private void LateUpdate()
    {
        if (updateCanvas)
        {
            updateCanvas = false;
            Canvas.ForceUpdateCanvases();
        }
    }
}
