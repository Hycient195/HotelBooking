using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace HotelBooking.Data
{
	public class Country
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

		public string Name { get; set; }

		public string Shortname { get; set; }

		public virtual IList<Hotel> Hotels { get; set; }
	}
}

