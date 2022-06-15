using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TCP_Yinyang
{
    public class FormatData
    {

        #region make singleton
        private static FormatData instance;
        public static FormatData Instance
        {
            get
            {
                if (instance == null) instance = new FormatData(); return instance;
            }
            private set { instance = value; }
        }

        private FormatData() { }
        #endregion



        /// <summary>
        /// Convert Object to Byte[]
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public byte[] SerializeData(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }

        /// <summary>
        /// Convert Byte[] to Object
        /// </summary>
        /// <param name="theByteArray"></param>
        /// <returns></returns>
        public object DeserializeData(byte[] theByteArray)
        {
            try
            {
                MemoryStream ms = new MemoryStream(theByteArray);
                BinaryFormatter bf1 = new BinaryFormatter();
                ms.Position = 0;
                return bf1.Deserialize(ms);
            }
            catch
            {
                return null;
            }
        }



        /*Ở đây tôi tạo 1 bảng dữ liệu thử.
 Bạn có thể kết nối csdl và load dữ liệu lên*/
        public DataTable getdata()
        {
            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add(new DataColumn("IntegerValue", typeof(Int32)));
            dt.Columns.Add(new DataColumn("StringValue", typeof(string)));
            dt.Columns.Add(new DataColumn("DateTimeValue", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("BooleanValue", typeof(bool)));

            for (int i = 1; i <= 100; i++)
            {
                dr = dt.NewRow();
                dr[0] = i;
                dr[1] = "Item " + i.ToString();
                dr[2] = DateTime.Now;
                dr[3] = (i % 2 != 0) ? true : false;

                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
