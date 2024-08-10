using Controller;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;
using static UnityEngine.Camera;
public class EaseRenderer : MonoBehaviour,IRefreshUI
{
    public EventEditItem eventEditItem;
    EventEdit ThisEventEdit => (EventEdit)eventEditItem.labelWindow.currentLabelWindow;
    public VectorLine line;
    public List<Vector2> points = new();
    //public LineRenderer lineRenderer;
    public VectrosityLineMask maskObject;
    private void Start()
    {
        eventEditItem.labelWindow.onWindowSizeChanged += () =>
        {
            if (eventEditItem.labelWindow.currentLabelWindow.labelWindowContentType != LabelWindowContentType.EventEdit) return;
            UpdateEaseLineArea();
            UpdateEaseLinePosition();
        };
        eventEditItem.labelWindow.onWindowMoved += () =>
        {
            if (eventEditItem.labelWindow.currentLabelWindow.labelWindowContentType != LabelWindowContentType.EventEdit) return;
            UpdateEaseLinePosition();
        };
        DrawCurveLine();
        StartCoroutine(UpdateAreaAndPosition());
    }
    void UpdateEaseLineArea()
    {
        maskObject.rectTransform.sizeDelta = new(ThisEventEdit.VerticalLineDistance * (main.pixelWidth / 1920f), ThisEventEdit.basicLine.AriseLineAndBasicLinePositionYDelta * (main.pixelHeight / 1080f));
        Debug.Log($"ThisEventEdit.basicLine.AriseLineAndBasicLinePositionYDelta:{ThisEventEdit.basicLine.AriseLineAndBasicLinePositionYDelta}");
    }
    void UpdateEaseLinePosition()
    {
        Vector3 eventEditItemWorldPosition = eventEditItem.rectTransform.transform.position;
        eventEditItemWorldPosition.y = ThisEventEdit.eventVerticalLines[0].transform.position.y - Vector2.Distance(ThisEventEdit.basicLine.arisePosition.transform.position, ThisEventEdit.basicLine.basicLine.transform.position) / 2;
        maskObject.transform.position = main.WorldToScreenPoint(eventEditItemWorldPosition);
    }
    
    Vector2 Viewport2LocalPosition(Vector2 viewport)
    {
        Vector2 res = new()
        {
            x = eventEditItem.rectTransform.sizeDelta.x * viewport.x,
            y = eventEditItem.rectTransform.sizeDelta.y * viewport.y
        };
        return res;
    }
    public void DrawCurveLine()
    {
        RefreshUI();
    }
    private void Update()
    {
        UpdateCurveLine();
    }
    void UpdateCurveLine()
    {
        List<Vector2> screenSpacePoints = new();
        for (int i = 0; i < points.Count; i++)
        {
            screenSpacePoints.Add(main.WorldToScreenPoint(transform.TransformPoint(Viewport2LocalPosition(points[i]))));
        }
        line.points2 = screenSpacePoints;
        //line.points2 = points;
        line.Draw();
        //Debug.Log($"鼠标位置:{Input.mousePosition},a1:{a1},a2:{a2}");
    }
    private void OnEnable()
    {
        line.color = new(line.color.r, line.color.g, line.color.b, 1);
    }
    private void OnDisable()
    {
        line.color = new(line.color.r, line.color.g, line.color.b, 0);
    }
    public void RefreshUI()
    {
        if(line.rectTransform!=null) Destroy(line.rectTransform.gameObject);
        if(maskObject!=null) Destroy(maskObject.gameObject);
        if (line != null) line=null;
        line = new("Line",points, 2,LineType.Continuous,Joins.Fill);
        line.SetCanvas(UIVectrosity.Instance.gameObject);
        //line.SetMask(eventEditItem.labelWindow.vectrosityLineMask.mask);
        line.color = new(92, 206, 250, 1);
        line.rectTransform.AddComponent<Mask>();
        maskObject = Instantiate(GlobalData.Instance.vectrosityLineMask, line.rectTransform);
        //UpdateEaseLineArea();
        //updateEaseLinePosition();
        StartCoroutine(UpdateAreaAndPosition());
    }
    
    IEnumerator UpdateAreaAndPosition()
    {
        yield return new WaitForEndOfFrame();

        UpdateEaseLineArea();
        UpdateEaseLinePosition();
    }
}
