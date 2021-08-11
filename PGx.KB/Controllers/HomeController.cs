using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FileHelpers;
//using PGx.KB.Models;
//using PGx.Model.Entities;
using System.IO;

using System.Web.Script.Serialization;
using PGx.KB.Infrastructure;
using PGx.KB.services;
namespace PGx.KB.Controllers
{
 
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        } 

    }
}
