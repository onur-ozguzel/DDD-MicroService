using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WisdomPetMedicine.RescueQuery.Api.Controllers.V2
{    
    [ApiController]
    [ApiVersion("2.0")]
    [Route("v{version:apiVersion}/[controller]")]
    public class RescueQueryController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public RescueQueryController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<IActionResult> Get()
        {
            string sql = @"select
ram.Id,
ram.Name,
ram.Breed,
ram.Color,
Sex = case ram.Sex when 0 then 'Male' when 1 then 'Female' end,
ra.AdopterId_Value as AdopterId,
AdoptionStatus = case ra.AdoptionStatus 
    when 0 then 'None' 
    when 1 then 'Pending review'
    when 2 then 'Accepted'
    when 3 then 'Rejected' end,
a.Name_Value as AdopterName,
a.questionnaire_doyouRent,
a.questionnaire_haschildren,
a.questionnaire_hasfencedyard,
a.questionnaire_isactiveperson,
a.address_Street,
a.address_number,
a.address_city,
a.address_postalCode,
a.address_country,
a.phonenumber_value
from RescuedAnimalMetadata ram
join RescuedAnimals ra on ram.id = ra.id
left outer join Adopters a on ra.adopterId_value = a.Id";

            using var connection = new SqlConnection(configuration.GetConnectionString("Rescue"));
            var orderDetail = (await connection.QueryAsync(sql)).ToList();
            return Ok(orderDetail);
        }
    }
}
