using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class SearchModel
    {
        public string Keywords { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }

        public List<ViewModels.SearchResultModel> Result { get; set; }
    }
}