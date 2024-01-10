using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using LinqToExcel;
//using LinqToExcel;
namespace Timesheet.Common
{
    public static class Functions
    {
        #region Extension Methods
        /*to convert a list into a data table */
        public static DataTable ConvertToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);

            }
            return table;

        }
        
        /*to convert a list modal to an xml*/
        public static string CreateXml<T>(this IList<T> modal)
        {
            var xmlDoc = new System.Xml.XmlDocument();

            var xmlSerializer = new XmlSerializer(modal.GetType());

            using (var xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, modal);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        public static List<Row> ReadExcel<T>(string excelFilePath)
        {
            var Excel = new ExcelQueryFactory(excelFilePath);

            List<Row> Data = Excel.Worksheet("Timesheet").ToList();
            return Data;



        }

        #endregion
    }
}
