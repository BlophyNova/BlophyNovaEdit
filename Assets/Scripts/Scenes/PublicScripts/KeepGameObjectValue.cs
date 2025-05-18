using System.Collections;
using UnityEngine;

public class KeepGameObjectValue : MonoBehaviour
{
    public bool isPositionMode;
    public bool isRotationMode;
    public bool isScaleMode;
    public Transform parentTransform;
    public Vector3 positionPoint;
    public float scaleXY;

    private void Update()
    {
        if (isPositionMode)
        {
            transform.position = parentTransform.TransformPoint(positionPoint);
        }

        if (isRotationMode)
        {
            transform.rotation = Quaternion.identity;
        }

        if (isScaleMode)
        {
            float x = scaleXY / parentTransform.lossyScale.x;
            float y = scaleXY / parentTransform.lossyScale.y;
            //float rotate = parentTransform.rotation.eulerAngles.z;
            //int CountOf90Degrees = (int)(rotate / 90);
            //rotate -= CountOf90Degrees * 90;
            //float proportion = rotate / 90f;
            //transform.localScale = new((1 - proportion) * x + proportion * y, (1 - proportion) * y + proportion * x);
            transform.localScale = new Vector3(x, y);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(InitScale());
    }

    private IEnumerator InitScale()
    {
        yield return new WaitForEndOfFrame();
        float x = scaleXY / 2.7f;
        float y = scaleXY / 2.7f;
        transform.localScale = new Vector3(x, y);
    }
}