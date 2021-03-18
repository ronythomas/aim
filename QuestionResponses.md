##### Please describe the differences between IAAS, PAAS and SAAS and give examples of each on the Azure platform.

IAAS, PAAS and SAAS services differ in the degree of components the user has to manage vs how much the cloud service provider does over a software stack.

IAAS: 
* Infrastructure as a service
* Cloud provider manages infrastructure building blocks like Networking, Storage, VMs etc
* Examples: Azure virtual machines and Azure Storage

PAAS: 
* Platform as a service
* Cloud provider manages software and hardware stacks for services like web server, databases, cache etc which can be composed to build a platform to run user software
* Examples: App Service and SQL Azure

PAAS: 
* Software as a service
* Cloud provider manages the whole software stack
* Examples: Office 365

##### What are the considerations of a build or buy decision when planning and choosing software

Things to consider to decide between building or buying software in no particular order are

* Cost
* Maintenance
* Time to market
* Degree of control
* Support
* Extensibility / Customization 

##### What are the tradeoffs between running an application in an Azure Function versus running it in a container orchestrated by AKS

* Containers is good choice for hosting long running, always available services. Serverless is preferred for short running functions which need not be always available rather be triggered on certain events
* Containers are good to host any kind of application. There are very stringent requirements for the type of applications recommended for serverless
* Container gives a good level control over the underlying resources of the host. Serverless abstracts those details and offers very less options to interact with the same
* Local development can be setup easily with containers
* Serverless can be cost beneficial as they are created and executed only when needed and destroyed after the execution is completed
* Having said that, these are not mutually exclusive options. An azure function can be hosted and executed from a container

##### Please describe the concept of composition over inheritance
Composition over inheritance principle encourages to use composition i.e containing the super type as a member of the sub type rather than inheriting from it to achieve polymorphism or code reuse.

A good rule of thumb is to look at the relationship and chose inheritance if the subtype **"is-a"** super type (car is a vehicle) and composition if it is a **"has-a"** (car has a engine) relationship. Also a good test would be check if the object relationship satisfies LSP (Liskov's substitution principle)

Inheritance creates a tight coupling between types and any change in the super class affects the subclass and also sub classes get to know the implementation details of the parent class thereby breaking encapsulation.

Most modern language allows a class to be inherited from only one class to encourage clean modeling of classes.

##### Please describe the technique of dependency injection
Dependency injection is a design pattern used to implement another design pattern called inversion of control(IoC) where a class states and its dependent objects and they are passed in externally. 
* This ensures a loosely coupled design 
* Dependencies can be swapped without breaking the class thats using it
* Allows to test code easily
* Manage the lifetime/configuration of the dependencies externally 

It is achieved by defining a IoC container and registering service type definitions and lifetime constraints to it. The runtime injects the dependency when an object needs them at runtime.