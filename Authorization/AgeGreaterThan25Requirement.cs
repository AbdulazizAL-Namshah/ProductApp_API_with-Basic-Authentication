using Microsoft.AspNetCore.Authorization;

namespace ProductApp_API.Authorization;

public class AgeGreaterThan25Requirement:IAuthorizationRequirement
{
}
