using Leave_management.Data;
using System.Collections.Generic;

namespace Leave_management.Contracts
{
	public interface ILeaveTypeRepository : IRepositoryBase<LeaveType>
    {
        ICollection<LeaveType> GetEmployeesByLeaveType(int id);
    }
}
