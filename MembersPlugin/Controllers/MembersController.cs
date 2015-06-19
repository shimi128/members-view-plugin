using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using MembersPlugin.Models;
using MembersPlugin.Services.Memebers;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace MembersPlugin.Controllers
{
    
    
    public class MembersController : UmbracoAuthorizedApiController
    {
        private readonly IMembersService _membersService;

        public MembersController()
        {
            _membersService = new MembersService();
        }


        public CustomMember Get(int pageIndex, int pageSize)
        {
            var members = _membersService.GetAllMembers(pageIndex, pageSize);
            return members;
        }


        public HttpResponseMessage GetExcel()
        {
            var workbook = _membersService.ExecuteMemberToExcel();
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "Members" + DateTime.Now.ToShortDateString() + ".xlsx"
            };
            return response;
        }
    }
}
