namespace Workforce360.Api.Domain;
public sealed class Department { public int Id {get;set;} public string Name {get;set;}=""; public ICollection<Employee> Employees {get;set;}=new List<Employee>(); }
public sealed class Employee { public int Id {get;set;} public string EmployeeCode {get;set;}=""; public string FullName {get;set;}=""; public string Email {get;set;}=""; public int DepartmentId {get;set;} public Department Department {get;set;}=null!; public bool IsActive {get;set;}=true; public DateOnly DateJoined {get;set;} }
public enum LeaveStatus { Pending=0, Approved=1, Rejected=2 }
public sealed class LeaveRequest { public int Id {get;set;} public int EmployeeId {get;set;} public Employee Employee {get;set;}=null!; public DateOnly StartDate {get;set;} public DateOnly EndDate {get;set;} public string Reason {get;set;}=""; public LeaveStatus Status {get;set;}=LeaveStatus.Pending; public DateTime CreatedUtc {get;set;}=DateTime.UtcNow; }
