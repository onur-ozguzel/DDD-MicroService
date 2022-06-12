using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomPetMedicine.Hospital.Infrastructure
{
    public static class HealthChecksBuilderExtensions
    {
        public static IHealthChecksBuilder AddCosmosDbCheck(this IHealthChecksBuilder builder, IConfiguration configuration)
        {
            return builder.Add(new HealthCheckRegistration("WisdomPetMedicine", new WisdomPetMedicineCosmosDbHealthCheck(configuration), HealthStatus.Unhealthy, null));
        }
    }
}
