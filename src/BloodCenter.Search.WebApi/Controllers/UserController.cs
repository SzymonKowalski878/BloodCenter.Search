using BloodCenter.Search.Application.Commands.AddUserCommand;
using BloodCenter.Search.Application.Queries.GetUsers;
using BloodCenter.Search.Client.Models;
using Feree.ResultType;
using Feree.ResultType.Results;
using Microsoft.AspNetCore.Mvc;

namespace BloodCenter.Search.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MediatR.IMediator _mediator;

        public UserController(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequestDto request)
        {
            try
            {
                var result = await _mediator.Send(new AddUserCommand(request));

                return result switch
                {
                    Success<Unit> success => Ok(success.Payload),
                    Failure<Unit> failure => BadRequest(failure.Error.Message),
                    _ => throw new InvalidOperationException("Unhandled result type")
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("search")]
        public Task<IActionResult> GetUsersByQuery([FromBody] GetUsersRequestDto request) =>
            SendReqeust(new GetUsersQuery(request));
        /*
        {
            try
            {
                var result = await _mediator.Send(new GetUsersByQueryQuery(request));

                return result switch
                {
                    Success<IReadOnlyList<UserDocumentDto>> success => Ok(success.Payload),
                    Failure<IReadOnlyList<UserDocumentDto>> failure => BadRequest(failure.Error.Message),
                    _ => throw new InvalidOperationException("Unhandled result type")
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        */

        private async Task<IActionResult> SendReqeust<TResponse>(MediatR.IRequest<IResult<TResponse>> mediatrRequest)
        {
            try
            {
                var result = await _mediator.Send(mediatrRequest);

                return result switch
                {
                    Success<TResponse> success => new Identity.Client.Models.CustomActionResult<TResponse>(success.Payload, System.Net.HttpStatusCode.OK),
                    Failure<TResponse> failure => new Identity.Client.Models.CustomActionResult<TResponse>(default, System.Net.HttpStatusCode.BadRequest, failure.Error.Message),
                    _ => throw new InvalidOperationException("Unhandled result type")
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
    }
}
