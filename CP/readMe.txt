1) Do not need to set multiple project to run server and clients, only run testexecutive.
2) All the possible commandline arguments are passed in config.xml file 
3) you can change local_address of server and clients from config file
4) you can change ports of server and clients
5) In config.xml, read_client port and writeclient port is starting port of first client, each extra clients's port id will be incrementd by one. So be carefull they do not overlap if the number of clients are huge.
6) You can edit num of messages to be sent from read or write client from config.xml file.
7) Analyzer shows that in sender.cs, defineprocessing function has cc 11 which does not seem to be true and the code is given by Dr.Fawcett which I haven't changed.