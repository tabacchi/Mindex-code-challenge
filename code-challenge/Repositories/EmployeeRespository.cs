using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        public ReportingStructure GetReportingStructureById(String id)
        {
            ReportingStructure reportingStructure = new ReportingStructure
            {
                Employee = _employeeContext
                    .Employees
                    .Include(i => i.DirectReports)
                    .FirstOrDefault(i => i.EmployeeId == id),

                NumberOfReports = GetNumberOfReportsById(id)
            };

            return reportingStructure;
        }

        private int GetNumberOfReportsById(String id)
        {
            int result = 0;
            Queue<Employee> myQueue = new Queue<Employee>(_employeeContext.Employees.Include(i => i.DirectReports).FirstOrDefault(i => i.EmployeeId == id).DirectReports);
            while (myQueue.Count > 0)
            {
                Employee employee = myQueue.Dequeue();
                result += 1;
                IEnumerable<Employee> directReports = _employeeContext.Employees.Include(i => i.DirectReports).FirstOrDefault(i => i.EmployeeId == employee.EmployeeId).DirectReports;
                foreach (var directReport in directReports)
                    myQueue.Enqueue(directReport);
            }

            return result;
        }
    }
}
