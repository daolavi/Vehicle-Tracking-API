using System;
using System.Collections.Generic;
using System.Text;

namespace VTA.Models.Response
{
    public class Result<T>
    {
        public Result(T data)
        {
            Data = data;
            ErrorMessage = string.Empty;
        }

        public Result(T data, string errorMessage)
        {
            Data = data;
            ErrorMessage = errorMessage;
        }

        public T Data { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsError => !string.IsNullOrEmpty(ErrorMessage);
    }
}
