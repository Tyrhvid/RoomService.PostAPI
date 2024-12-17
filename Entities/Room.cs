using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RoomService.PostAPI.Entities;

public class Room
{
    [BsonRepresentation(BsonType.ObjectId), BsonId]
    public string? Id { get; set; }

    public required string Name { get; set; }

    public int Price { get; set; }

    public int Size { get; set; }
}