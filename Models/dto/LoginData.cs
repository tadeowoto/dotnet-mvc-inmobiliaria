
using System.ComponentModel.DataAnnotations;

namespace inmobiliaria.Api.Controllers
{
    public class LoginData
    {
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        [DataType(DataType.Password)]
        public string password { get; set; }


    }
}