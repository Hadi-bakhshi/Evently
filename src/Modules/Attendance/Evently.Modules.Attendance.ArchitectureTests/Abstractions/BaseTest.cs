using System.Reflection;
using Evently.Modules.Attendance.Domain.Attendees;
using Evently.Modules.Attendance.Infrastructure;


namespace Evently.Modules.Attendance.ArchitectureTests.Abstractions;

#pragma warning disable CA1515 // Consider making public types internal
public abstract class BaseTest
#pragma warning restore CA1515 // Consider making public types internal
{
    protected static readonly Assembly ApplicationAssembly = typeof(Application.AssemblyReference).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Attendee).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(AttendanceModule).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(Attendance.Presentation.AssemblyReference).Assembly;
}
