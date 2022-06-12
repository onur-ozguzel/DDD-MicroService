using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisdomPetMedicine.Hospital.Domain.Entities;

namespace WisdomPetMedicine.Hotpital.Projector
{
    public static class SqlConnectionExtensions
    {
        public static void EnsurePatientsTable(this SqlConnection conn)
        {
            var query = @"if (not exists (select *
from information_schema.tables
where table_schema = 'dbo'
and table_name = 'Patients'))
create table dbo.Patients (
[Id] [uniqueidentifier] NOT NULL,
[BloodType] [nvarchar](max) NULL,
[Weight] [decimal](18,2) NULL,
[Status] [nvarchar](max) NOT NULL,
[UpdatedOn] [datetime] NOT NULL
constraint [PK_Patients] primary key clustered ([Id] ASC))";

            conn.Execute(query);
        }

        public static void InsertPatient(this SqlConnection conn, Patient patient)
        {
            conn.Execute(@"delete from patients where id = @id
insert into patients (Id, BloodType, Weight, Status, UpdatedOn) values (@Id, @BloodType, @Weight, @Status, GETUTCDATE())", 
new { Id = patient.Id, BloodType = patient.BloodType?.Value, Weight = patient.Weight?.Value, Status = Enum.GetName(patient.Status) });
        }
    }
}
