import {
  //JsonHubProtocol,
  //HubConnectionState,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

import { addImageManipulations } from "./drawingServices";

window.addEventListener("load", async (event) => {
  const hubConnection = new HubConnectionBuilder()
    .withUrl("/imageExchanging")
    .configureLogging(LogLevel.Information)
    .build();

  await hubConnection.start();

  addImageManipulations(hubConnection);
});
