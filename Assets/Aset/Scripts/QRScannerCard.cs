using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.Common;

public class QRScannerCard : MonoBehaviour
{
    public RawImage preview;
    public Texture defaultTexture;

    private WebCamTexture cam;
    private BarcodeReader reader;

    public System.Action<string> OnQRRead;

    public float scanArea = 0.35f; 

    void Start()
    {
        preview.texture = defaultTexture;

        reader = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = true,
            Options = new DecodingOptions { TryHarder = true }
        };
    }

    public void StartCamera()
    {
        if (cam == null)
        {
            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices.Length == 0)
            {
                Debug.Log("No camera.");
                return;
            }

            string camName = devices[0].name;

            foreach (var d in devices)
            {
                if (!d.isFrontFacing)
                {
                    camName = d.name;
                    break;
                }
            }

            cam = new WebCamTexture(camName, 1280, 720);
        }

        if (!cam.isPlaying)
        {
            cam.Play();
            preview.texture = cam;
            Debug.Log("Camera QRScannerCard start.");
        }
    }

    void Update()
    {
        if (cam == null || !cam.isPlaying) return;
        if (cam.width < 100) return;

        Color32[] data = cam.GetPixels32();
        int w = cam.width;
        int h = cam.height;

        int cropW = (int)(w * scanArea);
        int cropH = (int)(h * scanArea);

        int startX = (w - cropW) / 2;
        int startY = (h - cropH) / 2;

        Color32[] cropped = new Color32[cropW * cropH];

        for (int y = 0; y < cropH; y++)
        {
            for (int x = 0; x < cropW; x++)
            {
                cropped[y * cropW + x] = data[(startY + y) * w + (startX + x)];
            }
        }

        var result = reader.Decode(cropped, cropW, cropH);

        if (result != null)
        {
            OnQRRead?.Invoke(result.Text);
        }

        AutoFlip();
    }

    private void AutoFlip()
    {
        if (cam != null && cam.videoVerticallyMirrored)
        {
            preview.rectTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            preview.rectTransform.localScale = Vector3.one;
        }

        preview.rectTransform.localEulerAngles =
            new Vector3(0, 0, -cam.videoRotationAngle);
    }

    public void StopCamera()
    {
        if (cam != null && cam.isPlaying)
        {
            cam.Stop();
            Debug.Log("Camera QRScannerCard stop.");
        }

        preview.texture = defaultTexture;
    }


}