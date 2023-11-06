using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    public class ODataService
    {
        private readonly ILogger<ODataService> _logger;
        private readpmöy ODataResponseParser _parser:

        public ODataService(ILogger<ODataService> logger, ODataResponseParser parser) 
        {
            _logger = logger;
        }


    }
}
