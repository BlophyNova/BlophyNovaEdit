using Data.EaseData;
using Form.LabelWindow;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.ChartEdit;
using Newtonsoft.Json;
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


        EaseEdit easeEdit => notePropertyEdit.EditEvent.easeEdit;
        EditEvent EditEvent => notePropertyEdit.EditEvent;
        bool isPresetEase => easeEdit.easeStyle.value == 0;

        // Start is called before the first frame update
        void Start()
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
                points.Add(new(){x=.5f,y=.5f});
                points.Add(new(){x=.5f,y=.5f});
                UpdateDraw(points);
            });
            saveCurve.onClick.AddListener(() =>
            {
                List<Data.ChartEdit.Point> points = line.points.Select(point => point.thisPointData).ToList();
                
                CustomCurve customCurve = new() {name=$"{easeEdit.customEaseName.text}", points = points };
                GlobalData.Instance.chartEditData.customCurves.Add(customCurve);
            });
            deleteCurve.onClick.AddListener(() =>
            {
                GlobalData.Instance.chartEditData.customCurves.RemoveAt(currentCurveIndex);
            });
            exportToClipboard.onClick.AddListener(() =>
            {
                List<Data.ChartEdit.Point> points = line.points.Select(point => point.thisPointData).ToList();
                GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(points);
            });
            importFromClipboard.onClick.AddListener(() =>
            {
                string rawData = GUIUtility.systemCopyBuffer;
                List<Data.ChartEdit.Point> points = JsonConvert.DeserializeObject<List<Data.ChartEdit.Point>>(rawData);
                UpdateDraw(points);
            });
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
                UpdateDraw(GlobalData.Instance.chartEditData.customCurves[value].points);
            }
        }

        private void UpdateDraw(int value)
        {
            line.UpdateDraw(GlobalData.Instance.easeDatas[value].thisCurve);
        }
        private void UpdateDraw(List<Data.ChartEdit.Point> points)
        {
            line.UpdateDraw(points);
        }

        private void LabelWindow_onWindowSizeChanged()
        {

            selfRectTransform.sizeDelta = new(viewport.rect.width, viewport.rect.width+250);
            contentRectTransform.sizeDelta = new(viewport.rect.width, viewport.rect.width);
            UpdateDraw(currentCurveIndex);
        }
    }
}