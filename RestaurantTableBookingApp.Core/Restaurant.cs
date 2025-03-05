﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantTableBookingApp.Core;

public partial class Restaurant
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string Address { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [InverseProperty("Restaurant")]
    public virtual ICollection<RestaurantBranch> RestaurantBranches { get; set; } = new List<RestaurantBranch>();
}
