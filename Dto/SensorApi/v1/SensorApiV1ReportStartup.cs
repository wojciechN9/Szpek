using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Szpek.Application.SensorApi.v1
{
    public class SensorApiV1ReportStartup
    {
        [Required]
        public string FirmwareName { get; set; }
    }
}
