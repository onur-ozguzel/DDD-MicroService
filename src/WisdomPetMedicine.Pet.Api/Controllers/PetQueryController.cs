using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WisdomPetMedicine.Pet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetQueryController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public PetQueryController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string query = @"select p.name_value as Name,
p.breed_value as Breed,
sex = case p.sexofpet_value when 0 then 'Male' when 1 then 'Female' end,
p.color_value as color,
p.dateofbirth_value as DateOfBirth,
p.species_value as Species
from pets p";

            using var connection = new SqlConnection(configuration.GetConnectionString("Pet"));
            var pets = (await connection.QueryAsync(query)).ToList();
            return Ok(pets);
        }
    }
}
