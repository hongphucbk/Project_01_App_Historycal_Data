using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using S7Net;
using System.Data.Sql;
using System.Data.SqlClient;
using S7Net.Types;
using System.Threading;
using Microsoft.Win32;
using System.Globalization;

namespace test_data
{
    public partial class Form1 : Form
    {
        class globaldata
        {

            public byte[] byteArray1Pre = new byte[65];
            public byte[] byteArray2Pre = new byte[65];


        }
       
        PLC plcstation = new PLC(CPU_Type.S7200, "192.168.1.100", 0, 1);

        byte[] StartHourPre = new byte[65];//start hour 
        byte[] EndHourPre = new byte[65];//end hour

        byte[] StartMinPre = new byte[65];//startmin
        byte[] EndMinPre = new byte[65];//end min
     
        byte[] byteArray1_h = new byte[65 ];//start hour 
        byte[] byteArray1_M = new byte[65];//start min.

        byte[] byteArray2_h = new byte[65 ];//end min
        byte[] byteArray2_M = new byte[65 ];//end min
        byte[] startdate = new byte[65 ];//start date
        byte[] enddate = new byte[65];//end date
        //struct in datablock to read .

        bool[] DataLine = new bool[65];
       // string conectionstring = @"Data Source=egvnt08;Initial Catalog=tmpForProduction;Integrated Security=False;User ID=andonuser;Password=andon12User;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        string conectionstring = @"Data Source=DUYTRONGPC\WINCC;Initial Catalog=S71200;Persist Security Info=True;User ID=sa;Password=gcsvn";






        public Form1()
        {
            InitializeComponent();


            timer1.Interval = 1000 * 3 * 1;//
            plcstation.Open();
            timer1.Enabled = true;
            textBox6.Text = "Connected with PLC";
            timer1.Start();
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


            rkApp.SetValue("ApplicationName", Application.ExecutablePath);
            //conect.Open();


        }

        // int len = 0;
        //  int xuong = 0;
        // int temp = 1;
        int temp = 0;
        int index = 0;
        private void timer1_Tick_1(object sender, EventArgs e)
        { //call funtion read data
            try
            {
                if (plcstation.IsConnected)
                {
                    //    timer1.Enabled = false;
                    textBox6.Text = "PLC connected";
                    // bool data = false;
                    DataType DataType = S7Net.DataType.DataBlock;
                    int DB = 9;
                    int StartByteAdr = 10;//start address.
                    int count = 65 ; // Reads 20 Words.
                    //read time hour start
                    byte[] byteArray1_h = plcstation.ReadBytes(DataType, DB, StartByteAdr, count);//read start 0 -- 64 65 start 
                    //read time min start

                    byte[] byteArray1_M = plcstation.ReadBytes(DataType, DB, 76, count);//read end
                    byte[] byteArray2_h = plcstation.ReadBytes(DataType, DB, 142, count);//read start 0 -- 64 65 start 
                    //read time min start

                    byte[] byteArray2_M = plcstation.ReadBytes(DataType, DB, 208, count);//read end
                    ///////////////
                    byte[] startdate = plcstation.ReadBytes(DataType, DB, 274, 65 );
                    byte[] enddate = plcstation.ReadBytes(DataType, DB, 340, 65);

                     timer1.Stop();

                    if ((byteArray1_h[index] != StartHourPre[index] || byteArray1_M[index ] != StartMinPre[index]) && byteArray1_h[index] != 0)
                    {
                        //get data folow: date month year hour min
                        //  insert data base
                         InsertSQLStart(index, byteArray1_h, byteArray1_M, startdate[index]);//index i///start //end i+65
                        //Thread.Sleep(1000);
                    }

                    StartHourPre[index] = byteArray1_h[index];//start hour
                    StartMinPre[index] = byteArray1_M[index];//start min
                    if (byteArray2_h[index] != 0)
                    {
                        InsertSQLEnd(index, byteArray2_h,byteArray2_M, enddate[index ]);
                       // Thread.Sleep(1000);
                        dmm.Text = "End date undated in line" + (index+1).ToString();
                    }



                    else
                    {
                        dmm.Text = "";
                    }
                    
                  
                    start.Text = byteArray1_h[index].ToString() + ":" + byteArray1_M[index ].ToString();
                    end.Text = byteArray2_h[index ].ToString() + ":" + byteArray2_M[index ].ToString();

                 

                    temp = temp + 1;
                    Status.Text = (index+1).ToString();
                    index = index + 1;

                    if (index > 64)
                    {
                        index = 0;
                    }
                     timer1.Start();
                }


                if (plcstation.IsConnected == false)
                {
                    textBox6.Text = "Disconnect with PLC";
                    plcstation.Open();
                }



            }
            catch (Exception ess)
            {
                //  MessageBox.Show(this,ess.Message);
                plcstation.Open();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            plcstation.Open();
            timer1.Enabled = true;
            textBox6.Text = "Connected with PLC";
            timer1.Start();
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


            rkApp.SetValue("ApplicationName", Application.ExecutablePath);
            SqlConnection conect = new SqlConnection(conectionstring);







        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void end_TextChanged(object sender, EventArgs e)
        {

        }
        public void InsertSQLStart(int Index, byte[] byteArray_h, byte[] byteArray_M, int daystart)
        {
            string StartTime = "";//dd-mm-yyyy-hh-mm-sec
            string min = "";
            if (byteArray_M[Index ] < 10)
            {
                min = "0" + byteArray_M[Index ].ToString();
            }
            else
            {
                min = byteArray_M[Index ].ToString();
            }
            //match data

            StartTime = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + daystart.ToString() + " " + byteArray_h[Index].ToString() + ":" + min + ":00";
            //
            DateTime start_con = DateTime.Parse(StartTime);
            //start_con = ()
            ///////////sinsert 1 record///////////////////////////////
            //mode 1 insert start 2 insert end

            StringBuilder sbSql = new StringBuilder();

            //sbSql.Append(@"INSERT INTO AndonSewingSignal(Date,Start,Line) VALUES");
            sbSql.Append(@"INSERT INTO CapTime(Date,Start,Line) VALUES");

            string values = string.Format("({0},{1},{2}),",
                    new string[] { "@Date", "@Start", "@Line" });
            sbSql.Append(values);



            string sql = sbSql.ToString();
            sql = sql.TrimEnd(','); // remove the trailing comma ','

            SqlConnection conect = new SqlConnection(conectionstring);
            conect.Open();

            SqlCommand cmd = new SqlCommand(sql, conect);//create command

            cmd.Parameters.Add("@Date", System.Data.SqlDbType.SmallDateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Start", System.Data.SqlDbType.SmallDateTime).Value = start_con;
            // cmd.Parameters.Add("@End", System.Data.SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@Line", System.Data.SqlDbType.VarChar).Value = "Line_" + (Index + 1).ToString();



            cmd.ExecuteNonQuery();
            conect.Close();
            conect.Dispose();
        }
        private void InsertSQLEnd(int Index, byte[] byteArray2_h, byte[] byteArray2_m, int dayend)
        {

            string Line = "Line_" + (Index + 1).ToString();
            int min;
            string dayy = "";
            

            string dateTimeString = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + dayend.ToString() + " " + byteArray2_h[Index].ToString() + ":" + byteArray2_m[index].ToString();

           // string conectionstring1 = @"Data Source=egvnt08;Initial Catalog=tmpForProduction;Integrated Security=False;User ID=andonuser;Password=andon12User;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using (SqlConnection connection = new SqlConnection(conectionstring))
            using (SqlCommand command = connection.CreateCommand())

            {


                //  cmd.CommandText = "Update CapTime set [End] ='" + EndTime + "' where Line='" + Line.ToString() + "'";
                //cmd.CommandText = "Update CapTime set [End] = '" + end_con + "' where AutoID = (select Max(AutoID) from CapTime ) and Line = '" + Line.ToString() + "'";
                command.CommandText = "Update CapTime set [End] = cast ( '" + dateTimeString + "' AS smalldatetime) where AutoID = (select Max(AutoID) from CapTime where Line = '" + Line.ToString() + "')";
                //      command.CommandText = "Update AndonSewingSignal set [End] = cast ( '" + dateTimeString + "' AS smalldatetime) where AutoID = (select Max(AutoID) from AndonSewingSignal where Line = '" + Line.ToString() + "')";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {











        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
         
   
}
