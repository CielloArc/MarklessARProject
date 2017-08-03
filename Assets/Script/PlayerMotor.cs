using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{
    //Public
    [SerializeField]
    private RawImage background;
    [SerializeField]
    private AspectRatioFitter fit;

    //Private
    private Gyroscope gyro;
    private GameObject cameraContainer;
    private Quaternion rotation;
    private WebCamTexture cam;

    private bool arReady = false;

    void Start()
    {
        //Checar se o aparelho suporta o giroscópio e se o aparelho tem camera traseira
        //Primeiro checar o giroscópio, se não houver retornar
//        if (!SystemInfo.supportsGyroscope)
//        {
//            Debug.Log("Can't spin for shit capt'n!");
//            return;
//        }

        //Depois, checar a quantidade de cameras no aparelho.
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            //Se houver uma camera traseira, jogue ela na variavel cam
            if (!WebCamTexture.devices[i].isFrontFacing)
            {
                cam = new WebCamTexture(WebCamTexture.devices[i].name, Screen.width, Screen.height);
                break;
            }
        }

        //Se não houver camera, retornar.
        if (cam == null)
        {
            Debug.Log("Can't see shit capt'n!");
            return;
        }


        //NOTA: Este código só sera executado caso o dispositivo tenha câmera traseira e giroscópio.
        //A partir daqui, crie um novo GameObject e deixe ele como pai deste objeto
        cameraContainer = new GameObject("CameraContainer");
        cameraContainer.transform.position = transform.position;
        transform.SetParent(cameraContainer.transform);

        //Habilite o giroscópio
        gyro = Input.gyro;
        gyro.enabled = true;
        cameraContainer.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        rotation = new Quaternion(0, 0, 1, 0);

        //Ative a camera
        cam.Play();
        background.texture = cam;

        //E deixe o jogo saber que a realidade aumentada está habiltada
        arReady = true;

    }

    void Update()
    {
        if (arReady)
        {

            float ratio = (float)cam.width / (float)cam.height;
            fit.aspectRatio = ratio;

            float scaleY = cam.videoVerticallyMirrored ? -1.0f : 1.0f;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -cam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

            transform.localRotation = gyro.attitude * rotation;
        }
    }
}
