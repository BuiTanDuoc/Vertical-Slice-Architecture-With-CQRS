# CqrsDemo — Clean Architecture + CQRS (không dùng MediatR)

Demo tối giản: .NET 8, ASP.NET Core Web API, Clean Architecture, áp dụng CQRS bằng code tự viết thay vì thư viện MediatR.

## Cấu trúc

```
src/
  CqrsDemo.Domain          # Entity thuần, không phụ thuộc layer nào khác
  CqrsDemo.Application     # Command/Query, Handler, Dispatcher, interface repository
  CqrsDemo.Infrastructure  # EF Core (InMemory), Repository implementation
  CqrsDemo.Api             # Controller, Program.cs, middleware xử lý exception
```

Chiều phụ thuộc: `Api → Infrastructure → Application → Domain` (Api cũng reference thẳng Application để lấy `IDispatcher`).

## CQRS không có MediatR hoạt động thế nào

Thay vì cài package MediatR, project tự định nghĩa 4 khối:

1. **`ICommand<TResult>` / `ICommandHandler<TCommand, TResult>`** — cho thao tác ghi.
2. **`IQuery<TResult>` / `IQueryHandler<TQuery, TResult>`** — cho thao tác đọc.
3. **`IDispatcher`** — interface duy nhất mà Controller biết tới, thay cho `ISender` của MediatR.
4. **`Dispatcher`** — implementation dùng reflection để tìm đúng `Handler` đã đăng ký trong DI container, gọi `HandleAsync`. Trước khi gọi Command handler, nó tự chạy `IValidator<T>` (FluentValidation) nếu có đăng ký — đây là cách thay thế `IPipelineBehavior<,>` của MediatR mà không cần thư viện pipeline riêng.

Đăng ký handler tự động bằng reflection trong `Application/DependencyInjection.cs` — quét toàn bộ assembly để tìm class implement `ICommandHandler<,>` / `IQueryHandler<,>` rồi `AddScoped` vào DI, không cần khai báo tay từng handler.

Controller chỉ inject `IDispatcher`, gọi `SendAsync(command)` hoặc `SendAsync(query)` — không biết gì về EF Core, repository hay handler cụ thể nào cả.

## Cấu hình kết nối MSSQL

Sửa connection string trong `src/CqrsDemo.Api/appsettings.json` (key `ConnectionStrings:Default`) cho khớp máy bạn, ví dụ:

```json
"ConnectionStrings": {
  "Default": "Server=.;Database=CqrsDemo;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Nếu dùng LocalDB (mặc định khi cài Visual Studio):

```json
"Default": "Server=(localdb)\\mssqllocaldb;Database=CqrsDemo;Trusted_Connection=True;TrustServerCertificate=True;"
```

## Tạo migration & update database (Package Manager Console trong VS)

Mở solution trong Visual Studio, mở **Tools → NuGet Package Manager → Package Manager Console**, chọn:
- **Default project**: `CqrsDemo.Infrastructure` (nơi chứa `AppDbContext`)
- **Startup Project** (dropdown trên toolbar Solution Explorer, hoặc right-click `CqrsDemo.Api` → Set as Startup Project): `CqrsDemo.Api`

Rồi chạy:

```powershell
Add-Migration InitialCreate -OutputDir Persistence/Migrations
Update-Database
```

Nếu quen dùng `dotnet ef` CLI thay vì PMC (cần cài `dotnet tool install --global dotnet-ef` nếu chưa có):

```bash
dotnet ef migrations add InitialCreate -p src/CqrsDemo.Infrastructure -s src/CqrsDemo.Api -o Persistence/Migrations
dotnet ef database update -p src/CqrsDemo.Infrastructure -s src/CqrsDemo.Api
```

Cả hai cách đều dùng chung `AppDbContextFactory` (trong `Infrastructure/Persistence/`) để đọc connection string từ `appsettings.json` của `CqrsDemo.Api` tại design-time, nên không cần chạy `AddInfrastructure`/dựng cả host mới migrate được.

Package cần thiết để `Add-Migration` / `Update-Database` hoạt động (đã có sẵn trong `CqrsDemo.Infrastructure.csproj`):
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Microsoft.EntityFrameworkCore.Design`
- `Microsoft.EntityFrameworkCore.Tools`
- `Microsoft.Extensions.Configuration.Json` (để `AppDbContextFactory` đọc được `appsettings.json`)

## Chạy thử

Do sandbox không có sẵn .NET SDK nên project chưa được build/restore ở đây — cần chạy các lệnh sau trên máy có SDK, **sau khi đã tạo migration và update database ở trên**:

```bash
cd CqrsDemo
dotnet restore
dotnet run --project src/CqrsDemo.Api
```

Ở môi trường Development, `Program.cs` tự gọi `db.Database.Migrate()` lúc khởi động để áp mọi migration còn pending — tiện cho việc test, không cần chạy `Update-Database` lại mỗi lần đổi máy. Đoạn này nên bỏ/khoá lại trước khi deploy thật, vì migrate không nên chạy ngầm lúc app khởi động ở production.

Mở Swagger tại `https://localhost:<port>/swagger` để test 3 endpoint:

- `GET /api/products` — GetProductsQuery
- `GET /api/products/{id}` — GetProductByIdQuery (trả 404 dạng problem-json nếu không tìm thấy)
- `POST /api/products` — CreateProductCommand (body: `{ "name": "...", "price": 10, "stock": 5 }`, trả 400 nếu validation fail)

## Khi nào nên tách Command/Query kiểu này thay vì Service Layer trực tiếp?

Kiểu tổ chức này hợp khi:
- Nghiệp vụ đủ phức tạp để tách rõ đọc/ghi có lợi (audit, validation, business rule riêng cho từng use case).
- Muốn mỗi use case là 1 file độc lập, dễ test riêng lẻ, dễ thêm pipeline (logging, transaction, caching...) sau này qua `Dispatcher`.

Nếu app đơn giản (CRUD thuần, ít business rule), Service Layer trực tiếp (`Controller → IXxxService → IRepository`) — như cách đã dùng ở ParkingPro — vẫn là lựa chọn nhẹ và nhanh hơn.
