# TJSYXYCommunication
a simple tcp/udp communication framework,support us to create multi-server(multi-client) with simple steps. 

see more: http://www.cnblogs.com/xiaozhi_5638/p/4666957.html

- file structure

![](https://github.com/sherlockchou86/TJSYXYCommunication/blob/master/file_structure.png)

- manage multiple sockets in tcp server

![](https://github.com/sherlockchou86/TJSYXYCommunication/blob/master/multiple_tcp_server.png)

- manage multiple sockets in tcp client

![](https://github.com/sherlockchou86/TJSYXYCommunication/blob/master/multiple_tcp_client.png)

- manage multiple sockets in udp client

![](https://github.com/sherlockchou86/TJSYXYCommunication/blob/master/multiple_udp_client.png)

### TCPServer
- **Create a server**

	TCPServerManager manager = new TCPServerManager("RegisterServer"); 

- **Start a server**

    TCPServerManager manager = new TCPServerManager("RegisterServer"); 	
	manager.Start(9090);
	
- **Register events**

	TCPServerManager manager = new TCPServerManager("RegisterServer"); 
	manager.TCPClientConnected += new TCPClientConnectedEventHandler();
	manager.TCPClientDisConnected += new TCPClientDisConnectedEventHandler();
	manager.TCPMessageReceived += new TCP MessageReceivedEventHandler();
	
- **Deal eessages(Event handlers)**

	private void manager_TCPMessageReceived(string csID, TCPMessageRecivedEventArgs args)
	{
     	//csID indicates the server which activated the event(because of multiple servers).
     	//args.Msg indicates the Message type we received,a Msg enum type.
     	//args.Data indicates the Message data，byte[] type.
     	//args.Time indicates the time when we received the message.
     	//args.End indicates the remote endpoint who sent the message, TCPEndPoint type.
	}
	
### TCPCLient
- **Create a client**

	TCPClientManager manager = new TCPClientManager("RegisterClient");
	
- **Connect the server**

	TCPClientManager manager = new TCPClientManager("RegisterClient");
	manager.Connect("10.0.1.10",8090);  //connect to server
	
- **Register events**

	TCPClientManager manager = new TCPClientManager("RegisterClient");
	manager.TCPMessageReceived += new TCPMessageReceivedEventHandler();
	
- **Deal messages(Event handlers)**

> the same with tcp server.

### UDPClient
- **Create a client**

	UDPClientManager manager = new UDPClientManager("WeatherMessage");
	
- **Listen the port**

	UDPClientManager manager = new UDPClientManager("WeatherMessage");
	manager.Start(9000); //listen the 9000, receive weather message
	//...
	UDPClientManager manager = new UDPClientManager("Radar");
	manager.Start(9001); //listen the 9001, receive radar data

- **Register events**

	UDPClientManager manager = new UDPClientManager("WeatherMessage");
	manager.UDPMessageReceived += new UDPMessageReceivedEventHandler();

- **Deal messages(Event handlers)**

	private void manager_UDPMessageReceived(string csID, UDPMessageReceivedEventArgs args)
	{
     	//csID indicates the client which activated the event(because of multiple clients)
     	//args.Msg indicates th message type, Msg enum type.
     	//args.Data indicates the message data, byte[] type.
     	//args.Time indicates the time when we received the message.
     	//args.RemoteIP indicates the ip who sent the message.
     	//args.RemotePort indicates the port who sent the message.
	}



