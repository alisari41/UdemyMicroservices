using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FreeCourse.Services.Catalog.Models
{
    public class Course
    {//String değilse tipini tanımla
        [BsonId]//Otomatik Id üretimi MongoDB e bıraktığımı MongoDB'e haber ediyorum
        [BsonRepresentation(BsonType.ObjectId)]//tipini belirtiyorum
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }


        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }

        public string UserId { get; set; }//Identity tarafında string tuttuğum için string belirttim

        public string Picture { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedTime { get; set; }


        public Feature Feature { get; set; }//Bire bir ilişki


        [BsonRepresentation(BsonType.ObjectId)]//Category tablosunda Id tipi neyse onu veriyorum
        public string CategoryId { get; set; }

        [BsonIgnore]//MongoDb deki collectionlara yani birer satır olarak yansıtırken bunu göz ardı et. Bunu ben kod içersinde kullancam
        public Category Category { get; set; }



    }
}
