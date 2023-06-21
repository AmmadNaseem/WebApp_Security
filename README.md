# WebApp_Security

ASP.NET Core Security:
1.	User interact to the browser and enter the credentials and send the http request to server.
2.	Server will verify the entered credentials with stored database credentials. If user verified then identity information used to generate security context and serialize that in cookies.
3.	 Cookie can consider as a piece of information that it stored in the header of http request and response. Tha information will carried back and forth between web server and browser.
4.	Cookie information can be only shared within same domain and can’t send request from different server that carried same cookie.
5.	This serialized authenticated cookie will send back to the browser then browser will redirect the user to different page.
6.	Every subsequent request will contain the cookie and web server will de-serialized the security context in cookie and know whether the user logged in or not?
7.	This part of de-serialization is also the part of authentication. One statement the authentication is who you are?
8.	Once we have security context then we will check authorization whether the user have access this page or not?

 

ASP.NET Core Basics:
1.	Chain of responsibility design pattern: In order to process request from frontend to backend, we use design pattern and here we use pattern chain of responsibility.  So, we would have many functions instead of one function. User request message will transfer to first function that will process the message then to another function and up to last function then return the message back to all function and finally generate final http response and send back to the browser. The message is http context object that encapsulate both the http request and response. 
2.	Last function Have Second Pipeline:  will responsible to process basis logic here.  It has own pipeline because according to the different requests we process differently. Second pipeline is not handling cross cutting concerns. It will handling the specific concerns to specific request. Here we have different functions instead of different functions(Middlewares)
3.	First Pipeline: is handling cross cutting concerns. Like exceptional handing, authentication and authorization. 
 



ASP.NET Core Security Context:
1.	Security context has all the information that user required for security purpose. That include the user information like username and others.
2.	All the information encapsulates in one single object which is claims principle.
3.	Principle represents the logged in users.
4.	Claims principal object that represents this security context of the user. Principle contains one or many identities of the users.
5.	One person can have many identities like user can be a student and employee, and have driver license and employee card.
6.	Next level is Claims, one identity can have many claims. Claims are key value pairs that carries your information.
7.	Example:  identity is driver license and driver license have name, birthday, blood group, address etc. All of these are claims. 
8.	One claim has one key pairs.
9.	Authorization  is used these claims.
10.	Curtains claims to be present in order to gain access to resources. 
 

11.	If logged in then create the security context otherwise create anonymous identity which have still a security context.


12.	Now below following steps are used to create security context, Serialized Cookies:
I.	verify the credentials
II.	we will add claim in key pairs:
III.	we need to add these claims in identity. and give in it authentication type.
IV.	once we have identity then needs for claims principles.
V.	now we have generated the claims principle next step is encrypt and serialized security context.
VI.	here we also specify the authentication scheme and principle.
VII.	this will serialized the claims principle into the strings and encrypt that string and save that cookie or token or session and http context object.
VIII.	Authentication can’t directly interact with cookie or token for this we will use IAuthenticanServe that contains the abstraction. Cookie and token are different authentication handles that are implemented. 
IX.	We just tell the asp.net core to tell what what authentication scheme or type used.

 

//======verify the credentials
            if (Credential.userName=="admin" && Credential.Password=="password")
            {
                //========creating the security context: we will add claim in key pairs:
                var claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name,"admin"),
                    new Claim(ClaimTypes.Email,"admin@gmail.com")
                };

                //=========== we need to add these claims in identity. and give in it authentication type
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                
                //======once we have identity then needs for claims principles.
                ClaimsPrincipal claimsPrincipal=new ClaimsPrincipal(identity);

                // now we have generated the claims principle next step is encrypt and serialized security context.

                //=======here we also specify the authentication scheme and principle.
                await HttpContext.SignInAsync("MyCookieAuth",claimsPrincipal); //this will serialized the claims principle
                                                                               //into the strings and encrypt that string and save that 
                                                                               // cookie and http context object.


Now register this service in program.cs file:
builder.Services.AddAuthentication().AddCookie("MyCookieAuth", options =>
{
    //========cookie name is the most important aspect in cookie authentication.Authentication scheme name
    options.Cookie.Name = "MyCookieAuth";
});

This is the security context:
 








Deserialized the security cookie And Authentication Do:
1.	Now we need to implement Authentication middleware between the service pipeline to set primary Authentication:
 

2.	app.UseAuthentication(); we have inserted the authentication middleware which is responsible for authentication handler calling.
3.	Because of this middleware we are able to convert the cookies to in our claims principles.
4.	Authentication Scheme name provides the logical grouping of authentication handlers, identities and claims principle all together. For this we will give authentication scheme name in addAuthentication handler to know which scheme I used:
// Add services to the container.
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    //========cookie name is the most important aspect in cookie authentication.
    options.Cookie.Name = "MyCookieAuth";
}); 
Authorization Architecture and Flow:
1.	Before authentication middleware their will be a routing middle that will tell us which endpoint trying to navigate.
2.	Then authentication middleware deserialized the cookies into our security context like principle, claims and identity.
3.	Then the authorization middleware what will look at claims of user and check the requirements of page and compare them and make shower that the claim satisfy the requirements then allow user to access the page.
4.	Otherwise, it will return http 403 unauthorized error.
5.	Without those requirements, in asp.net core we have the requirements then we group different requirements into the groups together to form a policy.
6.	One Policy can have one or more requirements.
7.	Define these policies into configure services method in program.cs file and we will apply this policy using the authorize attributes on the appropriate pages.
8.	These requirements are pretty simple. If requirement claims has the admin values then give access to admin for page and etc.
9.	Each requirement in asp.net core have handler to handle requirement.
10.	Authorization middleware uses the IAuthorizationService and the handler uses for checking the requirements. For checking specific requirement check like the age of user check.
 





How to implement Authorization:
1.	Now we want to deny anonymous authorization from the index page. So we will add this [Authorize].
2.	We add policy in program.cs:
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBelongToHRDeparment",
        policy => policy.RequireClaim("Department", "HR"));
});
3.	Now apply policy in human resource page.
[Authorize(Policy ="MustBelongToHRDepartment")]
    public class HumanResourceModel : PageModel
    {}
4.	Now we will defined new claims in login for authorization:
var claims = new List<Claim> { 
                    new Claim(ClaimTypes.Name,"admin"),
                    new Claim(ClaimTypes.Email,"admin@gmail.com"),
                     new Claim("Department","HR")
                };
 
How ImplementLogout:
5.	For Logout 
6.	 
7.	Now we create partial view page for logout.
8.	 
9.	 




How to implement Specific case of Authorization:
1.	Any requirement derived from IAuthorizationRequirement interface.

 

2.	Now we register this in program.cs file and Interface and service:
3.	 
4.	 

5.	Now developer have choice to set the expiry date in cookie:

How to cookie effective by the lifetime of browser session:
1.	Specify the expiry time.
2.	 
3.	Browser Session is the interaction between browser and server. We use this for saving the information because in cookies when we close the browser all the information contain in cookies removed.
4.	If the browser session life time is longer than the cookie life time then it will not effect cookie lifetime. Cookie will expire when the time comes.
5.	 
6.	If Cookie lifetime is longer than the browser session then it will effect if browser is gone then cookie will gone.
7.	 
8.	USA developer set Persistent Cookie: if the browser session expired then cookie should servive. 
9.	 
10.	How do we set cookie as Persistent Cookie:
11.	We add the checkbox at the login screen for persistent cookies.
12.	 
13.	
