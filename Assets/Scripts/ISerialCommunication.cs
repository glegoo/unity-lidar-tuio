using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.SerialPortUtility.Interfaces
{
    public interface ISerialCommunication
    {
        void Connect(int baudrate, string portName);

        void Disconnect();
        void SendMessage(byte[] byteArray);
    }
}