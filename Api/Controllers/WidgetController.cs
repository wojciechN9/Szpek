using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Szpek.Api.Mappings;
using Szpek.Application.Location;
using Szpek.Application.Meassurement;
using Szpek.Core.Interfaces;

namespace Szpek.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMeassurementRepository _meassurementRepository;

        public WidgetController(ILocationRepository locationRepository, IMeassurementRepository meassurementRepository)
        {
            _locationRepository = locationRepository;
            _meassurementRepository = meassurementRepository;
        }

        [HttpGet("{locationId}")]
        [AllowAnonymous]
        public async Task<ContentResult> GetCurrentMeassurement(long locationId)
        {
            var location = await _locationRepository.GetActiveNonPrivateWithAddress(locationId);
            if (location == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = 200,
                    Content = "LOCATION_WITH_GIVEN_ID_NOT_EXIST"
                };
            }

            var latestMeassurement = await _meassurementRepository.GetLatest(locationId);
            if (latestMeassurement == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = 200,
                    Content = "NO_MEASSUREMENTS_SINCE_LAST_24H"
                };
            }

            var locationMeassurementRead = new LocationMeassurementsRead(
                location.Id,
                location.IsActive,
                location.Address.ToAddressRead(),
                new List<MeasurementRead>() { latestMeassurement.ToMeasurementRead() });

            return new ContentResult
            {
                ContentType = "text/html",                
                StatusCode = 200,
                Content =
                @"<html>
                    <head>
                        <meta charset=""utf-8"">
                        <style>
                        body {
                          font-family: -apple-system,BlinkMacSystemFont,""Segoe UI"",Roboto,""Helvetica Neue"",Arial,""Noto Sans"",sans-serif,""Apple Color Emoji"",""Segoe UI Emoji"",""Segoe UI Symbol"",""Noto Color Emoji"";
                        }                       

                        a {
                            color:inherit;
                            text-decoration: none;
                        }

                        .card {
                          border: 1px solid rgba(0,0,0,.125);
                          border-radius: .25rem;
                        }

                        .content {
                          flex: 1 1 auto;
                        }

                        .card-body {
                          display: flex;
                          cursor: pointer;
                          height: 4rem;
                          padding: 1rem;
                        }
                         
                        .status {
                          width: 4rem;
                          height: 4rem;
                        }

                        .circle {
                          margin: 0.8rem 1rem 1rem .75rem;
                          text-align: center;
                          height: 2rem;
                          width: 2rem;
                          background-color: #bbb;
                          border-radius: 50%;
                          display: inline-block;
                          background-color: " + GetQualityColor(locationMeassurementRead.Meassurements.First(m => m.SmogMeasurement != null).SmogMeasurement.AirQuality) + @";
                        }

                        .info {
                          padding-left: 1rem;
                        }

                        .card-title {
                          font-size: 1.5rem;
                          font-weight: 600;
                        }
                        
                         .card-subtitle {
                          font-size: 1rem;
                          font-weight: 500;
                          color: #6c757d;
                        }   

                        .clock-component {
                          margin-top: -0.25rem;
                          font-size: 0.75rem;
                        }

                        .card-footer {
                          display: flex;
                          background-color: initial;
                          justify-content: space-between;
                          height: 2rem;
                          border-top: 1px solid rgba(0,0,0,.125);
                          padding: 0.5rem;
                        }

                        .measurements {
                          display: flex;
                        }

                        .measurement-content {
                          width: 4.5rem;
                          margin-top: -0.25rem;
                          text-align: center;
                        }

                        .measurement-name {
                          font-size: 0.7rem;
                        }

                        .measurement-value {
                          font-size: 1.5rem;
                          margin-top: -0.5rem;
                          margin-bottom: -0.5rem;
                          color: black;
                        }

                        .company-name{
                          font-weight: 600;
                          display: flex;
                          align-items: center;
                          padding: 1rem;
                        }

                        .company-name:hover {
                          background: #f7f7f7;
                        }


                        </style>

                    </head>
                    <body>              
                     <a href=""https://szpek.pl"" rel = ""dofollow"" target = ""_blank"" class=""linkwrap"">                   
                      <div class=""card"" style=""width: 18rem; "">
                       <div class=""content"">
                        <div class=""card-body"">
                         <div class=""status"">
                          <div class=""circle""></div>
                         </div>
                         <div class=""info"">
                           <div class=""card-title"">" + locationMeassurementRead.Address.City + @"</div>
                           <div class=""card-subtitle mb-2 text-muted"">" + locationMeassurementRead.Address.Street + @"</div>                    
                         </div>
                        </div>
                        <div class=""card-footer"">
                          <div class=""measurements"">
                           <div class=""measurement-content"">
                            <div class=""measurement-name"">PM 10</div>
                            <div class=""measurement-value"">" + Math.Round(locationMeassurementRead.Meassurements.First(m => m.SmogMeasurement != null).SmogMeasurement.Pm10Value, 1) + @"</div>
                           </div>
                          <div class=""measurement-content"">
                           <div class=""measurement-name"">PM 2.5</div>
                           <div class=""measurement-value"">" + Math.Round(locationMeassurementRead.Meassurements.First(m => m.SmogMeasurement != null).SmogMeasurement.Pm25Value, 1) + @"</div>
                          </div>
                         </div>
                         <div class=""company-name"">Szpek.pl</div>
                        </div>
                       </div>
                      </div>
                     </a>
                    </body>
                   </html>"
            };
        }

        private string GetQualityColor(AirQuality airQuality)
        {
            return airQuality switch
            {
                AirQuality.VeryGood => "#56b207",
                AirQuality.Good => "#b0dd10",
                AirQuality.Ok => "#ffd912",
                AirQuality.Poor => "#e48100",
                AirQuality.Bad => "#e50000",
                AirQuality.VeryBad => "#9a0000",
                AirQuality.Error => "#bfbfbf",
                _ => "#ffffff",
            };
        }
    }
}