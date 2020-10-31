using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_management.Models
{
	public class EmployeeViewModel
	{
		public string Id { get; set; }
		[Display(Name = "User Name")]
		public string UserName { get; set; }
		public string Email { get; set; }
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
		[Display(Name = "First Name")]
		public string FirstName { get; set; }
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
		[Display(Name = "Tax ID Number")]
		public string TaxId { get; set; }
		[Display(Name = "Date Of Birth")]
		public DateTime DateOfBirth { get; set; }
		[Display(Name = "Join Date")]
		public DateTime DateJoined { get; set; }
	}
}
