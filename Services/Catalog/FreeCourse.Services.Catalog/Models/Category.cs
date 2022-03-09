using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FreeCourse.Services.Catalog.Models
{
    public class Category
    {
        [BsonId]//Otomatik Id üretimi MongoDB e bıraktığımı MongoDB'e haber ediyorum
        [BsonRepresentation(BsonType.ObjectId)]//tipini belirtiyorum
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
