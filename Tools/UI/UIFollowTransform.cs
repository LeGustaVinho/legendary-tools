﻿using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIFollowTransform : MonoBehaviour
{
    public enum UpdateModeType { Update, LateUpdate }
    public UpdateModeType UpdateMode;
    public Camera Camera;
    public Canvas Canvas;
    public Transform Target;
    public Vector2 Offset;

    private Vector3 targetScreenPointPosition;
    private Vector3 worldPointInRectangle;

    private RectTransform RectTransform;
    private RectTransform CanvasRectTransform;

    bool IsNotCanvasOverlay
    {
        get { return Canvas != null && Canvas.renderMode != RenderMode.ScreenSpaceOverlay; }
    }

    void Awake()
    {
        Cache();
    }

    void Cache()
    {
        RectTransform = GetComponent<RectTransform>();
        if(Canvas != null)
            CanvasRectTransform = Canvas.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (UpdateMode == UpdateModeType.Update)
            Follow();
    }

    void LateUpdate()
    {
        if (UpdateMode == UpdateModeType.LateUpdate)
            Follow();
    }

    public void Follow()
    {
        if (Target == null || Canvas == null)
            return;

        if (Canvas.renderMode == RenderMode.ScreenSpaceCamera && Camera == null)
            return;

        if (RectTransform == null || CanvasRectTransform == null)
            Cache();

        if (Camera != null)
            targetScreenPointPosition = Camera.WorldToScreenPoint(Target.position);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(CanvasRectTransform, targetScreenPointPosition, Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera, out worldPointInRectangle);
        RectTransform.position = worldPointInRectangle;
        RectTransform.anchoredPosition += Offset;
    }

    void Reset()
    {
        Cache();
    }
}
