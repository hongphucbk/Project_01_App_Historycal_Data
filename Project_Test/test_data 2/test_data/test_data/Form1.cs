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
namespace test_data
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           timer1.Enabled = false;
            timer1.Interval = 1000 * 60 * 1;
        }
        PLC plcstation = new PLC(CPU_Type.S71200, "192.168.45.24", 0, 1);
       

      
        //struct in datablock to read .
    

        private void timer1_Tick_1(object sender, EventArgs e)
        { //call funtion read data
            try
            {
                if (plcstation.IsConnected)
                {
                   
                    double m1 =(double)plcstation.Read("DB10.DBD0", VarType.Real);
                    double m2 = (double)plcstation.Read("DB10.DBD4", VarType.Real);
                    double m3 = (double)plcstation.Read("DB10.DBD8", VarType.Real);
                    double m4 = (double)plcstation.Read("DB10.DBD12", VarType.Real);
                    double m5 = (double)plcstation.Read("DB10.DBD16", VarType.Real);
                    //insert sql ;
                    //////////////////////////
                   

                     StringBuilder sbSql = new StringBuilder();
                    sbSql.Append(@"INSERT INTO MachineRuning(TagName,Valuee,TimeStamp) VALUES");
                    double[] data = { m1, m2, m3, m4, m5 };
                    for (int i = 0; i < data.Length; i++)
                    {
                        string values = string.Format("({0},{1},{2}),",
                            new string[] { "@TagName" + i.ToString(), "@Valuee" + i.ToString(), "@TimeStamp" + i.ToString() });
                        sbSql.Append(values);
                    }
                    string sql = sbSql.ToString();
                    sql = sql.TrimEnd(','); // remove the trailing comma ','
                    string conectionstring = @" Data Source = DESKTOP-TF7PT8I\WINCC; Initial Catalog = S71200; User ID = sa; Password = gcsvn";
                    SqlConnection conect = new SqlConnection(conectionstring);
                    SqlCommand cmd = new SqlCommand(sql, conect);//create command
                    conect.Open();
                    for (int j = 0; j < data.Length; j++)
                    {
                        cmd.Parameters.Add("@TagName" + j.ToString(), System.Data.SqlDbType.VarChar).Value = "ToTalTimeM_" + (j+1).ToString();
                        cmd.Parameters.Add("@Valuee" + j.ToString(), System.Data.SqlDbType.Float).Value = data[j] ;
                        cmd.Parameters.Add("@TimeStamp" + j.ToString(), System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    }
                    //insert into bang vlues(register1,3,datetime.now),(register2,5,datetime.now),()
                    cmd.ExecuteNonQuery();
                    conect.Close();
                    ///////////////display //////////////////////
                    textBox1.Text =string.Format("{0:N3}", m1);
                    textBox2.Text = string.Format("{0:N3}", m2);
                    textBox3.Text = string.Format("{0:N3}", m3);
                    textBox4.Text = string.Format("{0:N3}", m4);
                    textBox5.Text = string.Format("{0:N3}", m5);

                }
                else
                {
                    textBox6.Text = "Disconnect with PLC";
                }
            }
            catch (Exception ess)
            {
                MessageBox.Show(this,ess.Message);
                plcstation.Open();
            }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            plcstation.Open();
            timer1.Enabled = true;
            textBox6.Text = "Connected with PLC";
            
        }

        public float floatConversion(byte[] bytes,int index)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes); // Convert big endian to little endian
            }
            float myFloat = BitConverter.ToSingle(bytes, index);
            
            return myFloat;
        }
    }
}
