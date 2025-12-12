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

    // ukuran area scan (lebih kecil = lebih cepat, lebih akurat)
    public float scanArea = 0.35f; // 35 persen area tengah

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
                Debug.Log("Tidak ada kamera.");
                return;
            }

            string camName = devices[0].name;

            // pilih kamera belakang dulu (lebih stabil)
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
            Debug.Log("Kamera QRScannerCard dimulai.");
        }
    }

    void Update()
    {
        if (cam == null || !cam.isPlaying) return;
        if (cam.width < 100) return;

        // Ambil pixel
        Color32[] data = cam.GetPixels32();
        int w = cam.width;
        int h = cam.height;

        // Crop area tengah untuk akurasi & speed tinggi
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

        // ZXing decode
        var result = reader.Decode(cropped, cropW, cropH);

        if (result != null)
        {
            OnQRRead?.Invoke(result.Text);
        }

        // AUTO-FLIP kamera depan
        AutoFlip();
    }

    private void AutoFlip()
    {
        // fix mirror pada kamera depan
        if (cam != null && cam.videoVerticallyMirrored)
        {
            preview.rectTransform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            preview.rectTransform.localScale = Vector3.one;
        }

        // fix rotasi
        preview.rectTransform.localEulerAngles =
            new Vector3(0, 0, -cam.videoRotationAngle);
    }

    public void StopCamera()
    {
        if (cam != null && cam.isPlaying)
        {
            cam.Stop();
            Debug.Log("Kamera QRScannerCard berhenti.");
        }

        preview.texture = defaultTexture;
    }


}