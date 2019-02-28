using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using Assets.SerialPortUtility.Scripts;
using System.IO.Ports;

public delegate void SerialPortMessageEventHandler(byte[] sendData);
public delegate void SerialPortSendMessageReportHandler(byte[] sendData);
public class SerialCommunication
{
    // 從串口發出消息事件
    public event SerialPortMessageEventHandler SerialPortMessageEvent;
    // 給串口發消息事件
    public event SerialPortSendMessageReportHandler SerialPortSendMessageReportEvent;
    private SerialPort serialPort;
    private Thread threadReceive;
    // 儲存接收到的消息
    private List<byte> buffer = new List<byte>(4096);
    [NonSerialized]
    private List<byte> listReceive = new List<byte>();


    private int lsnIndex = 3;
    public SerialCommunication(SerialPort serialPort)
    {
        this.serialPort = serialPort;
    }
    public SerialCommunication(string portName, int boudrate)
    {
        serialPort = new SerialPort(portName, boudrate);
    }

    public void OpenSerialPort()
    {

        //Debug.Log("讀取端口" + ConvertXml._instance.COM);
        serialPort.Open();
        serialPort.ReadTimeout = 1;
        threadReceive = new Thread(ListenSerialPort);
        threadReceive.IsBackground = true;
        threadReceive.Start();

    }
    public bool IsSerialPortIsOpen()
    {
        return serialPort.IsOpen;
    }
    public void CloseSerialPort()
    {
        if (threadReceive != null)
        {
            threadReceive.Abort();//關閉線程
            threadReceive = null;
            serialPort.Close();//關閉串口
            serialPort.Dispose();//將串口從內存中釋放掉，注意如果這裏不釋放則在同一次運行狀態下打不開此關閉的串口

        }
        Debug.Log("close thread");
    }


    /// <summary>
    /// 監聽串口,讀取串口消息
    /// </summary>
    private void ListenSerialPort()
    {
        //string recvData = "";
        //int flag = 0;
        while (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                #region 原寫法
                //int bufferSize = serialPort.ReadBufferSize;
                ////										
                //byte[] buf = new byte[bufferSize];
                //int count = serialPort.Read(buf, 0, bufferSize);
                //if (count > 9)
                //{
                //    if (SerialPortMessageEvent != null && SerialPortMessageEvent.GetInvocationList().Length > 0) // If somebody is listening
                //    {
                //        SerialPortMessageEvent.Invoke(buf);// Invoke方法防止主線程擁堵衝突
                //    }

                //}
                #endregion

                #region 使用ReadByte()的寫法 加判斷包頭和包尾               

                byte buf = Convert.ToByte(serialPort.ReadByte());
                buffer.Add(buf);
                while (buffer.Count >= 2)
                {
                    if (buffer[0] == 0xAA && buffer[1] == 0x55)
                    {
                        //Debug.Log("內層收到未處理消息");

                        if (buffer.Count < 4)
                        {
                            break;
                        }

                        int numLen = buffer[3];

                        if (buffer.Count < numLen * 2 + 10)
                        {
                            break;
                        }
                        //Debug.Log("numLen: " + numLen);
                        Data_Process(numLen, buffer);
                        //一條完整數據  存儲  進行處理  移除前面一條完整數據

                        buffer.RemoveRange(0, numLen * 2 + 10);

                    }
                    else
                    {
                        buffer.RemoveAt(0);
                    }
                }
                //Debug.Log("buffer.Count: " + buffer.Count + "  " + RunSerial.byteToHexStr(buffer.ToArray()));
                #endregion

            }
            catch (System.Exception e)
            {
                //Debug.LogWarning(e.Message);
            }
        }
    }


    void Data_Process(int numLen, List<byte> bufferSrc)
    {
        byte[] readBuffer = null;
        readBuffer = new byte[numLen * 2 + 10];
        bufferSrc.CopyTo(0, readBuffer, 0, numLen * 2 + 10);
        SerialPortMessageEvent(readBuffer);// Invoke方法防止主線程擁堵衝突
    }

    #region
    /// <summary>
    /// ASCII碼轉字符：
    /// </summary>
    /// <param name="asciiCode"></param>
    /// <returns></returns>
    public static string Chr(int asciiCode)
    {
        if (asciiCode >= 0 && asciiCode <= 255)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)asciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        else
        {
            throw new Exception("ASCII Code is not valid.");
        }
    }


    /// <summary>
    /// 使用事件觸發方式來實現串口數據的讀取
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        byte[] Resoursedata = new byte[serialPort.BytesToRead];
        int count = serialPort.Read(Resoursedata, 0, Resoursedata.Length);//在此就可以讀取到當前緩衝區內的數據
        //執行數據操作
        serialPort.DiscardInBuffer();//丟棄傳輸緩衝區數據
        serialPort.DiscardOutBuffer();//每次丟棄接收緩衝區的數據
        if (count > 0)
        {
            if (SerialPortMessageEvent != null && SerialPortMessageEvent.GetInvocationList().Length > 0) // If somebody is listening
            {
                SerialPortMessageEvent.Invoke(Resoursedata);// Invoke方法防止主線程擁堵衝突
            }
        }
    }
    #endregion


    /// <summary>
    /// 給串口發消息
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>
    public bool SendMessageFromSerialPort(byte[] byteArray)
    {
        if (serialPort != null && serialPort.IsOpen == true)
        {

            serialPort.Write(byteArray, 0, byteArray.Length);

            if (SerialPortSendMessageReportEvent != null && SerialPortSendMessageReportEvent.GetInvocationList().Length > 0) // If somebody is listening
            {
                SerialPortSendMessageReportEvent(byteArray);
            }
            return true;
        }
        else
        {
            return false;
        }
    }


}