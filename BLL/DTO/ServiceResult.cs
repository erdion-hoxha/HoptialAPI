using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class ServiceResult
    {
        public bool Result{ get; set; }

        public List<string> ErrorMessage { get; set; }
    }
}
