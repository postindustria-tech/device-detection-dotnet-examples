/* *********************************************************************
 * This Original Work is copyright of 51 Degrees Mobile Experts Limited.
 * Copyright 2025 51 Degrees Mobile Experts Limited, Davidson House,
 * Forbury Square, Reading, Berkshire, United Kingdom RG1 3EU.
 *
 * This Original Work is licensed under the European Union Public Licence
 * (EUPL) v.1.2 and is subject to its terms as set out below.
 *
 * If a copy of the EUPL was not distributed with this file, You can obtain
 * one at https://opensource.org/licenses/EUPL-1.2.
 *
 * The 'Compatible Licences' set out in the Appendix to the EUPL (as may be
 * amended by the European Commission) shall be deemed incompatible for
 * the purposes of the Work and the provisions of the compatibility
 * clause in Article 5 of the EUPL shall not apply.
 *
 * If using the Work as, or as part of, a network application, by
 * including the attribution notice(s) required under Article 5 of the EUPL
 * in the end user terms of the application under an appropriate heading,
 * such notice(s) shall fulfill the requirements of that article.
 * ********************************************************************* */

using Microsoft.AspNetCore.Mvc;
using FiftyOne.Pipeline.Web.Services;
using FiftyOne.Pipeline.Core.FlowElements;

namespace Cloud_Client_Hints_Not_Integrated.Controllers
{
    public class HomeController : Controller
    {
        private IPipeline _pipeline;
        private IWebRequestEvidenceService _evidenceService;

        public HomeController(IPipeline pipeline,
            IWebRequestEvidenceService evidenceService)
        {
            _pipeline = pipeline;
            _evidenceService = evidenceService;
        }

        public IActionResult Index()
        {
            using(var flowData = _pipeline.CreateFlowData())
            {
                // Add evidence
                _evidenceService.AddEvidenceFromRequest(flowData, Request);
                // Process
                flowData.Process();
                // Set response headers
                SetHeaderService.SetHeaders(Response.HttpContext, flowData);
                // Send results to view
                return View(flowData);
            }
        }
    }
}
