﻿using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace PKAD_Hemo
{
    public class HemoManager
    {
        private string inputfile;
        private string exception_msg;
        public HemoManager(string inputfile)
        {
            this.inputfile = inputfile;
        }
        public void setInputfile(string inputfile)
        {
            this.inputfile = inputfile;
        }
        public string getLastException()
        {

            return this.exception_msg;
        }

        public List<HemoData> readData()
        {
            List<HemoData> data = new List<HemoData>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = header_args => header_args.Header.ToLower(),
            };
            try
            {
                using (var reader = new StreamReader(this.inputfile))
                using (var csv = new CsvReader(reader, config))
                {
                    data = csv.GetRecords<HemoData>().ToList();
                }

            }
            catch (Exception e)
            {
                //MessageBox.Show("Hello, world!", "My App");
                exception_msg = e.GetType().FullName;
                return null;
            }
            return data;
        }
    }
}
