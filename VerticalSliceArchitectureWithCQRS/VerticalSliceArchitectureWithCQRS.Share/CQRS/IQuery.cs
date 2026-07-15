//using MediatR;

namespace VerticalSliceArchitectureWithCQRS.Share.CQRS;

public interface IQuery<out TResponse>;
//public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull;
