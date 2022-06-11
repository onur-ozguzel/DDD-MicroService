using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WisdomPetMedicine.Rescue.Api.Commands
{
    public class SetAdopterPhoneNumberCommand
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
    }
}
