using Microsoft.AspNetCore.Authorization;

namespace WebApp_WithIdentity.Authorization
{
    public class HRManagerProbationRequirement:IAuthorizationRequirement
    {
        public HRManagerProbationRequirement(int probationMonths)
        {
            ProbationMonths = probationMonths; 
        }
        public int ProbationMonths { get;}
    }

    public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
        {
            // we will get the claims first
            if (!context.User.HasClaim(x=>x.Type=="EmploymentDate"))
                return Task.CompletedTask; // we will return empty task here.

            //==========we just access the claim value and parse that.
            var empDate = DateTime.Parse(context.User.FindFirst(x => x.Type == "EmploymentDate").Value);

            //if the hr has probation period then he can only access the hr page if pass probation period.

            var period = DateTime.Now - empDate; // check the difference the probation period to now date.

            if (period.Days > 30 * requirement.ProbationMonths) //if day of period greater than our requirements
                context.Succeed(requirement);

            return Task.CompletedTask; // after we send empty task
        }
    }
}
