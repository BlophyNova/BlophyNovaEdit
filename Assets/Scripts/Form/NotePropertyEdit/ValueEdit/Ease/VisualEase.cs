using System.Collections.Generic;
using System.Linq;
using Data.ChartEdit;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UnityEngine.UI;

namespace Form.NotePropertyEdit.ValueEdit.Ease
{
    public class VisualEase : MonoBehaviour
    {
        public int currentCurveIndex;

        public NotePropertyEdit notePropertyEdit;
        public RectTransform selfRectTransform;
        public RectTransform contentRectTransform;
        public RectTransform viewport;
        public Line line;

        public Button createCurve;
        public Button saveCurve;
        public Button deleteCurve;
        public Button exportToClipboard;
        public Button importFromClipboard;


        private EaseEdit easeEdit => notePropertyEdit.EditEvent.easeEdit;
        private EditEvent EditEvent => notePropertyEdit.EditEvent;
        private bool isPresetEase => easeEdit.easeStyle.value == 0;

        // Start is called before the first frame update
        private void Start()
        {
            notePropertyEdit.labelWindow.onWindowSizeChanged += LabelWindow_onWindowSizeChanged;
            LabelWindow_onWindowSizeChanged();

            easeEdit.easeStyle.onValueChanged.AddListener(EaseStyleChanged);
            EaseStyleChanged(easeEdit.easeStyle.value);
            easeEdit.onValueChanged += EaseEdit_onValueChanged;
            easeEdit.onCustomValueChanged += EaseEdit_onValueChanged;
            createCurve.onClick.AddListener(() =>
            {
                List<Data.ChartEdit.Point> points = new();
                points.Add(new Data.ChartEdit.Point { x = .5f, y = .5f });
                points.Add(new Data.ChartEdit.Point { x = .5f, y = .5f });
                UpdateDraw(points);
            });
            saveCurve.onClick.AddListener(() =>
            {
                List<Data.ChartEdit.Point> points = new();
                foreach (Point point in line.points)
                {
                    points.Add(new Data.ChartEdit.Point(point.thisPointData));
                }

                string customCurveName = easeEdit.customEaseName.text;
                CustomCurve customCurve = new() { name = customCurveName, points = points };
                GlobalData.Instance.chartEditData.customCurves.Add(customCurve);

                easeEdit.easeStyle.value = 0;
                easeEdit.easeStyle.value = 1;
            });
            deleteCurve.onClick.AddListener(() =>
            {
                if (currentCurveIndex <= 0)
                {
                    return;
                }

                //GlobalData.Instance.chartEditData.customCurves.RemoveAt(currentCurveIndex-1);
                //GlobalData.Instance.chartEditData.customCurves[currentCurveIndex - 1] = null;
                GlobalData.Instance.chartEditData.customCurves[currentCurveIndex - 1].isDeleted = true;
                currentCurveIndex = 0;
                easeEdit.easeStyle.value = 0;
                easeEdit.easeStyle.value = 1;

                EditEvent.Finally();
            });
            exportToClipboard.onClick.AddListener(() =>
            {
                List<Data.ChartEdit.Point> points = line.points.Select(point => point.thisPointData).ToList();

                CustomCurve customCurve = new() { name = $"{easeEdit.customEaseName.text}", points = points };
                GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(customCurve);
            });
            importFromClipboard.onClick.AddListener(() =>
            {
                string rawData = GUIUtility.systemCopyBuffer;
                CustomCurve customCurve = JsonConvert.DeserializeObject<CustomCurve>(rawData);
                GlobalData.Instance.chartEditData.customCurves.Add(customCurve);
                easeEdit.easeStyle.value = 0;
                easeEdit.easeStyle.value = 1;
            });
            notePropertyEdit.labelItem.onLabelGetFocus += () => { line.lineRenderer.gameObject.SetActive(true); };
            notePropertyEdit.labelItem.onLabelLostFocus += () => { line.lineRenderer.gameObject.SetActive(false); };
        }

        private void EaseStyleChanged(int value)
        {
            if (value == 0)
            {
                createCurve.interactable = false;
                saveCurve.interactable = false;
                deleteCurve.interactable = false;
                exportToClipboard.interactable = false;
                importFromClipboard.interactable = false;
                line.HidePoints();
            }
            else
            {
                createCurve.interactable = true;
                saveCurve.interactable = true;
                deleteCurve.interactable = true;
                exportToClipboard.interactable = true;
                importFromClipboard.interactable = true;
                line.ShowPoints();
            }
        }

        public void EaseEdit_onValueChanged(int value)
        {
            currentCurveIndex = value;


            if (isPresetEase)
            {
                UpdateDraw(currentCurveIndex);
            }
            else
            {
                if (value <= 0)
                {
                    return;
                }

                if (GlobalData.Instance.chartEditData.customCurves[value - 1].isDeleted)
                {
                    return;
                }

                UpdateDraw(GlobalData.Instance.chartEditData.customCurves[value - 1].points);
            }
        }

        private void UpdateDraw(int value)
        {
            line.UpdateDraw(GlobalData.Instance.easeDatas[value].thisCurve);
        }

        private void UpdateDraw(List<Data.ChartEdit.Point> points)
        {
            Vector3[] corners = new Vector3[4];
            line.selfRect.GetWorldCorners(corners);
            //更新points的xy
            for (int i = 0; i < line.points.Count; i++)
            {
                float x = (corners[2].x - corners[0].x) * points[i].x + corners[0].x;
                float y = (corners[2].y - corners[0].y) * points[i].y + corners[0].y;
                line.points[i].transform.position = new Vector3(x, y);
                line.points[i].xyInfo.text = $"({points[i].x:F3},{points[i].y:F3})";
            }

            line.UpdateDraw(points);
        }

        private void LabelWindow_onWindowSizeChanged()
        {
            selfRectTransform.sizeDelta = new Vector2(viewport.rect.width, viewport.rect.width + 250);
            contentRectTransform.sizeDelta = new Vector2(viewport.rect.width, viewport.rect.width);
            UpdateDraw(currentCurveIndex);
        }
    }
}