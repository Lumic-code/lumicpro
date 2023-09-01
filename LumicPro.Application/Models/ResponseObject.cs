using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumicPro.Application.Models
{
    public class ResponseObject<T>
    {
        public int StatusCode { get; set; }
        public string DisplayMessage { get; set; }
        public List<string> ErrorMessages { get; set; }
        public T Data { get; set; }

        public ResponseObject()
        {
            ErrorMessages = new List<string>();
        }
    }
}
