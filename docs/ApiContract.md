# CampusConnect API Contract Specification

**Version:** 1.0.0  
**Last Updated:** 2026-10-19  
**Source of Truth:** `backend/src/Api/` and `backend/src/SharedKernel/` (.NET 10 Web API)  
**Ref:** `workflow.md §0 AI Development Guidelines | Feature: API Layer Contract`

---

## 1. Global API Configuration & Runtime Facts

### Base URL & Routing
- In development, the backend runs on `http://localhost:5000` or `https://localhost:5001` (or Kestrel configured port).
- The frontend Axios client uses `baseURL: "/api"` (proxying or direct) or `process.env.NEXT_PUBLIC_API_BASE_URL`.
- Routes in ASP.NET Core controllers are defined without a global prefix in `Program.cs`, but each controller specifies routes starting with `api/` (e.g. `api/auth`, `api/community`, `api/communities`, `api/event`, `api/user`).

### Authentication & Token Flow
- **Authentication Scheme:** JWT Bearer via cookie `accessToken`.
- **Login Flow:**
  - `POST api/auth/login` receives `{ username, password }`.
  - On valid credentials, sets an HTTP-only, secure cookie named `accessToken` with `SameSite = SameSiteMode.Lax`.
  - Response body: empty (HTTP 200 OK). No JSON body returned!
  - `TokenValidationParameters`: Validates Issuer (`Jwt:Issuer`), Audience (`Jwt:Audience`), Lifetime, and Signing Key (`Jwt:SecretKey`). `ClockSkew = TimeSpan.Zero`.
  - `JwtBearerEvents.OnMessageReceived` retrieves the token from `context.Request.Cookies["accessToken"]`.
  - Claims in JWT: `sub` (Account Guid as string).
- **Logout Flow:**
  - `POST api/auth/logout`.
  - Deletes `accessToken` cookie: `Response.Cookies.Delete("accessToken", new CookieOptions { SameSite = SameSiteMode.Lax, HttpOnly = true, Secure = true })`.
  - Response body: empty (HTTP 200 OK).
- **Client Auth Header Note:** Although frontend previously attempted to attach `Authorization: Bearer <token>` from `localStorage`, the backend *exclusively* reads the JWT from `Cookies["accessToken"]`. Frontend requests must use `withCredentials: true`.

### CORS Policy
- Configured in `Program.cs`:
  - Allowed Origins: `http://localhost:3000`, `https://campus-connect-frontend-dqftc4ddd8ejbwd5.indiasouthcentral-01.azurewebsites.net`.
  - `AllowAnyHeader()`, `AllowAnyMethod()`, `AllowCredentials()`.

### JSON Serialization & Model Formatting
- ASP.NET Core defaults to **camelCase** for JSON property names (e.g. `communityName`, `eventDate`, `items`, `totalCount`).
- **Enums:**
  - `CommunityStatusType`, `CommunityType`, `Department`, `EventCategory`, `EventVisibility` all have `[JsonConverter(typeof(JsonStringEnumConverter))]`. They serialize as strings (e.g. `"Public"`, `"Academic"`).
  - Note: `MemberType` currently does **not** have `[JsonConverter(typeof(JsonStringEnumConverter))]`, so it serializes as an integer numeric enum (`0: None`, `1: ClubOfficer`, `2: Student`, `3: ClubMember`, `4: StudentAffairsStaff`).
- **Dates:** `DateTimeOffset` serializes to ISO 8601 UTC strings with offset (e.g. `"2026-10-19T14:30:00+00:00"`).

---

## 2. Error Shapes & Exception Handling

### RFC 9110 / RFC 7807 ProblemDetails
All unhandled domain and server exceptions are caught by `GlobalExceptionHandler` (`IExceptionHandler`) in `backend/src/Api/ExceptionHandlers/GlobalExceptionHandler.cs`.
The JSON payload structure:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.x",
  "title": "Title String",
  "status": 400,
  "detail": "Descriptive message"
}
```

### Exception Mapping in `GlobalExceptionHandler`:
| Exception Type | HTTP Status | ProblemDetails Title | Detail Message |
|---|---|---|---|
| `AccountAlreadyExistsException` | 409 Conflict | `"Account Already Exists"` | `exception.Message` |
| `UsernameAlreadyTakenException` | 409 Conflict | `"Username Already Taken"` | `exception.Message` |
| `ConflictException` | 409 Conflict | `"Conflict"` | `exception.Message` |
| `InvalidCredentialsException` | 401 Unauthorized | `"Invalid Credentials"` | `exception.Message` |
| `UnauthorizedAccessException` | 401 Unauthorized | `"Unauthorized"` | `exception.Message` |
| `ForbiddenException` | 403 Forbidden | `"Forbidden"` | `exception.Message` |
| `AccountNotFoundException` | 404 NotFound | `"Account Not Found"` | `exception.Message` |
| `CommunityNotFoundException` | 404 NotFound | `"Community Not Found"` | `exception.Message` |
| `EventNotFoundException` | 404 NotFound | `"Event Not Found"` | `exception.Message` |
| `ReservationNotFoundException` | 404 NotFound | `"Reservation Not Found"` | `exception.Message` |
| Any unhandled Exception | 500 InternalServerError | `"Internal Server Error"` | `"An unexpected error occurred."` |

### FluentValidation Errors
When request validation fails in `FluentValidationActionFilter` (`backend/src/Api/Filters/FluentValidationActionFilter.cs`), it short-circuits with HTTP 400 Bad Request and `HttpValidationProblemDetails`:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "PropertyName": [
      "Validation error message 1",
      "Validation error message 2"
    ]
  }
}
```

---

## 3. Paging Specification

Paged endpoints use `PageRequest` query binding (`[FromQuery] PageRequest page`):
- **Query Parameter Names:** `page` (int, default 1), `pageSize` (int, default 20).
- **Backend Constraints:**
  - `Page`: $\ge 1$ (if $< 1$, clamped to 1).
  - `PageSize`: $1 \le pageSize \le 50$, default 20 (`MaxPageSize = 50`).
- **Response Envelope:** `PagedResult<T>`
```json
{
  "items": [ ... ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

---

## 4. Endpoints Inventory & Contract

### 4.1. Auth Module (`api/auth`)

#### `POST api/auth/register`
- **Controller:** `RegisterController`
- **Auth Required:** No
- **Request Body:** `AccountRegisterRequest` (JSON)
- **Validation Rules (`AccountRegisterRequestValidator`):**
  - `Username`: Not empty, min 3, max 50 chars (`"Username is required"`, `"Username must be at least 3 characters"`, `"Username must not exceed 50 characters"`).
  - `Email`: Not empty, valid email address mode AspNetCoreCompatible (`"Email is required"`, `"Invalid email address"`).
  - `FirstName`: Not empty, min 2, max 50 chars (`"First name is required"`, `"First name must be at least 2 characters"`, `"First name must not exceed 50 characters"`).
  - `LastName`: Not empty, min 2, max 50 chars (`"Last name is required"`, `"Last name must be at least 2 characters"`, `"Last name must not exceed 50 characters"`).
  - `Password`: Not empty, min 8 chars, regex `[a-z]` (lowercase), regex `[A-Z]` (uppercase), regex `[0-9]` (number), regex `[^a-zA-Z0-9]` (special character).
  - `PhoneNumber` (optional): Max 20 chars when not empty.
- **Success Response:** `200 OK`
  ```json
  {
    "message": "Registration successful"
  }
  ```
- **Error Responses:**
  - `400 Bad Request`: Validation failure.
  - `409 Conflict`: `AccountAlreadyExistsException` (email exists) or `UsernameAlreadyTakenException` (username exists).

#### `POST api/auth/login`
- **Controller:** `LoginController`
- **Auth Required:** No
- **Request Body:** `LoginRequest` (JSON)
- **Validation Rules (`LoginRequestValidator`):**
  - `Username`: Not empty, min 3 chars.
  - `Password`: Not empty, min 6 chars.
- **Success Response:** `200 OK`
  - Cookie: `Set-Cookie: accessToken=...; Path=/; Secure; HttpOnly; SameSite=Lax`
  - Body: empty.
- **Error Responses:**
  - `400 Bad Request`: Validation failure.
  - `401 Unauthorized`: `InvalidCredentialsException` ("Invalid credentials").

#### `POST api/auth/logout`
- **Controller:** `LogoutController`
- **Auth Required:** No (Clears cookie regardless)
- **Request Body:** None
- **Success Response:** `200 OK`
  - Deletes `accessToken` cookie. Body: empty.

#### `PATCH api/auth/change-password`
- **Controller:** `ChangePasswordController`
- **Auth Required:** Yes (`[Authorize]`, checks JWT `sub`)
- **Request Body:** `ChangePasswordRequest` (JSON)
- **Validation Rules (`ChangePasswordRequestValidator`):**
  - `OldPassword`: Not empty.
  - `NewPassword`: Not empty, min 8 chars, regex `[a-z]`, regex `[A-Z]`, regex `[0-9]`, regex `[^a-zA-Z0-9]`, not equal to `OldPassword`.
- **Success Response:** `200 OK`
  - Body: `"Password Changed Successfully"` (string literal).
- **Error Responses:**
  - `400 Bad Request`: Validation failure.
  - `401 Unauthorized`: Missing/invalid JWT or invalid old credentials.

---

### 4.2. User Module (`api/user`)

#### `GET api/user/me`
- **Controller:** `MeController`
- **Auth Required:** Yes (`[Authorize]`)
- **Query / Body:** None
- **Success Response:** `200 OK`
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "johndoe",
    "accountVerifiedAt": null,
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "john@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "phoneNumber": null,
      "profileImageUrl": null
    }
  }
  ```
- **Error Responses:**
  - `401 Unauthorized`: Missing `sub` or unauthorized.
  - `404 Not Found`: `AccountNotFoundException`.

---

### 4.3. Communities Module (`api/community` & `api/communities`)

#### `GET api/communities/all`
- **Controller:** `GetAllPublicCommunitiesController`
- **Auth Required:** Yes (`[Authorize]`)
- **Query Params:** `page` (int, opt), `pageSize` (int, opt)
- **Success Response:** `200 OK`
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "communityName": "Robotics Club",
        "communityType": "Public",
        "createdBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "status": "Active"
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
  ```
- **Error Responses:**
  - `401 Unauthorized`

#### `GET api/community/mine`
- **Controller:** `GetMyCommunityController`
- **Auth Required:** Yes (`[Authorize]`)
- **Query Params:** `page` (int, opt), `pageSize` (int, opt)
- **Success Response:** `200 OK`
  - Shape: `PagedResult<Community>` (same as above).
- **Error Responses:**
  - `401 Unauthorized`
  - `404 Not Found`: Account not found.

#### `GET api/community/search/{communityName}`
- **Controller:** `SearchCommunityController`
- **Auth Required:** Yes (`[Authorize]`)
- **Route Params:** `communityName` (string, validated by `SearchCommunityRequestValidator`: not empty, max 100)
- **Query Params:** `page` (int, opt), `pageSize` (int, opt)
- **Success Response:** `200 OK`
  - Shape: `PagedResult<Community>`
- **Error Responses:**
  - `400 Bad Request`: Empty community name or $> 100$ characters.
  - `401 Unauthorized`

#### `GET api/community/{communityId:guid}`
- **Controller:** `GetCommunityById`
- **Auth Required:** Yes (`[Authorize]`)
- **Route Params:** `communityId` (Guid, validated by `GetCommunityRequestValidator`: not empty)
- **Query Params:** `page` (int, opt), `pageSize` (int, opt) - applies to inner `CommunityMembers`
- **Success Response:** `200 OK`
  ```json
  {
    "communityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "communityName": "Tech Club",
    "communityType": "Public",
    "createdBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "communityMembers": {
      "items": [
        {
          "memberId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          "communityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          "memberType": 1,
          "createdAt": "2026-10-19T10:00:00+00:00",
          "modifiedAt": null,
          "user": {
            "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            "email": "user@example.com",
            "firstName": "Alice",
            "lastName": "Smith",
            "profileImageUrl": null
          }
        }
      ],
      "totalCount": 1,
      "page": 1,
      "pageSize": 20,
      "totalPages": 1
    }
  }
  ```
- **Error Responses:**
  - `400 Bad Request`: Invalid GUID.
  - `401 Unauthorized`
  - `404 Not Found`: `CommunityNotFoundException`.

#### `POST api/community/create`
- **Controller:** `CreateCommunityController`
- **Auth Required:** Yes (`[Authorize]`)
- **Request Body:** `CreateCommunityRequest` (JSON)
- **Validation Rules (`CreateCommunityRequestValidator`):**
  - `CommunityName`: Not empty, min 3, max 100 chars (`"Community name is required"`).
  - `CommunityType`: Is in enum (`CommunityType.Public` or `CommunityType.Private`).
- **Success Response:** `200 OK` (empty body).
- **Error Responses:**
  - `400 Bad Request`: Validation failure.
  - `401 Unauthorized`
  - `404 Not Found`: Account/user not found.

#### `POST api/community/{communityId:guid}/join`
- **Controller:** `JoinPublicCommunityController`
- **Auth Required:** Implicit via `User.FindFirst("sub")`
- **Route Params:** `communityId` (Guid, validated by `JoinPublicCommunityRequestValidator`: not empty)
- **Success Response:** `200 OK` (empty body).
- **Error Responses:**
  - `400 Bad Request`: Invalid GUID.
  - `401 Unauthorized`: Missing `sub`.
  - `404 Not Found`: `CommunityNotFoundException`.
  - `409 Conflict`: `ConflictException` (already a member of this community).

---

### 4.4. Events Module (`api/event`)

#### `GET api/event/all`
- **Controller:** `GetAllPublicEventController`
- **Auth Required:** Yes (`[Authorize]`)
- **Query Params:** `page` (int, opt), `pageSize` (int, opt)
- **Success Response:** `200 OK`
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "title": "Hackathon 2026",
        "location": "Main Hall",
        "eventDate": "2026-11-01T09:00:00+00:00",
        "eventEndDate": "2026-11-01T18:00:00+00:00",
        "visibility": "Public",
        "category": "Academic"
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
  ```
- **Error Responses:**
  - `401 Unauthorized`

#### `GET api/event/{eventId:guid}`
- **Controller:** `GetEventController`
- **Auth Required:** Yes (`[Authorize]`)
- **Route Params:** `eventId` (Guid, validated by `GetEventRequestValidator`: not empty)
- **Success Response:** `200 OK`
  ```json
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Hackathon 2026",
    "description": "Full day coding event",
    "location": "Main Hall",
    "communityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "createdBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "eventDate": "2026-11-01T09:00:00+00:00",
    "eventEndDate": "2026-11-01T18:00:00+00:00",
    "visibility": "Public",
    "category": "Academic",
    "organizer": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "organizer@example.com",
      "firstName": "Organizer",
      "lastName": "User",
      "phoneNumber": null,
      "profileImageUrl": null
    },
    "organizerCommunity": {
      "communityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "communityName": "Tech Club",
      "communityType": "Public"
    }
  }
  ```
- **Error Responses:**
  - `400 Bad Request`: Invalid GUID.
  - `401 Unauthorized`
  - `404 Not Found`: `EventNotFoundException`.

#### `POST api/event/create`
- **Controller:** `EventController`
- **Auth Required:** Yes (`[Authorize]`)
- **Request Body:** `EventRequest` (JSON)
- **Validation Rules (`EventRequestValidator`):**
  - `Title`: Not empty, min 3, max 200 chars.
  - `Description`: Not empty, min 10 chars.
  - `Location`: Not empty, min 2 chars.
  - `CommunityId`: Not empty Guid.
  - `EventDate`: Not empty DateTimeOffset.
  - `EventEndDate`: Not empty DateTimeOffset, greater than `EventDate`.
  - `Visibility`: IsInEnum (`Public` or `Private`).
  - `Category`: IsInEnum (`None`, `Academic`, `Social`, `Sports`, `Cultural`, `Career`, `Volunteering`, `Other`).
- **Success Response:** `200 OK` (empty body).
- **Error Responses:**
  - `400 Bad Request`: Validation failure.
  - `401 Unauthorized`: Missing `sub`.
  - `403 Forbidden`: `ForbiddenException` (user is not an officer/eligible to create events in this community).
  - `404 Not Found`: Community not found.

#### `POST api/event/join`
- **Controller:** `JoinEventController`
- **Auth Required:** Yes (`[Authorize]`)
- **Query Params:** `communityId` (Guid), `eventId` (Guid) (bound via `[FromQuery] JoinEventRequest`)
- **Validation Rules (`JoinEventRequestValidator`):**
  - `CommunityId`: Not empty.
  - `EventId`: Not empty.
- **Success Response:** `200 OK` (empty body).
- **Error Responses:**
  - `400 Bad Request`: Missing/empty GUIDs.
  - `401 Unauthorized`: Missing `sub`.
  - `403 Forbidden`: Not eligible or forbidden.
  - `404 Not Found`: `EventNotFoundException` or `CommunityNotFoundException`.
  - `409 Conflict`: `ConflictException` (already reserved / duplicate reservation).

#### `GET api/event/reservation/{eventId:guid}`
- **Controller:** `GetEventReservationDetailsController`
- **Auth Required:** Yes (`[Authorize]`)
- **Route Params:** `eventId` (Guid, validated by `GetEventReservationDetailsRequestValidator`: not empty)
- **Query Params:** `page` (int, opt), `pageSize` (int, opt)
- **Success Response:** `200 OK`
  ```json
  {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "reservedAt": "2026-10-19T12:00:00+00:00",
        "firstName": "John",
        "lastName": "Doe",
        "email": "john@example.com",
        "profileImageUrl": null
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
  ```
- **Error Responses:**
  - `400 Bad Request`: Invalid GUID.
  - `401 Unauthorized`
  - `403 Forbidden`: `ForbiddenException` (current user is NOT the creator of this event).
  - `404 Not Found`: Event or user not found.

#### `GET api/event/reservations/mine`
- **Controller:** `GetMyEventReservationsController`
- **Auth Required:** Yes (`[Authorize]`)
- **Query Params:** `page` (int, opt), `pageSize` (int, opt)
- **Success Response:** `200 OK`
  ```json
  {
    "items": [
      {
        "reservationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "eventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "reservedAt": "2026-10-19T12:00:00+00:00",
        "title": "Hackathon 2026",
        "description": "Full day coding event",
        "location": "Main Hall",
        "eventDate": "2026-11-01T09:00:00+00:00",
        "eventEndDate": "2026-11-01T18:00:00+00:00",
        "visibility": "Public",
        "category": "Academic",
        "communityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
      }
    ],
    "totalCount": 1,
    "page": 1,
    "pageSize": 20,
    "totalPages": 1
  }
  ```
- **Error Responses:**
  - `401 Unauthorized`
  - `404 Not Found`: Account/user not found.

#### `DELETE api/event/reservation/{eventId:guid}`
- **Controller:** `CancelEventReservationController`
- **Auth Required:** Yes (`[Authorize]`)
- **Route Params:** `eventId` (Guid, validated by `CancelEventReservationRequestValidator`: not empty)
- **Success Response:** `204 No Content`
- **Error Responses:**
  - `400 Bad Request`: Invalid GUID.
  - `401 Unauthorized`
  - `404 Not Found`: `ReservationNotFoundException` or `AccountNotFoundException`.

---

## 5. How to Add an Endpoint (Sync Guide)
To maintain complete parity and prevent silent drift when creating new features:
1. **Backend Implementation:** Define domain model, database migration, facade/use-case, repository, request DTO, FluentValidation validator, controller endpoint, and register services in `Program.cs`.
2. **Contract Update:** Document the new route, method, params, validation rules, response shape, and error codes in this file (`docs/ApiContract.md`).
3. **Frontend Types:** Create or update TypeScript response types in `frontend/src/types/` matching property names and nullability.
4. **Validation Parity:** Create or update client Zod schemas matching server FluentValidation rules.
5. **Service & Query Keys:** Add the API call to `frontend/src/services/` and query key factory in `frontend/src/services/queryKeys.ts`.
6. **UI Integration:** Build or update UI components using `useQuery` / `usePaginatedQuery` / `useMutation`, handling loading, error, and empty states.
7. **Testing:** Add unit & MSW integration tests for the new endpoint and components.

---

## 6. OpenAPI / Swagger Generation
The backend uses .NET 10 Microsoft.AspNetCore.OpenApi (`AddOpenApi()` and `MapOpenApi()`), accessible at `/openapi/v1.json` when running in development mode.
- **Proposed Workflow:** When approved, add `openapi-typescript` to generate strict TypeScript types directly from the backend endpoint schema via `npm run gen:api`:
  ```bash
  npx openapi-typescript http://localhost:5000/openapi/v1.json -o src/types/api.generated.ts
  ```
- This ensures compile-time drift detection whenever the backend API schema is modified.

