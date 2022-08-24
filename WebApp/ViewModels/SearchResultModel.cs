using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.ViewModels
{
    public class SearchResultModel
    {
        public int Id { get; set; }
        public string PagePath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}