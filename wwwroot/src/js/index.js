import {
    //JsonHubProtocol,
    //HubConnectionState,
    HubConnectionBuilder,
    LogLevel
} from '@microsoft/signalr';

import { AddImageManipulations } from './ImagesServices.js';

window.addEventListener('load', (event) => {

    const hubConnection = new HubConnectionBuilder()
        .withUrl("/imageExchanging")
        .configureLogging(LogLevel.Information)
        .build();

    AddImageManipulations(hubConnection);
})