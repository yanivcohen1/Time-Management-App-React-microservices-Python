using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthApi.Models;

public class ApplicationUser
{
    [Key]
    [BsonId]
    [BsonRepresentation(BsonType.Int32)]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public required string PasswordHash { get; set; }
}
