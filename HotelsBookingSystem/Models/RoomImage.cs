using HotelsBookingSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class RoomImage
{
    [Key]
    public int ImageId { get; set; }

    [Required]
    [RegularExpression(@"^.*\.(jpg|jpeg|png)$", ErrorMessage = "Only .jpg, .jpeg, .png files are allowed.")]
    public string ImageUrl { get; set; }

    public string? Caption { get; set; }

    public bool? IsPrimary { get; set; }

    [ForeignKey("Room")]
    public int RoomId { get; set; }

    public virtual Room Room { get; set; }
}
