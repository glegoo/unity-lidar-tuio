using Assets.SerialPortUtility.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using System.Xml.Linq;

namespace Assets.SerialPortUtility.Scripts
{
    public class SerialCommunicationFacade : MonoBehaviour, ISerialCommunication
    {
        List<int> ids = new List<int>();

        SerialCommunication serialCom;
        private string reciveStr;
        private string oldStr;

        float fsaAngle;
        float lsaAngle;
        float angle;//中間角
       
        public float time = 0;

        //UI 用於顯示
        public GameObject img;
        public GameObject canvas;
        List<GameObject> imgs = new List<GameObject>();
        public GameObject camera;
        public UdpServer udp;

        //particle system
        public float particleSize = 3f;
        int skipParticles = 2;
        public ParticleSystem pointCloudParticles;
        private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];

        public float zoomTimes = 0.3f;
        public float sensorAngle = 0;   // cable upwards - 0 degree, clockwise


        public float sensorOffsetX = 0;  // sensor's distance in x axis to the canvas
        public float sensorOffsetY = 0;
        float myCanvasWidth = 1920;
        float myCanvasHeight = 1080;
        public float ratioMM = 100;
        public float ratioPX = 400;
        float sensorOffsetinPixelX;
        float sensorOffsetinPixelY;

        //public float neededAreaWidth = 1920;
        //public float neededAreaHeight = 1080;
        //public float neededAreaOffsetX = 0;
        //public float neededAreaOffsetY = 0;
        public int gridSizeX = 20;
        public int gridSizeY = 20;
        public string xmlPath = "Assets/myXML/settings.xml";

        //trigger area
        public GameObject[] triggerAreas;
        Rect[] triggerRects;
        RawImage[] triggerRawImage;
        Color32[] colors;
        bool[] IsInsideArea;
        bool[] IsInsideArea_last;


        public Material myCanvasMat;
        int canvasGridX, canvasGridY;

        List<GameObject> objs = new List<GameObject>();

        //GameObject neededArea;

        void Start()
        {

            myCanvasHeight = Screen.currentResolution.height;
            myCanvasWidth = Screen.currentResolution.width;
            //initialize the needed area
            //neededArea = new GameObject();
            //neededArea.AddComponent<RectTransform>();
            //neededArea.transform.SetParent(GameObject.Find("Panel").transform);
           
            //neededArea.AddComponent<RawImage>();
            //neededArea.GetComponent<RawImage>().color = new Color32(0, 86, 147, 45);

            //neededArea.GetComponent<RectTransform>().localScale = Vector3.one;

            //initialize the trriger area rects colors and bools
            triggerRects = new Rect[triggerAreas.Length];
            triggerRawImage = new RawImage[triggerAreas.Length];
            colors = new Color32[triggerAreas.Length];
            IsInsideArea = new bool[triggerAreas.Length];
            IsInsideArea_last = new bool[triggerAreas.Length];

            UpdateDataWPlayerPref();


            //print("x: " + myXY[0] + "  y: " + myXY[1]);



            //print(triggerRects[0].width +" " + triggerRects[0].height +" "+ triggerRects[0].x +" "+ triggerRects[0].y);

            //repeatly calling update in 0.02f time interval
            InvokeRepeating("UpdateParticlesAndAreas", 1.0f, 0.02f);
        }

        public void UpdateDataWPlayerPref()
        {
            //initialize the particle system
            canvasGridX = Mathf.FloorToInt(myCanvasWidth) / gridSizeX;
            canvasGridY = Mathf.FloorToInt(myCanvasHeight) / gridSizeY;

            int num = canvasGridX * canvasGridY;
            particles = new ParticleSystem.Particle[num];
            for (int hor = 0; hor < canvasGridX; hor++)
            {
                for (int ver = 0; ver < canvasGridY; ver++)
                {
                    int id = hor + canvasGridX * ver;
                    //particles[id].position = new Vector2((hor - canvasGridX / 2) * 3, (ver - canvasGridY / 2) * 3);
                    particles[id].position = new Vector2(0, 0);
                    particles[id].startSize = particleSize;
                    particles[id].startColor = Color.black;
                }
            }

            //set the particles initially
            pointCloudParticles.SetParticles(particles, particles.Length);

            //calculate the sensor offset in pixel
            sensorOffsetinPixelX = sensorOffsetX * ratioPX / ratioMM;
            sensorOffsetinPixelY = sensorOffsetY * ratioPX / ratioMM;

            //needed area
            //neededArea.GetComponent<RectTransform>().localPosition = new Vector2(sensorOffsetinPixelX, sensorOffsetinPixelY);
            //neededArea.GetComponent<RectTransform>().sizeDelta = new Vector2(neededAreaWidth, neededAreaHeight);

            //reading xml n assigning to the trigger area
            XDocument allXMLData = XDocument.Load(xmlPath);
            for (int m = 0; m < triggerAreas.Length; m++)
            {
                string[] center = allXMLData.Element("settings").Element("videoContainer" + m).Element("center").Value.Trim().Split(',');
                string width = allXMLData.Element("settings").Element("videoContainer" + m).Element("width").Value.Trim();
                string height = allXMLData.Element("settings").Element("videoContainer" + m).Element("height").Value.Trim();
                triggerAreas[m].GetComponent<RectTransform>().localPosition = new Vector3((myCanvasWidth / 2 - (float.Parse(center[0]) + float.Parse(width) / 2)) * (-1),
                                                                                          (myCanvasHeight / 2 - (float.Parse(center[1]) + float.Parse(height) / 2)),
                                                                                          0);
                triggerAreas[m].GetComponent<RectTransform>().sizeDelta = new Vector3(float.Parse(width), float.Parse(height), 1);
            }

            //initialize rect
            for (int i = 0; i < triggerAreas.Length; i++)
            {
                triggerRects[i] = new Rect(triggerAreas[i].GetComponent<RectTransform>().localPosition.x - triggerAreas[i].GetComponent<RectTransform>().sizeDelta.x * 0.5f,
                                            triggerAreas[i].GetComponent<RectTransform>().localPosition.y - triggerAreas[i].GetComponent<RectTransform>().sizeDelta.y * 0.5f,
                                            triggerAreas[i].GetComponent<RectTransform>().sizeDelta.x,
                                            triggerAreas[i].GetComponent<RectTransform>().sizeDelta.y);
                triggerRawImage[i] = triggerAreas[i].GetComponent<RawImage>();
                triggerRawImage[i].color = new Color32(2, 130, 27, 55);
                IsInsideArea[i] = false;
                IsInsideArea_last[i] = false;
            }

         

        }

#region things that dont need to care

        public void Connect(int baudrate, string portName)
        {
            serialCom = new SerialCommunication(portName, baudrate);
            serialCom.OpenSerialPort();// 打開串口
            // 綁定方法觸發,監聽讀取串口
            serialCom.SerialPortMessageEvent += SerialCom_SerialPortMessageEvent;
            // 綁定方法觸發,給串口發消息
            serialCom.SerialPortSendMessageReportEvent += SerialCom_SerialPortSendMessageReportEvent;
        }
        public void Disconnect()
        {
            serialCom.CloseSerialPort();
            Debug.Log("Serial Disconnected");
        }
        public void SendMessage(byte[] byteArray)
        {
            try
            {
                if (serialCom.IsSerialPortIsOpen() == true)
                {

                    serialCom.SendMessageFromSerialPort(byteArray);
                    //Debug.Log("Message Sended");
                }
                else
                {
                    Debug.Log("Message Send Failed!");
                }
            }
            catch (Exception)
            {


            }

        }

        /// <summary>
        /// 監聽給串口發的消息
        /// </summary>
        /// <param name="sendData"></param>
        private void SerialCom_SerialPortSendMessageReportEvent(byte[] sendData)
        {
            string text = RunSerial.byteToHexStr(sendData);
            Debug.Log("Message Send From Serial.. Message =>  " + text.ToString());
        }

        /// <summary>
        /// 串口發過來的消息
        /// </summary>
        /// <param name="sendData"></param>
        private void SerialCom_SerialPortMessageEvent(byte[] sendData)
        {
            //Debug.Log("____________shoudaole _____________");

            reciveStr = RunSerial.byteToHexStr(sendData);
            //Debug.Log("Message Coming From Serial.. Message =>  " + reciveStr.ToString());//打印一條數據信息

            Data_ResponseContentProcess(sendData);





            //if (!reciveStr.Contains("D") || !reciveStr.Contains("m"))
            //{
            //    return;
            //}
            ////Debug.Log("收到的數據:" + reciveStr.ToString());
            //if (reciveStr == oldStr)
            //{
            //    return;
            //}
            ////AllUiManger._Instance.DealReceiveString(reciveStr.ToString());
            //oldStr = reciveStr;

        }

        List<float> angles = new List<float>();//一級解析角度
        List<float> distances = new List<float>();//所有的距離
        List<double> angleCorrects = new List<double>();//角度偏差
        List<float> correctedAngles = new List<float>();//二級解析角度

        List<Vector2> ids_positions = new List<Vector2>();
      
        public static bool isSaveAll = false;//距離和角度都存進去了

        public static Dictionary<float, float> angleAndDistances = new Dictionary<float, float>();


        /// <summary>
        /// 應答數據處理S
        /// </summary>
        ///數據處理  
        ///responseData[0] AA responseData[1]  55  數據包頭    
        ///responseData[2]  包類型   
        ///responseData[3]   採樣數量  
        ///responseData[4] responseData[5]  起始角  
        ///responseData[6] responseData[7] 結束角
        ///responseData[8]  responseData[9]校驗碼
        ///responseData[................] 採樣數據  兩位爲一個數據
        ///
        #endregion
        /// <param name="responseData"></param>
        void Data_ResponseContentProcess(byte[] responseData)
        {
            //零位解析  該數據包中 LSN = 1，即 Si 的數量爲 1，S1 = 零位距離數據 ；
            //FSA = LSA = 零位角度數據； 其距離和角度的具體值解析參見距離和角度的解析
            if (responseData[2] == 0x01 && responseData[3] == 1)
            {

                //AA55 01 01 DFA0 DFA0 AB54 0000
                //零位距離
                //print("-------------------------------------------0x01   " + ids.Count);
                //isIdsAllSet = true;
                for (int restid = ids.Count; restid < particles.Length; restid++)
                {
                    if (restid != canvasGridX / 2 + canvasGridY / 2 * canvasGridX)
                    {
                        particles[restid].position = Vector2.zero;
                        particles[restid].startSize = 0;
                    }
                 
                }

                for (int aid = 0; aid < triggerAreas.Length; aid++)
                {
                    
                    colors[aid] = new Color32(2, 130, 27, 75);
                    IsInsideArea[aid] = false;
                    for (int pid = 0; pid < ids.Count; pid++)
                    {

                        if (triggerRects[aid].Contains(ids_positions[pid]))
                        {
                            
                            colors[aid] = new Color32(171, 37, 1, 75);
                            IsInsideArea[aid] = true;
                            break;
                        }
                        //isContained = false;

                    }
                    //if (isContained)
                    //{
                    //    //print(aid + "  " + 1);
                    //    colors[aid] = new Color32(171, 37, 1, 255);
                    //    IsInsideArea[aid] = true;
                    //}

                    //else
                    //{
                    //    //print(aid + " " + 0);
                    //    colors[aid] = new Color32(2, 130, 27, 255);
                    //    IsInsideArea[aid] = false;
                    //}
                }

                ids.Clear();
                ids_positions.Clear();

#region things dont need to care abt
                //print(ids.Count + "  " + ids_positions.Count);
                string disStr = BinaryConversion(responseData[11], responseData[10]);
                int dis = Convert.ToInt32(disStr, 16);
                float Distancezero = dis / 4;

                //AA55 01 01 7958 7958 AB54 0000
                //起始角度  結束角度

                string fsaStr = BinaryConversion(responseData[5], responseData[4]);
                string lsaStr = BinaryConversion(responseData[7], responseData[6]);

                // Debug.Log(fsaStr);

                int fsa = Convert.ToInt32(fsaStr, 16);
                //Debug.Log(fsa);
                float fsaAnglezero = (fsa / 2) / 64;//起始角計算公式  右移一位除以 64


                string debugMsg = string.Format("零位解析 距離 = {0}, FSA = {1}, LAS = {2} ", Distancezero, fsaAnglezero, fsaAnglezero);
                //Debug.Log(debugMsg);

            }

            // AA55 000F FD27 F129 1252 
            //00 000000000000000000000000000000000000000000000000000000B406
            //點雲數據包
            if (responseData[2] == 0x00)
            {

                ///print("0x00");

                //Debug.Log("dianyunshuju ---------------");
                //Debug.Log(responseData[3]);
                byte[] dataDis = new byte[responseData[3] * 2];


                Array.Copy(responseData, 10, dataDis, 0, responseData[3] * 2);


                //距離解析   兩位爲一個距離數據  距離開始不對，注意距離數據是去除掉前十位之後的數據
                for (int i = 10; i < responseData[3] * 2 + 10; i++)
                {
                    string distanceStr = BinaryConversion(responseData[i + 1], responseData[i]);
                    int dis = Convert.ToInt32(distanceStr, 16);
                    float Distance = dis / 4;
                    distances.Add(Distance);
                    //Debug.Log("距離解析：  距離 Distance = " + Distance);
                    i++;
                }


                //角度解析
                //一級解析  注意小端模式
                string fsaStr = BinaryConversion(responseData[5], responseData[4]);
                string lsaStr = BinaryConversion(responseData[7], responseData[6]);


                int fsa = Convert.ToInt32(fsaStr, 16);
                fsaAngle = (fsa / 2) / 64;//起始角計算公式  右移一位除以 64
                angles.Add(fsaAngle);
                //Debug.Log("角度一級解析：  FSA = " + fsaAngle);

                int lsa = Convert.ToInt32(lsaStr, 16);
                lsaAngle = (lsa / 2) / 64;//起始角計算公式  右移一位除以 64

                if (fsaAngle > lsaAngle) lsaAngle += 360;

                angles.Add(lsaAngle);
                //Debug.Log("角度一級解析：  LSA = " + lsaAngle);

                //中間角  
                float diffAngle = lsaAngle - fsaAngle;
                for (int i = 2; i < responseData[3]; i++)
                {

                    angle = diffAngle / (responseData[3] - 1) * (i - 1) + fsaAngle;
                    angles.Insert(i - 1, angle);
                    //Debug.Log("角度一級解析：  Angle 中間角 = " + angle);
                }

                //print("1");


                double angleCorrect;
                //二級解析   偏差角
                foreach (var item in distances)
                {
                    if (item == 0)
                    {
                        angleCorrect = 0;

                    }
                    else
                    {
                        float mid = 21.8f * (155.3f - item) / (155.3f * item);//
                        angleCorrect = Mathf.Atan((float)mid);//反正切函數參數範圍（-無窮，+ 無窮）  Mathf.Atan 和 Math.Atan 返回的都是弧度。此處需要得到角度 
                        angleCorrect = (180 / Math.PI) * angleCorrect;//弧度轉角度  得到偏差角度
                    }
                    angleCorrects.Add(angleCorrect);
                }

                //print("2");
                #endregion
                // ===========================================================================================================================
                for (int i = 0; i < angleCorrects.Count; i++)
                {
                    float correctedAngle = angles[i] + (float)angleCorrects[i];
                    //Debug.Log("角度二級解析：  所有角度 = " + correctedAngle);
                    correctedAngles.Add(correctedAngle);
       
                    //print(correctedAngle + "              " + distances[i]);
                   
                    float angle = correctedAngle + 180 + sensorAngle; // cable upwards
                    float distance = distances[i];
                    Vector2 pos;
                    //convert to radians
                    float angle_in_rand = angle * Mathf.Deg2Rad;

                    pos = new Vector2(distance * Mathf.Sin(angle_in_rand), distance * Mathf.Cos(angle_in_rand));
                    pos.x *= -1f * ratioPX / ratioMM * zoomTimes;
                    pos.y *= -1f * ratioPX / ratioMM * zoomTimes;

                    int xCount = Mathf.RoundToInt(pos.x / 3);
                    int yCount = Mathf.RoundToInt(pos.y / 3);               

                    Vector2 snapped_pos = new Vector2((float)xCount, (float)yCount);
                   
                    if (pos.x != 0 && pos.y != 0 
                        //&& pos.x < neededAreaWidth * 0.5f + neededAreaOffsetX && pos.x > neededAreaWidth * -0.5f + neededAreaOffsetX
                        //&& pos.y > neededAreaHeight * -0.5f + neededAreaOffsetY && pos.y < neededAreaHeight * 0.5f + neededAreaOffsetY
                        )
                    {
                        xCount = xCount + canvasGridX / 2;
                        yCount = yCount + canvasGridY / 2;
                        int pid = xCount + canvasGridX * yCount;
                        if (!ids.Contains(pid))
                        {
                            pos.x += sensorOffsetinPixelX;
                            pos.y += sensorOffsetinPixelY;
                            ids_positions.Add(new Vector2(pos.x, pos.y));
                            ids.Add(pid);
                        }
                    }
                }


                angleCorrects.Clear();//注意 LIST 要進行清空  否則數據出錯
#region things that dont need to care abt
                //print("3");

                //Debug.Log("angleCorrects.Count,distances.Count:    " + angleCorrects.Count + "   " + distances.Count);
                //for (int i = 0; i < responseData[3]; i++)
                //{
                //    angleAndDistances.Add(correctedAngles[i], distances[i]);//距離計算出的是毫米，Unity 中單位默認是米
                //}

                // print("4");
                isSaveAll = true;
                angles.Clear();
                distances.Clear();
                //angleCorrects.Clear();//注意 LIST 要進行清空  否則數據出錯
                correctedAngles.Clear();

            }

        }

        #endregion
        //this function runs in 0.02f interval
        //remember to update the data in 0x01 (when the lidar turns a round
        public void UpdateParticlesAndAreas()
        {
            //update needed area
            //neededArea.GetComponent<RectTransform>().localPosition = new Vector2(neededAreaOffsetX + sensorOffsetX, neededAreaOffsetY + sensorOffsetY);
            //neededArea.GetComponent<RectTransform>().sizeDelta = new Vector2(neededAreaWidth, neededAreaHeight);

            //print(ids.Count + "  " + ids_positions.Count);
            for (int id = 0; id < ids.Count; id++)
            {
                particles[id].position = ids_positions[id];
                particles[id].startSize = 5;
            }
            //print(ids.Count);
           
            for (int tid = 0; tid < triggerAreas.Length; tid++)
            {
                triggerRawImage[tid].color = colors[tid];
                
                if (IsInsideArea_last[tid] != IsInsideArea[tid])
                {
                    udp.SocketSend(tid + "," + Convert.ToInt32(IsInsideArea[tid]) + "/e/");
                    IsInsideArea_last[tid] = IsInsideArea[tid];
                }

               


               // udp.SocketSend(tid + "," + isin +"/e/");
            }

            int centerid = canvasGridX / 2 + canvasGridY / 2 * canvasGridX;
            particles[centerid].startColor = Color.red;
            particles[centerid].startSize = 15;
            particles[centerid].position = new Vector2(sensorOffsetinPixelX, sensorOffsetinPixelY);
            pointCloudParticles.SetParticles(particles, particles.Length);


          


            //if (isFarPerspective)
            //{
            //    camera.transform.position = new Vector3(myCanvasWidth * 0.5f, myCanvasHeight * 0.5f, -1600f);
            //}
            //else
            //{
            //    camera.transform.position = new Vector3(myCanvasWidth * 0.5f, myCanvasHeight * 0.5f, -1000f);
            //}

        }
        void FixedUpdate()
        {
           

            //print("----------------------------------------------------fixedupdate");

            //Loom.RunAsync(() =>
            //{
            //    Loom.QueueOnMainThread(() =>
            //    {
            //        time += Time.deltaTime;
            //        if (SerialCommunicationFacade.isSaveAll)
            //        {
            //            isSaveAll = false;
            //            time = 0;
            //            var angless = new List<float>(angleAndDistances.Keys);
            //            for (int i = 0; i < angless.Count; i++)
            //            {
            //                float distance = angleAndDistances[angless[i]];
            //                GetPos(angless[i], distance);
            //            }
            //            angleAndDistances.Clear();
            //            angless.Clear();
            //            isSaveAll = true;
            //        }
            //    });
            //});
            // print(1 / Time.deltaTime);
        }

        //not using this function
        /*
        void GetPos(float angle, float distance)
        {
            //print(angle +"now get pos"+ distance );

            Vector2 pos;
            //convert to radians
            float angle_in_rand = angle * Mathf.Deg2Rad;

            pos = new Vector2(distance * Mathf.Sin(angle_in_rand), distance * Mathf.Cos(angle_in_rand));
            pos.x *= -0.3f;
            pos.y *= -0.3f;
            //print(pos);


            int xCount = Mathf.RoundToInt(pos.x / 3);
            int yCount = Mathf.RoundToInt(pos.y / 3);


            Vector2 snapped_pos = new Vector2((float)xCount, (float)yCount);

            if (pos.x != 0 && pos.y != 0 && pos.x < 960 && pos.x > -960 && pos.y > -540 && pos.y < 540)
            {

                xCount = xCount + canvasGridX / 2;
                yCount = yCount + canvasGridY / 2;
                int pid = xCount + canvasGridX * yCount;
                ids.Add(pid);
                //print(pid);
                //print("pid" + pid);
                // print(snapped_pos + new Vector2(canvasGridX / 2, canvasGridY / 2) + "     " + angle + "      " + distance);
                particles[pid].position = new Vector3(snapped_pos.x, snapped_pos.y, 0);
                particles[pid].startColor = new Color32(0, 0, 0, 255);
                particles[pid].startSize = 1f;

            }



        }
        */
        /// <summary>
        /// 進制轉換以及字符串重組
        /// </summary>
        string BinaryConversion(byte one, byte two)
        {
            string returnStr;
            string oneStr;
            string twoStr;

            if (one < 16)
            {
                oneStr = "0" + Convert.ToString(one, 16);//十進制轉十六進制  byte 出來的直接是十進制
            }
            else
            {
                oneStr = Convert.ToString(one, 16);
            }
            if (two < 16)
            {
                twoStr = "0" + Convert.ToString(two, 16);
            }
            else
            {
                twoStr = Convert.ToString(two, 16);
            }

            returnStr = oneStr + twoStr;
            return returnStr;

        }


    }
}