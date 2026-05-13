using System;
using System.Collections.Generic;

namespace PlantShop.Models;

public partial class Video
{
    public int VideoId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string YoutubeId { get; set; } = null!;

    public string? Thumbnail { get; set; }

    public int? Duration { get; set; }

    public int? Views { get; set; }

    public string? Category { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public int? SortOrder { get; set; }
}
