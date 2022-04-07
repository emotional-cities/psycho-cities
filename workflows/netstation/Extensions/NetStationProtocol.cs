using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;

[Combinator]
[Description("Connects and sends commands and events to EGI Net Station.")]
[WorkflowElementCategory(ElementCategory.Source)]
public class NetStationProtocol
{
    public NetStationProtocol()
    {
        Port = 55513;
    }

    [Description("The IP address of the Net Station host computer.")]
    public string HostName { get; set; }

    [Description("The port number of the Net Station host.")]
    public int Port { get; set; }

    public IObservable<Unit> Process()
    {
        return Observable.Using(
            () => new NetStation(),
            client => Observable
                .FromAsync(async () =>
                {
                    await client.ConnectAsync(HostName, Port);
                }).Finally(async () =>
                    await client.DisconnectAsync()));
    }
}
