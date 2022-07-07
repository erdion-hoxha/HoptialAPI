using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.Requests
{
    public class RoleCreateDto
    {

        [Required]
        public string Role { get; set; }
    }
}
