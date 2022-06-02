using Nest_Homework_Partial.Data_Access_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nest_Homework_Partial.Services
{
    public class LayoutServices
    {
        private AppDbContext _context { get; }
        public LayoutServices(AppDbContext context)
        {
            _context = context;
        }
        public Dictionary<string,string> GetSettings()
        {
            return _context.Settings.ToDictionary(prd => prd.Key, prd => prd.Value);
        }
    }
}
