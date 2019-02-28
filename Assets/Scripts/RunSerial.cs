using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.SerialPortUtility.Scripts
{
    public class RunSerial : MonoBehaviour
    {

        SerialCommunicationFacade facade;
        private string portName = "COM3";
        private int baudRate = 230400;
        byte[] sendMsg;
        int index = 0;

        //UI 用於顯示
        public GameObject img;
        public GameObject canvas;

        List<GameObject> imgs = new List<GameObject>();
        int MaxLength = 5000;

        List<float> angless;



        private void Awake()
        {
            //for (int i = 0; i < MaxLength; i++)
            //{
            //    GameObject obj = GameObject.Instantiate(img);
            //    obj.transform.SetParent(canvas.transform);
            //    obj.SetActive(true);
            //    imgs.Add(obj);
            //}
        }
        void Start()
        {
            facade = this.GetComponent<SerialCommunicationFacade>();
            facade.Connect(baudRate, portName);
            sendMsg = strToToHexByte("A560");
            facade.SendMessage(sendMsg);
        }

        Vector2 GetPos(float angle, float distance)
        {
            Vector2 pos;
            pos = new Vector2(distance * Mathf.Sin(angle), distance * Mathf.Cos(angle));
            return pos;
        }

        void Update()
        {
            //Loom.RunAsync(() =>
            //{
            //    Loom.QueueOnMainThread(() =>
            //    {
            //        if (SerialCommunicationFacade.isSaveAll)
            //        {
            //            print("ken test");
            //            SerialCommunicationFacade.isSaveAll = false;
            //            //Unity在Dictionary中刪除修改元素時出現InvalidOperationException: out of sync  出現這個錯誤
            //            foreach (var item in SerialCommunicationFacade.angleAndDistances)
            //            {
            //                imgs[index].transform.localPosition = GetPos(item.Key, item.Value);
            //                print(item.Key +" ~~~~~~~~~~~~~~~~~~~~~~~~~~ " + item.Value);
            //                index++;
            //            }
            //            index = 0;
            //            //angless = new List<float>(SerialCommunicationFacade.angleAndDistances.Keys);
            //            //for (int i = 0; i < angless.Count; i++)
            //            //{
            //            //float distance = SerialCommunicationFacade.angleAndDistances[angless[i]];
            //            //BuildObj(angless[i], distance);
            //            //}
            //            SerialCommunicationFacade.angleAndDistances.Clear();
            //            //angless.Clear();

            //            //SerialCommunicationFacade.angles.Clear();
            //            //SerialCommunicationFacade.distances.Clear();
            //        }
            //    });
            //});
            
        }




        private void OnApplicationQuit()
        {

            sendMsg = strToToHexByte("A565");
            facade.SendMessage(sendMsg);
            facade.Disconnect();
        }
        /// <summary>
        /// 字符串轉16進制字節數組
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        /// <summary>
        /// 字節數組轉16進制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }

}