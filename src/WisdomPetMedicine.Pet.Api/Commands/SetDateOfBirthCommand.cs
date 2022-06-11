using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WisdomPetMedicine.Pet.Api.Commands
{
    public class SetDateOfBirthCommand
    {
        public Guid Id { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
