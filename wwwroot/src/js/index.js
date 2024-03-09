import {
    //JsonHubProtocol,
    //HubConnectionState,
    HubConnectionBuilder,
    LogLevel
} from '@microsoft/signalr';

import { addImageManipulations } from './drawingServices.js';

window.addEventListener('load', async (event) => {

    const hubConnection = new HubConnectionBuilder()
        .withUrl("/imageExchanging")
        .configureLogging(LogLevel.Information)
        .build();

    await hubConnection.start();
 
    addImageManipulations(hubConnection);
})