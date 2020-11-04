using Leave_management.Data;
using System.Collections.Generic;

namespace Leave_management.Contracts
{
	public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
	{
		ICollection<LeaveRequest> GetLeaveRequestsByEmployee(string employeeId);
	}
}
