using Leave_management.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leave_management.Contracts
{
	public interface ILeaveRequestRepository : IRepositoryBase<LeaveRequest>
	{
		Task<ICollection<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeId);
	}
}
