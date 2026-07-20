*** Simple Demo: Net 8, c#, ASP.NET Core Web API, Clean Architecture with CQRS (without MediatR) ***
This demo as a working sample project with clean layer separation (Domain / Application / Infrastructure / Api) and a lightweight hand-rolled dispatcher standing in for MediatR. 

# 01. Create the project files.
	Create csproj files (scaffold)
	Install Packages

# 02. Domain layer:Create Product entity
	Product.CS (in Domain.Entities)

# 03. Application layer's CQRS "engine" — the abstractions and a hand-rolled dispatcher that replaces MediatR
	Create ICommand/ICommandHandler abstractions (in Application.Common.Interfaces)
	Create IQuery/IQueryHandler abstractions (in Application.Common.Interfaces)
	Create IDispatcher abstraction (in Application.Common.Interfaces)
	Create Dispatcher implementation using reflection + DI, with FluentValidation pipeline for commands (in Application.Common)
	Add Microsoft.CSharp package needed for dynamic dispatch used by the Dispatcher

# 04. The shared exceptions:
	Create ValidationException wrapping FluentValidation errors (in Application.Common.Exceptions)
	Create NotFoundException (in Application.Common.Exceptions)

# 05. The repository interface and DTO
	Create IProductRepository interface (Application.Interfaces)
	Create ProductDto (in Application.Products.Dtos)

# 06. The CreateProduct command feature (command, validator, handler)
	Create CreateProductCommand (in Application.Products.Commands.CreateProduct)
	Create CreateProductCommandValidator (in Application.Products.Commands.CreateProduct)
	Create CreateProductCommandHandler (in Application.Products.Commands.CreateProduct)
# 07. The CreateProduct command feature (command, validator, handler)
	Create GetProductByIdQuery and handler (in Application.Products.Queries.GetProductById)
	Create GetProductByIdQueryHandler (in Application.Products.Queries.GetProductById)
	Create GetProductsQuery (in Application.Products.Queries.GetProducts)
	Create GetProductsQueryHandler (in Application.Products.Queries.GetProducts)

# 08. The Application layer's DI registration (auto-registers handlers + validators via reflection, plus the dispatcher)
	Create Application DependencyInjection (in Application root)

# 09. The Infrastructure layer — EF Core DbContext and repository
	Create AppDbContext (in Infrastructure.Persistence)
	Create ProductRepository (in Infrastructure.Persistence)
	Create Infrastructure DependencyInjection (in Infrastructure root)

# 10. The API layer — Program.cs, controller, exception middleware, appsettings
	Create ExceptionHandlingMiddleware (in Api.Middleware)
	Create ProductsController (in Api.Controllers)
	Create Program.cs
	Create appsettings.json
# 11. Đầy đủ Schemas của request / response cho swagger\
	Create typed error response DTOs so Swagger can document error shapes (in Api.Contracts)
	Create typed response for the Create endpoint instead of an anonymous object (in Api.Contracts)
	Rewrite middleware to emit the typed ApiErrorResponse / ApiValidationErrorResponse shapes documented in Swagger (ApiErrorResponse in Api.Contracts)
	Overwrite middleware with typed error responses (ApiErrorResponse & ApiValidationErrorResponse)
	Thêm XML doc cho các schema request/response chính (Command, DTO) để Swagger hiển thị mô tả từng field:
	Bật generate XML doc file cho cả hai project (Api và Application) và tắt cảnh báo CS1591 cho các thành phần chưa có comment:
	Enable XML documentation file generation for Application project
	Enable XML documentation file generation for Api project
	Giờ cấu hình SwaggerGen trong Program.cs để nạp cả hai file XML và thêm thông tin document (title/version/description):

# 12. Tech notes
	- Using EF Core + SqlServer 
	- Using Add-Migration/ Update-Databse (Package Manager Console)
	- Auto-migrate lúc khởi động (chỉ ở Development)
	- Cấu trúc: Domain → Application → Infrastructure → Api, product CRUD tối giản (Create/GetById/GetAll).
	- Điểm cốt lõi — thay MediatR bằng gì:
		ICommand<T> / ICommandHandler<,> và IQuery<T> / IQueryHandler<,> — interface tự định nghĩa, không phụ thuộc thư viện ngoài.
		IDispatcher — interface duy nhất Controller biết tới (giống ISender).
		Dispatcher — dùng reflection để resolve đúng handler từ DI container rồi gọi HandleAsync; trước khi chạy Command handler, nó tự chạy IValidator<T> (FluentValidation) nếu có — thay cho IPipelineBehavior<,> mà không cần thư viện pipeline riêng.
		Application/DependencyInjection.cs quét assembly, tự AddScoped mọi handler tìm thấy — không cần đăng ký tay từng cái.
	- Controller (ProductsController) chỉ inject IDispatcher, hoàn toàn không biết EF Core hay repository nào cả.
	- Start = open swagger page ("launchUrl": "swagger/index.html",)

# 13. Vài điểm cần lưu ý:
	- Dùng EF Core InMemory cho gọn, connection string SQL Server trong appsettings.json chỉ là placeholder — đổi sang UseSqlServer(...) trong Infrastructure/DependencyInjection.cs khi cần.
	- README.md: Giải thích chi tiết hơn, kèm cả gợi ý khi nào nên dùng CQRS kiểu này so với Service Layer trực tiếp.