# FamilyBoard

FamilyBoard is a webapplication created to help families with chore management, and giving parents a way to help encourage kids to do chores, by giving them a goal to work towards.
You can create a family or join one, using an invitation code, then create, assign, delete or track which tasks have been done. 

It also displays a scoreboard, so you can compete on who has collected the most points!

Demo: https://familyboard.boldbyte.dev/

## Tech
### Backend
- C# / .NET
- Dapper + Npgsql
- Swagger / OpenAPI
- PostgreSQL

### Frontend
- Vue.js v3.5 + TypeScript
- Pinia
- Bootstrap 4

 ## Installation
 1. Have Docker + Docker Compose installed on the machine.
 2. Add a '.env' file at the root of the project - an example file can be found included in the project, at the root.
 3. Copy the variables in the example file, and replace the values with your desired values. For more information, scroll down to see the environment variables table.
 4. The project is dockerized and uses Docker Compose, so start it using this command at the project root:

    ```sh
    docker-compose up --build
    ```
    To run it detatched, so you can run other commands in the same terminal, add the -d tag:
    ```sh
    docker compose up -d --build
    ```

  5. Once it is running:
     - Frontend is available at: http://localhost:${WEB_PORT}
     - Backend (Swagger UI) is available at: http://localhost:${BACKEND_PORT}/swagger



## Environment variables
| Variable | Description |
| --- | --- |
| `WEB_PORT` | The port on your machine the frontend will be available on. |
| `BACKEND_PORT` | The port on your machine the backend API will be available on. |
| `DB_PORT` | The port on your machine PostgreSQL will be exposed on. |
| `VITE_API_URL` | The URL the frontend is built with to call the backend, e.g. `http://localhost:BACKEND_PORT`. |
| `Jwt_key` | A secret key, **64 characters long**, used by the backend to sign JWT tokens. |
| `DB_PASSWORD` | Password for the Postgres user. Must match the password used in `Connection_string` below. |
| `Connection_string` | The backend's database connection string: `Host=db:<same as DB_PORT>;Username=postgres;Password=<same as DB_PASSWORD>;Database=family_board`. |
