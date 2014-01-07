using System;

namespace AbacusLab.DataExtractionTool.Entitys.Html
{
    public class HtmlParserParameterList
    {
        private string _fileSavePath;
        private string _html;

        public string Html
        {
            get
            {
                if (String.IsNullOrEmpty(_html) || String.IsNullOrWhiteSpace(_html))
                    throw new InvalidOperationException("Html document can`t be empty");
                return _html;
            }
            set { _html = value; }
        }

        public string FileSavePath
        {
            get
            {
                if (String.IsNullOrEmpty(_html) || String.IsNullOrWhiteSpace(_html))
                    throw new InvalidOperationException("Save file path can`t be empty");
                return _fileSavePath;
            }
            set { _fileSavePath = value; }
        }

        public string AdditionalParameterOne { get; set; }
    }
}