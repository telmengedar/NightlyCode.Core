using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NightlyCode.Core.Collections;
using NightlyCode.Core.Conversion;

namespace NightlyCode.Core.Data
{

    /// <summary>
    /// table containing data
    /// </summary>
    public class DataTable
    {
        string[] columnnames;
        readonly List<string[]> data=new List<string[]>(); 

        /// <summary>
        /// indexer using column and row indices
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string this[int row, int column] => data[row][column];

        /// <summary>
        /// indexer using column name and row index
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string this[int row, string column]
        {
            get { return this[row, columnnames.IndexOf(s => s == column)]; }
        }

        /// <summary>
        /// number of columns in table
        /// </summary>
        public int ColumnCount => data.FirstOrDefault()?.Length ?? 0;

        /// <summary>
        /// number of rows in table
        /// </summary>
        public int RowCount => data.Count;

        /// <summary>
        /// column names
        /// </summary>
        public string[] Columns => columnnames;

        /// <summary>
        /// get value of a cell
        /// </summary>
        /// <typeparam name="T">type of value to get</typeparam>
        /// <param name="row">row index of cell</param>
        /// <param name="column">column index of cell</param>
        /// <returns>value of cell</returns>
        public T GetValue<T>(int row, int column)
        {
            return Converter.Convert<T>(this[row, column]);
        }

        /// <summary>
        /// get value of cell
        /// </summary>
        /// <typeparam name="T">type of value to get</typeparam>
        /// <param name="row">row index of cell</param>
        /// <param name="column">name of column</param>
        /// <returns>value of cell</returns>
        public T GetValue<T>(int row, string column)
        {
            return Converter.Convert<T>(this[row, column]);
        }

        /// <summary>
        /// reads a csv data table from stream
        /// </summary>
        /// <param name="stream">stream to use as datasource</param>
        /// <param name="delimiter">delimiter to use for cells</param>
        /// <param name="readheaderline">whether to read first line as column description</param>
        /// <returns>datatable containing read data</returns>
        public static DataTable ReadCSV(Stream stream, char delimiter=';', bool readheaderline=false)
        {
            DataTable table = new DataTable();

            using (StreamReader sr = new StreamReader(stream))
            {
                if(readheaderline && !sr.EndOfStream) {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                        table.columnnames = line.Split(delimiter);
                }

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    table.data.Add(line.Split(delimiter));
                }
            }

            return table;
        }

        /// <summary>
        /// deserializes items from the table
        /// </summary>
        /// <typeparam name="T">type of items to deserialize</typeparam>
        /// <returns>enumeration of items</returns>
        public IEnumerable<T> Deserialize<T>() {
            List<Tuple<PropertyInfo, int>> columnspec = new List<Tuple<PropertyInfo, int>>();
            foreach(PropertyInfo property in typeof(T).GetProperties()) {
                if(property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType == typeof(string)) {
                    int index = Array.IndexOf(columnnames, property.Name);
                    if(index > -1)
                        columnspec.Add(new Tuple<PropertyInfo, int>(property, index));
                }
            }

            foreach(string[] cells in data) {
                T item = (T)Activator.CreateInstance(typeof(T));
                foreach(Tuple<PropertyInfo, int> property in columnspec)
                    property.Item1.SetValue(item, Converter.Convert(cells[property.Item2], property.Item1.PropertyType, true), null);
                yield return item;
            }
        }
    }
}