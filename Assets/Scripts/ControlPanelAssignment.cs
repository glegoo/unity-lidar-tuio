using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Assets.SerialPortUtility.Scripts
{
    public class ControlPanelAssignment : MonoBehaviour
    {

        public SerialCommunicationFacade scf;
        public RunSerial rs;
        public GameObject inputfields;
        public GameObject textfields;
        public GameObject updatebutton;
        public GameObject reloadbutton;


        public Text particleSize;
        public Text zoomTimes;
        public Text sensorAngle;
        public Text sensorOffsetX;
        public Text sensorOffsetY;
        public Text myCanvasWidth;
        public Text myCanvasHeight;
        public Text neededAreaWidth;
        public Text neededAreaHeight;
        public Text neededAreaOffsetX;
        public Text neededAreaOffsetY;
        public Text gridSizeX;
        public Text gridSizeY;
        public Text xmlPath;
        public Text portName;

        public InputField particleSizeInput;
        public InputField zoomTimesInput;
        public InputField sensorAngleInput;
        public InputField sensorOffsetXInput;
        public InputField sensorOffsetYInput;
        public InputField myCanvasWidthInput;
        public InputField myCanvasHeightInput;
        public InputField neededAreaWidthInput;
        public InputField neededAreaHeightInput;
        public InputField neededAreaOffsetXInput;
        public InputField neededAreaOffsetYInput;
        public InputField gridSizeXInput;
        public InputField gridSizeYInput;
        public InputField xmlPathInput;
        public InputField portNameInput;



        bool isShown;




        void Awake()
        {
            if (PlayerPrefs.HasKey(particleSize.ToString())) particleSizeInput.text = PlayerPrefs.GetFloat(particleSize.ToString()).ToString();
            if (PlayerPrefs.HasKey(zoomTimes.ToString())) zoomTimesInput.text = PlayerPrefs.GetFloat(zoomTimes.ToString()).ToString();
            if (PlayerPrefs.HasKey(sensorAngle.ToString())) sensorAngleInput.text = PlayerPrefs.GetFloat(sensorAngle.ToString()).ToString();
            if (PlayerPrefs.HasKey(sensorOffsetX.ToString())) sensorOffsetXInput.text = PlayerPrefs.GetFloat(sensorOffsetX.ToString()).ToString();
            if (PlayerPrefs.HasKey(sensorOffsetY.ToString())) sensorOffsetYInput.text = PlayerPrefs.GetFloat(sensorOffsetY.ToString()).ToString();
            if (PlayerPrefs.HasKey(myCanvasWidth.ToString())) myCanvasWidthInput.text = PlayerPrefs.GetFloat(myCanvasWidth.ToString()).ToString();
            if (PlayerPrefs.HasKey(myCanvasHeight.ToString())) myCanvasHeightInput.text = PlayerPrefs.GetFloat(myCanvasHeight.ToString()).ToString();
            if (PlayerPrefs.HasKey(neededAreaWidth.ToString())) neededAreaWidthInput.text = PlayerPrefs.GetFloat(neededAreaWidth.ToString()).ToString();
            if (PlayerPrefs.HasKey(neededAreaHeight.ToString())) neededAreaHeightInput.text = PlayerPrefs.GetFloat(neededAreaHeight.ToString()).ToString();
            if (PlayerPrefs.HasKey(neededAreaOffsetX.ToString())) neededAreaOffsetXInput.text = PlayerPrefs.GetFloat(neededAreaOffsetX.ToString()).ToString();
            if (PlayerPrefs.HasKey(neededAreaOffsetY.ToString())) neededAreaOffsetYInput.text = PlayerPrefs.GetFloat(neededAreaOffsetY.ToString()).ToString();
            if (PlayerPrefs.HasKey(gridSizeX.ToString())) gridSizeXInput.text = PlayerPrefs.GetFloat(gridSizeX.ToString()).ToString();
            if (PlayerPrefs.HasKey(gridSizeY.ToString())) gridSizeYInput.text = PlayerPrefs.GetFloat(gridSizeY.ToString()).ToString();
            if (PlayerPrefs.HasKey(xmlPath.ToString())) xmlPathInput.text = PlayerPrefs.GetString(xmlPath.ToString()).ToString();
            if (PlayerPrefs.HasKey(portName.ToString())) portNameInput.text = PlayerPrefs.GetString(portName.ToString()).ToString();

            //assignment
            scf.particleSize = float.Parse(particleSizeInput.text);
            scf.zoomTimes = float.Parse(zoomTimesInput.text);
            scf.sensorAngle = float.Parse(sensorAngleInput.text);
            scf.sensorOffsetX = float.Parse(sensorOffsetXInput.text);
            scf.sensorOffsetY = float.Parse(sensorOffsetYInput.text);
            scf.myCanvasWidth = float.Parse(myCanvasWidthInput.text);
            scf.myCanvasHeight = float.Parse(myCanvasHeightInput.text);
            scf.neededAreaWidth = float.Parse(neededAreaWidthInput.text);
            scf.neededAreaHeight = float.Parse(neededAreaHeightInput.text);
            scf.neededAreaOffsetX = float.Parse(neededAreaOffsetXInput.text);
            scf.neededAreaOffsetY = float.Parse(neededAreaOffsetYInput.text);
            scf.gridSizeX = int.Parse(gridSizeXInput.text);
            scf.gridSizeY = int.Parse(gridSizeYInput.text);
            scf.xmlPath = xmlPathInput.text.ToString();

            rs.portName = portNameInput.text.ToString();

            //save playerPref
            PlayerPrefs.SetFloat(particleSize.ToString(), float.Parse(particleSizeInput.text));
            PlayerPrefs.SetFloat(zoomTimes.ToString(), float.Parse(zoomTimesInput.text));
            PlayerPrefs.SetFloat(sensorAngle.ToString(), float.Parse(sensorAngleInput.text));
            PlayerPrefs.SetFloat(sensorOffsetX.ToString(), float.Parse(sensorOffsetXInput.text));
            PlayerPrefs.SetFloat(sensorOffsetY.ToString(), float.Parse(sensorOffsetYInput.text));
            PlayerPrefs.SetFloat(myCanvasWidth.ToString(), float.Parse(myCanvasWidthInput.text));
            PlayerPrefs.SetFloat(myCanvasHeight.ToString(), float.Parse(myCanvasHeightInput.text));
            PlayerPrefs.SetFloat(neededAreaWidth.ToString(), float.Parse(neededAreaWidthInput.text));
            PlayerPrefs.SetFloat(neededAreaHeight.ToString(), float.Parse(neededAreaHeightInput.text));
            PlayerPrefs.SetFloat(neededAreaOffsetX.ToString(), float.Parse(neededAreaOffsetXInput.text));
            PlayerPrefs.SetFloat(neededAreaOffsetY.ToString(), float.Parse(neededAreaOffsetYInput.text));
            PlayerPrefs.SetFloat(gridSizeX.ToString(), int.Parse(gridSizeXInput.text));
            PlayerPrefs.SetFloat(gridSizeY.ToString(), int.Parse(gridSizeYInput.text));
            PlayerPrefs.SetString(xmlPath.ToString(), xmlPathInput.text.ToString());
            PlayerPrefs.SetString(portName.ToString(), portNameInput.text.ToString());

            isShown = false;
            inputfields.SetActive(isShown);
            textfields.SetActive(isShown);
            updatebutton.SetActive(isShown);
            reloadbutton.SetActive(isShown);
        }

        public void UpdatePlayerPref()
        {
            //assignment
            scf.particleSize = float.Parse(particleSizeInput.text);
            scf.zoomTimes = float.Parse(zoomTimesInput.text);
            scf.sensorAngle = float.Parse(sensorAngleInput.text);
            scf.sensorOffsetX = float.Parse(sensorOffsetXInput.text);
            scf.sensorOffsetY = float.Parse(sensorOffsetYInput.text);
            scf.myCanvasWidth = float.Parse(myCanvasWidthInput.text);
            scf.myCanvasHeight = float.Parse(myCanvasHeightInput.text);
            scf.neededAreaWidth = float.Parse(neededAreaWidthInput.text);
            scf.neededAreaHeight = float.Parse(neededAreaHeightInput.text);
            scf.neededAreaOffsetX = float.Parse(neededAreaOffsetXInput.text);
            scf.neededAreaOffsetY = float.Parse(neededAreaOffsetYInput.text);
            scf.gridSizeX = int.Parse(gridSizeXInput.text);
            scf.gridSizeY = int.Parse(gridSizeYInput.text);
            scf.xmlPath = xmlPathInput.text.ToString();
            scf.UpdateDataWPlayerPref();
            rs.portName = portNameInput.text.ToString();

            //save playerPref
            PlayerPrefs.SetFloat(particleSize.ToString(), float.Parse(particleSizeInput.text));
            PlayerPrefs.SetFloat(zoomTimes.ToString(), float.Parse(zoomTimesInput.text));
            PlayerPrefs.SetFloat(sensorAngle.ToString(), float.Parse(sensorAngleInput.text));
            PlayerPrefs.SetFloat(sensorOffsetX.ToString(), float.Parse(sensorOffsetXInput.text));
            PlayerPrefs.SetFloat(sensorOffsetY.ToString(), float.Parse(sensorOffsetYInput.text));
            PlayerPrefs.SetFloat(myCanvasWidth.ToString(), float.Parse(myCanvasWidthInput.text));
            PlayerPrefs.SetFloat(myCanvasHeight.ToString(), float.Parse(myCanvasHeightInput.text));
            PlayerPrefs.SetFloat(neededAreaWidth.ToString(), float.Parse(neededAreaWidthInput.text));
            PlayerPrefs.SetFloat(neededAreaHeight.ToString(), float.Parse(neededAreaHeightInput.text));
            PlayerPrefs.SetFloat(neededAreaOffsetX.ToString(), float.Parse(neededAreaOffsetXInput.text));
            PlayerPrefs.SetFloat(neededAreaOffsetY.ToString(), float.Parse(neededAreaOffsetYInput.text));
            PlayerPrefs.SetFloat(gridSizeX.ToString(), int.Parse(gridSizeXInput.text));
            PlayerPrefs.SetFloat(gridSizeY.ToString(), int.Parse(gridSizeYInput.text));
            PlayerPrefs.SetString(xmlPath.ToString(), xmlPathInput.text.ToString());
            PlayerPrefs.SetString(portName.ToString(), portNameInput.text.ToString());
        }


        private void Update()
        {
            if (Input.GetKeyDown("d"))
            {
              
                isShown = !isShown;
                inputfields.SetActive(isShown);
                textfields.SetActive(isShown);
                updatebutton.SetActive(isShown);
                reloadbutton.SetActive(isShown);
               
            }
         
        }

        public void ReloadScene()
        {
            UpdatePlayerPref();
            rs.DisconnectLidar();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
           
        }

    }
}