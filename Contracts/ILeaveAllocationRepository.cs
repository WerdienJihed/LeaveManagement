using Leave_management.Data;
using System.Collections;
using System.Collections.Generic;

namespace Leave_management.Contracts
{
	public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
    {
        bool CheckAllocation(int leaveTypeId, string employeeId);
        ICollection<LeaveAllocation> GetLeaveAllocationByEmployee(string id);
    }
}
