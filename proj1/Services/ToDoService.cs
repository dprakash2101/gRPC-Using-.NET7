using Grpc.Core;
using Microsoft.EntityFrameworkCore;

using proj1.Data;
using proj1.Models;

namespace proj1.Services;

public class ToDoService : ToDoIt.ToDoItBase
{
    private readonly AddDbContext _dbContext;

    public ToDoService(AddDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
    {
        if (request.Firstname == string.Empty || request.Lastname == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must suppply a valid object"));

        var toDoItem = new Todo
        {
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            Age = request.Age
        };

        await _dbContext.AddAsync(toDoItem);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new CreateToDoResponse
        {
            Id = toDoItem.Id
        });
    }

    public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));

        var toDoItem = await _dbContext.ToDoIt.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem != null)
        {
            return await Task.FromResult(new ReadToDoResponse
            {
                Id = toDoItem.Id,
                Firstname = toDoItem.Firstname,
                Lastname = toDoItem.Lastname,
               Age= toDoItem.Age
            });
        }

        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    }

    public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
    {
        var response = new GetAllResponse();
        var toDoItems = await _dbContext.ToDoIt.ToListAsync();

        foreach (var toDo in toDoItems)
        {
            response.ToDo.Add(new ReadToDoResponse
            {
                Id = toDo.Id,
                Firstname = toDo.Firstname,
                Lastname = toDo.Lastname,
                Age = toDo.Age
            });
        }

        return await Task.FromResult(response);
    }

    public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0 || request.Firstname == string.Empty || request.Lastname == string.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "You must suppply a valid object"));

        var toDoItem = await _dbContext.ToDoIt.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));

        toDoItem.Firstname = request.Firstname;
        toDoItem.Lastname = request.Lastname;
        toDoItem.Age = request.Age;

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new UpdateToDoResponse
        {
            Id = toDoItem.Id
        });
    }

    public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));

        var toDoItem = await _dbContext.ToDoIt.FirstOrDefaultAsync(t => t.Id == request.Id);

        if (toDoItem == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"No Task with Id {request.Id}"));

        _dbContext.Remove(toDoItem);

        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(new DeleteToDoResponse
        {
            Id = toDoItem.Id
        });

    }
}