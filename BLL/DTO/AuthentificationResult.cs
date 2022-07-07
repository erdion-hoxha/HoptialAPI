using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class AuthentificationResult
    {
        public string Token { get; set; }

        public bool  Success { get; set; }

        public List<string> ErrorMessage { get; set; }

    }
}
