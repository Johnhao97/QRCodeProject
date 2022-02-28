using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;
using TMPro;
using UnityEngine.UI;

public class QR : MonoBehaviour
{
    [SerializeField]
    private RawImage _rawImageBackground;

    [SerializeField]
    private AspectRatioFitter _aspectRatioFitter;

    [SerializeField]
    private TextMeshProUGUI _textOut;

    [SerializeField]
    private RectTransform _scanZone;

    private bool _isCamAvaible;

    private WebCamTexture _cameratexture;


    // Start is called before the first frame update
    void Start()
    {
        SetUpCamera();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraRender();
    }

    private void SetUpCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            _isCamAvaible = false;
            return;
        }

        for (int i = 0; i<devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                _cameratexture = new WebCamTexture(devices[i].name, (int)_scanZone.rect.width, (int)_scanZone.rect.height);
            }
        }
        
        _cameratexture.Play();
        _rawImageBackground.texture = _cameratexture;
        _isCamAvaible = true;
    }

    private void UpdateCameraRender()
    {
        if (_isCamAvaible == false)
        {
            return;
        }
        float ratio = (float)_cameratexture.width / (float)_cameratexture.height;
        _aspectRatioFitter.aspectRatio = ratio;

        int orientation = _cameratexture.videoRotationAngle;
        _rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }

    public void OnClickSccan()
    {
        Scan();
    }

    private void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            Result result = barcodeReader.Decode(_cameratexture.GetPixels32(), _cameratexture.width, _cameratexture.height);
            if (result != null)
            {
                _textOut.text = result.Text;
            }
            else
            {
                _textOut.text = "FAILED TO READ QR CODE";
            }
        }

        catch
        {
            _textOut.text = "FAILED IN TRY";
        }
    }
}
