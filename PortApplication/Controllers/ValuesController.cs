using Ports.DAL;
using PortUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PortApplication.Controllers
{
    public class ValuesController : ApiController
    {
        PortHelper ufile = new PortHelper();
        // GET api/values

        [HttpGet]
        public IEnumerable<PORTDATA> Get()
        {
            var task = Task.Run(async () => await ufile.GetPort());
            var response = task.Result;
            return response;
        }

        // GET api/values/5
        [HttpGet]
        public IHttpActionResult GetbyId(int id)
        {
            var data = ufile.GetPortbyId(id);
            if (data != null)
            {
                return Ok(data);
            }
            else
            {
                return BadRequest("Port not found");
            }
        }

        // POST api/values
        [HttpPost]
        public IHttpActionResult SavePort([FromBody] PORTDATA pData)
        {
            string path = "~/Data/Ports.json";
            var task = Task.Run(async () => await ufile.AddPort(pData));
            var response = task.Result;
            if (response == "Port Added")
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        // DELETE api/values/5

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            string path = "~/Data/Ports.json";
            var task = Task.Run(async () => await ufile.Delete(id));
            var response = task.Result;
            if (response == "Port Delete successfully")
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
    }
}
