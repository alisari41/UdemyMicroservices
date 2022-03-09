using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Dtos
{
    public class CourseCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; }
        public string UserId { get; set; }//Identity tarafında string tuttuğum için string belirttim
        public FeatureDto Feature { get; set; }//Bire bir ilişki
        public string CategoryId { get; set; }
    }
}
