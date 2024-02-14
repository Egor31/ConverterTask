import { HubConnectionBuilder } from "@microsoft/signalr";

export const connection = new HubConnectionBuilder()
    .withUrl("/taskCompletedHub")
    .withAutomaticReconnect()
    .build();

connection.start().catch((err) => console.error(err.toString()));