using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace S7Net
{
    #region CPU_Type
    public enum CPU_Type
    {
        S7200 = 0, // index = 0 in combobox
        S7300 = 10,// index = 1 in combobox
        S7400 = 20,// index = 2 in combobox
        S71200 = 30 // index = 3 in combobox
    }
    #endregion
    #region Error Codes
    public enum ExceptionCode
    {
        ExceptionNo = 0,
        WrongCPU_Type = 1,
        ConnectionError = 2,
        IPAdressNotAvailable,

        WrongVariableFormat = 10,
        WrongNumberReceivedBytes = 11,

        SendData = 20,
        ReadData = 30,

        WriteData = 50
    }
    #endregion
    #region DataType
    public enum DataType
    {
        Input = 129,
        Output = 130,
        Marker = 131,
        DataBlock = 132,
        Timer = 29,
        Counter = 28
    }
    #endregion
    #region VarType
    public enum VarType
    {
        Bit,
        Byte,
        Word,
        DWord,
        Int,
        DInt,
        Real,
        String,
        Timer,
        Counter
    }
    #endregion
}
