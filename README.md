# Document generation service

## Backend

- .NET 8
- ASP.NET
- Dapper ORM
- PuppeteerSharp
- SQLite
- SignalR

## Frontend

- Vue.js 3
- SignalR
- axios

### Workflow

The user opens the webpage and then selects an .html or .htm file for conversion to PDF. A message containing the .html data is sent to a backend endpoint. On the backend, the file data is saved to a database. Then, using a Channel, the file is transferred to a background service which converts it to PDF, saves the result to the database, and informs the user that the conversion is completed by sending a message via SignalR. If the service stops working during the conversion, it will check the database for any unfinished tasks upon restart. If an unfinished task is found, the service will complete it and send a notification to the user through SignalR. After being notified, the user can either download the resulting file or delete it.
